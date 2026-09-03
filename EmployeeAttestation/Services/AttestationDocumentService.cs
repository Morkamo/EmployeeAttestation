using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using EmployeeAttestation.Data;
using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace EmployeeAttestation.Services;

public sealed class AttestationDocumentService
{
    private const int MaxScoreMembers = 8;
    private const string DocumentFont = "Times New Roman";
    private const double PdfMargin = 18;
    private static readonly int[] ScoreTableWidths = [709, 4536, 850, 850, 850, 850, 850, 850, 850, 850, 1134, 1276];
    private static readonly object PdfFontSync = new();
    private static bool pdfFontResolverConfigured;
    private readonly AttestationService attestationService;
    private readonly AttestationProcessService processService;

    public AttestationDocumentService(DatabaseManager databaseManager)
    {
        ArgumentNullException.ThrowIfNull(databaseManager);
        attestationService = new AttestationService(databaseManager);
        processService = new AttestationProcessService(databaseManager);
    }

    public AttestationDocumentData Load(int attestationId)
    {
        try
        {
            Attestation attestation = attestationService.GetById(attestationId)
                ?? throw new AttestationDocumentServiceException("Аттестация не найдена.");

            return new AttestationDocumentData
            {
                Attestation = attestation,
                Criteria = processService.GetCriteria(attestationId),
                CommissionMembers = processService.GetMembers(attestationId),
                Scores = processService.GetScores(attestationId),
                Votes = processService.GetVotes(attestationId)
            };
        }
        catch (AttestationDocumentServiceException)
        {
            throw;
        }
        catch (Exception exception) when (exception is AttestationServiceException or AttestationProcessServiceException)
        {
            throw new AttestationDocumentServiceException("Не удалось подготовить данные документа.", exception);
        }
    }

    public string GetDefaultFileName(int attestationId, string extension)
    {
        AttestationDocumentData data = Load(attestationId);
        string date = (data.Attestation.AttestationDate ?? data.Attestation.CompletedAt ?? DateTime.Today)
            .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string safeName = SanitizeFileName(data.Attestation.EmployeeFullName);
        return $"Аттестация_{safeName}_{date}.{extension.TrimStart('.')}";
    }

    public void SaveDocx(int attestationId, string filePath)
    {
        AttestationDocumentData data = LoadCompleted(attestationId);
        try
        {
            using WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new W.Document();
            W.Body body = new();
            mainPart.Document.Append(body);

            AppendDocxReport(body, data);
            body.Append(CreateSectionProperties());
            mainPart.Document.Save();
        }
        catch (Exception exception)
        {
            throw new AttestationDocumentServiceException("Не удалось записать DOCX-файл. Проверьте путь и доступ к файлу.", exception);
        }
    }

    public void SavePdf(int attestationId, string filePath)
    {
        AttestationDocumentData data = LoadCompleted(attestationId);
        try
        {
            EnsurePdfFontResolver();
            using PdfDocument document = new();
            document.Info.Title = "Аттестационный лист";
            using PdfReportWriter writer = new(document);
            writer.WriteReport(data);
            writer.Save(filePath);
        }
        catch (Exception exception)
        {
            throw new AttestationDocumentServiceException("Не удалось записать PDF-файл. Проверьте путь и доступ к файлу.", exception);
        }
    }

    private AttestationDocumentData LoadCompleted(int attestationId)
    {
        AttestationDocumentData data = Load(attestationId);
        if (data.Attestation.Status != AttestationStatusHelper.Completed)
            throw new AttestationDocumentServiceException("Сохранение доступно только для завершенной аттестации.");
        return data;
    }

    private static void AppendDocxReport(W.Body body, AttestationDocumentData data)
    {
        Attestation attestation = data.Attestation;
        body.Append(CreateParagraph("АТТЕСТАЦИОННЫЙ ЛИСТ", 28, true, W.JustificationValues.Center, after: 120));
        body.Append(CreateUnderlinedCenterParagraph(attestation.EmployeeFullName, 24));
        body.Append(CreateParagraph("(Фамилия, имя, отчество (при наличии) аттестуемого работника)", 16, false, W.JustificationValues.Center, after: 80));
        body.Append(CreateUnderlinedCenterParagraph(attestation.PositionName, 24));
        body.Append(CreateParagraph("(Должность аттестуемого работника)", 16, false, W.JustificationValues.Center, after: 180));

        body.Append(CreateParagraph("Оценка аттестуемого аттестационной комиссией", 24, true, W.JustificationValues.Left, after: 90));
        body.Append(CreateScoreTable(data));

        body.Append(CreateParagraph("Решение и рекомендации аттестационной комиссии", 24, true, W.JustificationValues.Center, before: 180, after: 90));
        body.Append(CreateDecisionTable(data));

        body.Append(CreateParagraph("III. Подписи аттестующих", 24, true, W.JustificationValues.Left, before: 180, after: 90));
        body.Append(CreateSignatureTable(data));

        body.Append(CreateParagraph($"Дата проведения аттестации                   {FormatDate(attestation.AttestationDate)}", 24, false, W.JustificationValues.Left, before: 140, after: 80));
        body.Append(CreateEmployeeAcknowledgementTable());
    }

    private static W.Table CreateScoreTable(AttestationDocumentData data)
    {
        IReadOnlyList<AttestationCommissionMember> scoreMembers = GetPresentMembers(data).Take(MaxScoreMembers).ToList();
        IReadOnlyList<AttestationCriterion> criteria = GetDocumentCriteria(data);
        Dictionary<(int MemberId, int CriterionId), int> scores = data.Scores.ToDictionary(
            score => (score.AttestationCommissionMemberId, score.AttestationCriterionId), score => score.Score);

        W.Table table = CreateFixedTable(ScoreTableWidths);
        W.TableRow firstHeader = new();
        firstHeader.Append(
            CreateCell("№ п/п", 0, bold: true, center: true, verticalMerge: "restart"),
            CreateCell("Качества, характеризующие\nаттестуемого", 1, bold: true, center: true, verticalMerge: "restart"),
            CreateCell("Оценки (баллы), проставляемые членами аттестационной комиссии", 2, span: MaxScoreMembers, bold: true, center: true),
            CreateCell("Средний балл оценки", 10, bold: true, center: true, verticalMerge: "restart"),
            CreateCell("Примечание", 11, bold: true, center: true, verticalMerge: "restart"));
        table.Append(firstHeader);

        W.TableRow secondHeader = new();
        secondHeader.Append(CreateCell(string.Empty, 0, verticalMerge: "continue"), CreateCell(string.Empty, 1, verticalMerge: "continue"));
        for (int index = 0; index < MaxScoreMembers; index++)
            secondHeader.Append(CreateCell($"{index + 1}-й", index + 2, center: true));
        secondHeader.Append(CreateCell(string.Empty, 10, verticalMerge: "continue"), CreateCell(string.Empty, 11, verticalMerge: "continue"));
        table.Append(secondHeader);

        W.TableRow numbersHeader = new();
        for (int index = 0; index < 12; index++)
            numbersHeader.Append(CreateCell((index + 1).ToString(CultureInfo.InvariantCulture), index, center: true));
        table.Append(numbersHeader);

        int groupNumber = 1;
        foreach (IGrouping<string, AttestationCriterion> group in criteria.GroupBy(item => item.Category))
        {
            string categoryTitle = GetDocumentCategoryTitle(group.Key);
            W.TableRow groupRow = new();
            groupRow.Append(
                CreateCell($"{groupNumber}.", 0, bold: true, center: true),
                CreateCell(categoryTitle, 1, span: 11, bold: true));
            table.Append(groupRow);

            int criterionNumber = 1;
            foreach (AttestationCriterion criterion in group.OrderBy(item => item.SortOrder).ThenBy(item => item.CriterionName))
            {
                List<double> rowScores = [];
                W.TableRow row = new();
                row.Append(CreateCell($"{groupNumber}.{criterionNumber}.", 0, center: true));
                row.Append(CreateCell(criterion.CriterionName, 1));
                for (int memberIndex = 0; memberIndex < MaxScoreMembers; memberIndex++)
                {
                    string value = string.Empty;
                    if (memberIndex < scoreMembers.Count
                        && scores.TryGetValue((scoreMembers[memberIndex].Id, criterion.Id), out int score))
                    {
                        value = score.ToString(CultureInfo.InvariantCulture);
                        rowScores.Add(score);
                    }
                    row.Append(CreateCell(value, memberIndex + 2, center: true));
                }
                row.Append(CreateCell(FormatAverage(rowScores.Count > 0 ? rowScores.Average() : null), 10, center: true));
                row.Append(CreateCell(string.Empty, 11));
                table.Append(row);
                criterionNumber++;
            }

            double? groupAverage = GetAverageForCategory(data.Attestation, group.Key);
            W.TableRow averageRow = new();
            averageRow.Append(CreateCell(string.Empty, 0), CreateCell($"Критерий оценки\n{categoryTitle.ToLower(CultureInfo.CurrentCulture)}", 1));
            for (int index = 0; index < MaxScoreMembers; index++)
                averageRow.Append(CreateCell(FormatAverage(groupAverage), index + 2, center: true));
            averageRow.Append(CreateCell(FormatAverage(groupAverage), 10, center: true));
            averageRow.Append(CreateCell(string.Empty, 11));
            table.Append(averageRow);
            groupNumber++;
        }

        return table;
    }

    private static W.Table CreateDecisionTable(AttestationDocumentData data)
    {
        Attestation item = data.Attestation;
        int[] widths = [709, 4800, 1800, 1800, 1800, 1800, 1800, 1200];
        W.Table table = CreateFixedTable(widths);
        table.Append(CreateWideRow(widths, "1.", $"Обобщенный показатель по результатам аттестации (указать значение)        _______{FormatAverage(item.OverallAverage)}___________ балла"));
        table.Append(CreateWideRow(widths, "2.", $"Решение аттестационной комиссии: (нужное отметить)        {DisplayText(item.Decision)}"));
        table.Append(CreateWideRow(widths, "3.", $"Рекомендации аттестационной комиссии        {DisplayText(item.Recommendations)}"));
        table.Append(CreateWideRow(widths, "4.", $"Количественный состав аттестационной комиссии:        _______{item.CommissionMembersCount ?? data.CommissionMembers.Count}________"));
        table.Append(CreateWideRow(widths, string.Empty, $"На заседании присутствовало        _______{item.PresentMembersCount ?? GetPresentMembers(data).Count}________ членов аттестационной комиссии"));
        table.Append(CreateWideRow(widths, string.Empty, $"Количество голосов при вынесении решения: за _____{item.VotesFor ?? 0}_______, против _____{item.VotesAgainst ?? 0}_______, воздержались _____{item.VotesAbstained ?? 0}_______"));
        return table;
    }

    private static W.TableRow CreateWideRow(IReadOnlyList<int> widths, string number, string text)
    {
        W.TableRow row = new();
        row.Append(CreateCell(number, 0, widths, bold: !string.IsNullOrWhiteSpace(number), center: true));
        row.Append(CreateCell(text, 1, widths, span: 7));
        return row;
    }

    private static W.Table CreateSignatureTable(AttestationDocumentData data)
    {
        IReadOnlyList<AttestationCommissionMember> members = data.CommissionMembers.OrderBy(item => item.SortOrder).ToList();
        int[] widths = [7200, 300, 2200, 300, 3200];
        W.Table table = CreateFixedTable(widths, borders: false);
        AppendSignatureRows(table, widths, "Председатель аттестационной комиссии:", members.FirstOrDefault(item => item.Role == "Chairperson")?.MemberFullName);
        AppendSignatureRows(table, widths, "Заместитель председателя аттестационной комиссии:", members.FirstOrDefault(item => item.Role == "DeputyChairperson")?.MemberFullName);

        List<AttestationCommissionMember> ordinaryMembers = members.Where(item => item.Role == "Member").ToList();
        for (int index = 0; index < ordinaryMembers.Count; index++)
            AppendSignatureRows(table, widths, index == 0 ? "Члены аттестационной комиссии:" : string.Empty, ordinaryMembers[index].MemberFullName);

        AppendSignatureRows(table, widths, "Секретарь аттестационной комиссии:", members.FirstOrDefault(item => item.Role == "Secretary")?.MemberFullName);
        return table;
    }

    private static void AppendSignatureRows(W.Table table, IReadOnlyList<int> widths, string label, string? name)
    {
        W.TableRow valueRow = new();
        valueRow.Append(
            CreateCell(label, 0, widths, borders: false),
            CreateCell(string.Empty, 1, widths, borders: false),
            CreateCell(ShortName(name), 2, widths, borders: false, bottomBorder: true),
            CreateCell(string.Empty, 3, widths, borders: false),
            CreateCell(string.Empty, 4, widths, borders: false, bottomBorder: true));
        table.Append(valueRow);

        W.TableRow captionRow = new();
        captionRow.Append(
            CreateCell(string.Empty, 0, widths, borders: false, halfPointSize: 16),
            CreateCell(string.Empty, 1, widths, borders: false, halfPointSize: 16),
            CreateCell("(подпись)", 2, widths, center: true, borders: false, halfPointSize: 16),
            CreateCell(string.Empty, 3, widths, borders: false, halfPointSize: 16),
            CreateCell("(расшифровка подписи)", 4, widths, center: true, borders: false, halfPointSize: 16));
        table.Append(captionRow);
    }

    private static W.Table CreateEmployeeAcknowledgementTable()
    {
        int[] widths = [4600, 10800];
        W.Table table = CreateFixedTable(widths, borders: false);
        W.TableRow row = new();
        row.Append(
            CreateCell("С решением и рекомендациями    аттестационной комиссии ознакомлен\n(дата, подпись, расшифровка подписи аттестуемого)", 0, widths, borders: false),
            CreateCell("_______________________________________________________________________________________________________________", 1, widths, borders: false));
        table.Append(row);
        return table;
    }

    private static W.Table CreateFixedTable(IReadOnlyList<int> widths, bool borders = true)
    {
        W.TableProperties properties = new(
            new W.TableWidth { Width = widths.Sum().ToString(CultureInfo.InvariantCulture), Type = W.TableWidthUnitValues.Dxa },
            new W.TableIndentation { Width = 0, Type = W.TableWidthUnitValues.Dxa },
            new W.TableLayout { Type = W.TableLayoutValues.Fixed });
        if (borders) properties.Append(CreateTableBorders());
        return new W.Table(properties, new W.TableGrid(widths.Select(width => new W.GridColumn { Width = width.ToString(CultureInfo.InvariantCulture) })));
    }

    private static W.TableBorders CreateTableBorders() => new(
        new W.TopBorder { Val = W.BorderValues.Single, Size = 4 },
        new W.LeftBorder { Val = W.BorderValues.Single, Size = 4 },
        new W.BottomBorder { Val = W.BorderValues.Single, Size = 4 },
        new W.RightBorder { Val = W.BorderValues.Single, Size = 4 },
        new W.InsideHorizontalBorder { Val = W.BorderValues.Single, Size = 4 },
        new W.InsideVerticalBorder { Val = W.BorderValues.Single, Size = 4 });

    private static W.TableCell CreateCell(
        string? text,
        int widthIndex,
        IReadOnlyList<int>? widths = null,
        int span = 1,
        bool bold = false,
        bool center = false,
        string? verticalMerge = null,
        bool borders = true,
        bool bottomBorder = false,
        int halfPointSize = 24)
    {
        IReadOnlyList<int> sourceWidths = widths ?? ScoreTableWidths;
        int width = sourceWidths.Count > widthIndex ? sourceWidths.Skip(widthIndex).Take(span).Sum() : 1000;
        W.TableCellProperties cellProperties = new(
            new W.TableCellWidth { Width = width.ToString(CultureInfo.InvariantCulture), Type = W.TableWidthUnitValues.Dxa },
            new W.TableCellMargin(
                new W.TopMargin { Width = "60", Type = W.TableWidthUnitValues.Dxa },
                new W.BottomMargin { Width = "60", Type = W.TableWidthUnitValues.Dxa },
                new W.LeftMargin { Width = "80", Type = W.TableWidthUnitValues.Dxa },
                new W.RightMargin { Width = "80", Type = W.TableWidthUnitValues.Dxa }),
            new W.TableCellVerticalAlignment { Val = W.TableVerticalAlignmentValues.Center });
        if (span > 1) cellProperties.Append(new W.GridSpan { Val = span });
        if (verticalMerge is not null) cellProperties.Append(new W.VerticalMerge { Val = verticalMerge == "restart" ? W.MergedCellValues.Restart : W.MergedCellValues.Continue });
        if (!borders || bottomBorder)
        {
            cellProperties.Append(new W.TableCellBorders(
                new W.TopBorder { Val = W.BorderValues.Nil },
                new W.LeftBorder { Val = W.BorderValues.Nil },
                new W.BottomBorder { Val = bottomBorder ? W.BorderValues.Single : W.BorderValues.Nil, Size = 4 },
                new W.RightBorder { Val = W.BorderValues.Nil }));
        }

        return new W.TableCell(cellProperties, CreateParagraph(text ?? string.Empty, halfPointSize, bold, center ? W.JustificationValues.Center : W.JustificationValues.Left));
    }

    private static W.Paragraph CreateUnderlinedCenterParagraph(string text, int halfPointSize)
    {
        W.Paragraph paragraph = CreateParagraph(text, halfPointSize, false, W.JustificationValues.Center, after: 0);
        paragraph.ParagraphProperties ??= new W.ParagraphProperties();
        paragraph.ParagraphProperties.Append(new W.ParagraphBorders(new W.BottomBorder { Val = W.BorderValues.Single, Size = 4 }));
        return paragraph;
    }

    private static W.Paragraph CreateParagraph(
        string text,
        int halfPointSize,
        bool bold,
        W.JustificationValues justification,
        int before = 0,
        int after = 0)
    {
        W.RunProperties runProperties = new(
            new W.RunFonts { Ascii = DocumentFont, HighAnsi = DocumentFont, EastAsia = DocumentFont, ComplexScript = DocumentFont },
            new W.FontSize { Val = halfPointSize.ToString(CultureInfo.InvariantCulture) },
            new W.FontSizeComplexScript { Val = halfPointSize.ToString(CultureInfo.InvariantCulture) });
        if (bold) runProperties.Append(new W.Bold());

        W.Paragraph paragraph = new(new W.ParagraphProperties(
            new W.Justification { Val = justification },
            new W.SpacingBetweenLines { Before = before.ToString(CultureInfo.InvariantCulture), After = after.ToString(CultureInfo.InvariantCulture) }));
        string[] lines = text.Replace("\r", string.Empty, StringComparison.Ordinal).Split('\n');
        for (int index = 0; index < lines.Length; index++)
        {
            if (index > 0) paragraph.Append(new W.Run(new W.Break()));
            paragraph.Append(new W.Run((W.RunProperties)runProperties.CloneNode(true), new W.Text(lines[index]) { Space = SpaceProcessingModeValues.Preserve }));
        }
        return paragraph;
    }

    private static W.SectionProperties CreateSectionProperties() => new(
        new W.PageSize { Width = 16838U, Height = 11906U, Orient = W.PageOrientationValues.Landscape },
        new W.PageMargin { Top = 284, Right = 510U, Bottom = 170, Left = 510U, Header = 0U, Footer = 0U, Gutter = 0U });

    private static IReadOnlyList<AttestationCriterion> GetDocumentCriteria(AttestationDocumentData data) =>
        data.Criteria.Where(criterion => data.Attestation.EvaluateManagerial || criterion.Category != EvaluationCategoryHelper.Managerial)
            .OrderBy(criterion => CategoryOrder(criterion.Category))
            .ThenBy(criterion => criterion.SortOrder)
            .ThenBy(criterion => criterion.CriterionName)
            .ToList();

    private static IReadOnlyList<AttestationCommissionMember> GetPresentMembers(AttestationDocumentData data) =>
        data.CommissionMembers.Where(member => member.IsPresent).OrderBy(member => member.SortOrder).ToList();

    private static int CategoryOrder(string category) => category switch
    {
        EvaluationCategoryHelper.Professional => 1,
        EvaluationCategoryHelper.Personal => 2,
        EvaluationCategoryHelper.Managerial => 3,
        _ => 9
    };

    private static string GetDocumentCategoryTitle(string category) => category switch
    {
        EvaluationCategoryHelper.Professional => "Профессиональные качества",
        EvaluationCategoryHelper.Personal => "Личностные качества",
        EvaluationCategoryHelper.Managerial => "Руководительские качества",
        _ => EvaluationCategoryHelper.GetDisplayName(category)
    };

    private static double? GetAverageForCategory(Attestation item, string category) => category switch
    {
        EvaluationCategoryHelper.Professional => item.ProfessionalAverage,
        EvaluationCategoryHelper.Personal => item.PersonalAverage,
        EvaluationCategoryHelper.Managerial => item.ManagerialAverage,
        _ => null
    };

    private static string ShortName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;
        string[] parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0];
        StringBuilder result = new(parts[0]);
        for (int index = 1; index < Math.Min(parts.Length, 3); index++)
            result.Append(' ').Append(parts[index][0]).Append('.');
        return result.ToString();
    }

    private static string SanitizeFileName(string value)
    {
        HashSet<char> invalid = Path.GetInvalidFileNameChars().ToHashSet();
        StringBuilder result = new(value.Length);
        foreach (char character in value.Trim())
            result.Append(invalid.Contains(character) || char.IsWhiteSpace(character) ? '_' : character);
        while (result.ToString().Contains("__", StringComparison.Ordinal)) result.Replace("__", "_");
        return result.Length == 0 ? "Сотрудник" : result.ToString().Trim('_');
    }

    private static string FormatDate(DateTime? value) => value?.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture) ?? "—";
    private static string FormatAverage(double? value) => value?.ToString("0.0", CultureInfo.CurrentCulture) ?? string.Empty;
    private static string DisplayText(string? value) => string.IsNullOrWhiteSpace(value) ? "___________________________________________________________________________________________" : value.Trim();

    private static void EnsurePdfFontResolver()
    {
        lock (PdfFontSync)
        {
            if (pdfFontResolverConfigured) return;
            if (GlobalFontSettings.FontResolver is null)
                GlobalFontSettings.FontResolver = new WindowsFontResolver();
            pdfFontResolverConfigured = true;
        }
    }

    private sealed class WindowsFontResolver : IFontResolver
    {
        private readonly string fontsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

        public byte[]? GetFont(string faceName)
        {
            string fileName = faceName switch
            {
                "TimesNewRoman-Bold" => "timesbd.ttf",
                "TimesNewRoman-Regular" => "times.ttf",
                "SegoeUI-Bold" => "segoeuib.ttf",
                _ => "segoeui.ttf"
            };
            string path = Path.Combine(fontsDirectory, fileName);
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals(DocumentFont, StringComparison.OrdinalIgnoreCase)
                || familyName.Equals("Times", StringComparison.OrdinalIgnoreCase))
                return new FontResolverInfo(isBold ? "TimesNewRoman-Bold" : "TimesNewRoman-Regular", false, isItalic);
            return new FontResolverInfo(isBold ? "SegoeUI-Bold" : "SegoeUI-Regular", false, isItalic);
        }
    }

    private sealed class PdfReportWriter : IDisposable
    {
        private readonly PdfDocument document;
        private PdfPage page = null!;
        private XGraphics graphics = null!;
        private readonly XFont normal = new(DocumentFont, 9, XFontStyleEx.Regular);
        private readonly XFont small = new(DocumentFont, 7, XFontStyleEx.Regular);
        private readonly XFont bold = new(DocumentFont, 9, XFontStyleEx.Bold);
        private readonly XFont title = new(DocumentFont, 14, XFontStyleEx.Bold);
        private double y;

        public PdfReportWriter(PdfDocument document)
        {
            this.document = document;
            AddPage();
        }

        public void WriteReport(AttestationDocumentData data)
        {
            Attestation attestation = data.Attestation;
            DrawCentered("АТТЕСТАЦИОННЫЙ ЛИСТ", title, 18);
            DrawLineValue(attestation.EmployeeFullName, "(Фамилия, имя, отчество (при наличии) аттестуемого работника)");
            DrawLineValue(attestation.PositionName, "(Должность аттестуемого работника)");
            DrawText("Оценка аттестуемого аттестационной комиссией", bold, ContentLeft, ContentWidth, 16);
            y += 2;
            DrawScoreTable(data);
            y += 8;
            DrawCentered("Решение и рекомендации аттестационной комиссии", bold, 16);
            DrawDecisionBlock(data);
            y += 8;
            DrawText("III. Подписи аттестующих", bold, ContentLeft, ContentWidth, 16);
            DrawSignatures(data);
            DrawText($"Дата проведения аттестации                   {FormatDate(attestation.AttestationDate)}", normal, ContentLeft, ContentWidth, 15);
            DrawText("С решением и рекомендациями аттестационной комиссии ознакомлен        ______________________________________________", normal, ContentLeft, ContentWidth, 15);
        }

        public void Save(string path) => document.Save(path);

        private void DrawScoreTable(AttestationDocumentData data)
        {
            IReadOnlyList<AttestationCommissionMember> members = GetPresentMembers(data).Take(MaxScoreMembers).ToList();
            IReadOnlyList<AttestationCriterion> criteria = GetDocumentCriteria(data);
            Dictionary<(int MemberId, int CriterionId), int> scores = data.Scores.ToDictionary(score => (score.AttestationCommissionMemberId, score.AttestationCriterionId), score => score.Score);
            double[] widths = [34, 210, 38, 38, 38, 38, 38, 38, 38, 38, 54, 66];

            DrawTableRow(["№ п/п", "Качества, характеризующие аттестуемого", "1-й", "2-й", "3-й", "4-й", "5-й", "6-й", "7-й", "8-й", "Средний балл", "Примечание"], widths, bold, 26);
            DrawTableRow(["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"], widths, normal, 16);

            int groupNumber = 1;
            foreach (IGrouping<string, AttestationCriterion> group in criteria.GroupBy(item => item.Category))
            {
                DrawTableRow([$"{groupNumber}.", GetDocumentCategoryTitle(group.Key), "", "", "", "", "", "", "", "", "", ""], widths, bold, 18);
                int criterionNumber = 1;
                foreach (AttestationCriterion criterion in group)
                {
                    List<string> row = [$"{groupNumber}.{criterionNumber}.", criterion.CriterionName];
                    List<double> values = [];
                    for (int memberIndex = 0; memberIndex < MaxScoreMembers; memberIndex++)
                    {
                        string value = string.Empty;
                        if (memberIndex < members.Count && scores.TryGetValue((members[memberIndex].Id, criterion.Id), out int score))
                        {
                            value = score.ToString(CultureInfo.InvariantCulture);
                            values.Add(score);
                        }
                        row.Add(value);
                    }
                    row.Add(FormatAverage(values.Count > 0 ? values.Average() : null));
                    row.Add(string.Empty);
                    DrawTableRow(row, widths, normal, 22);
                    criterionNumber++;
                }
                string average = FormatAverage(GetAverageForCategory(data.Attestation, group.Key));
                DrawTableRow(["", $"Критерий оценки {GetDocumentCategoryTitle(group.Key).ToLower(CultureInfo.CurrentCulture)}", average, average, average, average, average, average, average, average, average, ""], widths, normal, 22);
                groupNumber++;
            }
        }

        private void DrawDecisionBlock(AttestationDocumentData data)
        {
            Attestation item = data.Attestation;
            DrawText($"1. Обобщенный показатель по результатам аттестации (указать значение) _______{FormatAverage(item.OverallAverage)}___________ балла", normal, ContentLeft, ContentWidth, 16);
            DrawText($"2. Решение аттестационной комиссии: {DisplayText(item.Decision)}", normal, ContentLeft, ContentWidth, 16);
            DrawText($"3. Рекомендации аттестационной комиссии: {DisplayText(item.Recommendations)}", normal, ContentLeft, ContentWidth, 16);
            DrawText($"4. Количественный состав аттестационной комиссии: _______{item.CommissionMembersCount ?? data.CommissionMembers.Count}________", normal, ContentLeft, ContentWidth, 16);
            DrawText($"На заседании присутствовало _______{item.PresentMembersCount ?? GetPresentMembers(data).Count}________ членов аттестационной комиссии", normal, ContentLeft, ContentWidth, 16);
            DrawText($"Количество голосов при вынесении решения: за _____{item.VotesFor ?? 0}_______, против _____{item.VotesAgainst ?? 0}_______, воздержались _____{item.VotesAbstained ?? 0}_______", normal, ContentLeft, ContentWidth, 16);
        }

        private void DrawSignatures(AttestationDocumentData data)
        {
            IReadOnlyList<AttestationCommissionMember> members = data.CommissionMembers.OrderBy(item => item.SortOrder).ToList();
            DrawSignature("Председатель аттестационной комиссии:", members.FirstOrDefault(item => item.Role == "Chairperson")?.MemberFullName);
            DrawSignature("Заместитель председателя аттестационной комиссии:", members.FirstOrDefault(item => item.Role == "DeputyChairperson")?.MemberFullName);
            List<AttestationCommissionMember> ordinaryMembers = members.Where(item => item.Role == "Member").ToList();
            for (int index = 0; index < ordinaryMembers.Count; index++)
                DrawSignature(index == 0 ? "Члены аттестационной комиссии:" : string.Empty, ordinaryMembers[index].MemberFullName);
            DrawSignature("Секретарь аттестационной комиссии:", members.FirstOrDefault(item => item.Role == "Secretary")?.MemberFullName);
        }

        private void DrawSignature(string label, string? name)
        {
            EnsureSpace(28);
            graphics.DrawString(label, normal, XBrushes.Black, new XRect(ContentLeft, y, 330, 13), XStringFormats.TopLeft);
            graphics.DrawString(ShortName(name), normal, XBrushes.Black, new XRect(ContentLeft + 360, y, 130, 13), XStringFormats.TopLeft);
            graphics.DrawLine(XPens.Black, ContentLeft + 350, y + 13, ContentLeft + 480, y + 13);
            graphics.DrawLine(XPens.Black, ContentLeft + 500, y + 13, ContentLeft + 680, y + 13);
            y += 13;
            graphics.DrawString("(подпись)", small, XBrushes.Black, new XRect(ContentLeft + 350, y, 130, 10), XStringFormats.TopCenter);
            graphics.DrawString("(расшифровка подписи)", small, XBrushes.Black, new XRect(ContentLeft + 500, y, 180, 10), XStringFormats.TopCenter);
            y += 13;
        }

        private void DrawTableRow(IReadOnlyList<string> values, IReadOnlyList<double> widths, XFont font, double minHeight)
        {
            double height = minHeight;
            for (int index = 0; index < values.Count; index++)
                height = Math.Max(height, Wrap(values[index], font, widths[index] - 4).Count * 10 + 6);
            EnsureSpace(height);

            double x = ContentLeft;
            for (int index = 0; index < values.Count; index++)
            {
                graphics.DrawRectangle(XPens.Black, x, y, widths[index], height);
                IReadOnlyList<string> lines = Wrap(values[index], font, widths[index] - 4);
                double textY = y + 3;
                foreach (string line in lines)
                {
                    graphics.DrawString(line, font, XBrushes.Black, new XRect(x + 2, textY, widths[index] - 4, 10), XStringFormats.TopLeft);
                    textY += 10;
                }
                x += widths[index];
            }
            y += height;
        }

        private void DrawLineValue(string value, string caption)
        {
            EnsureSpace(38);
            graphics.DrawString(value, normal, XBrushes.Black, new XRect(ContentLeft, y, ContentWidth, 14), XStringFormats.TopCenter);
            graphics.DrawLine(XPens.Black, ContentLeft + 120, y + 14, ContentLeft + ContentWidth - 120, y + 14);
            y += 15;
            graphics.DrawString(caption, small, XBrushes.Black, new XRect(ContentLeft, y, ContentWidth, 10), XStringFormats.TopCenter);
            y += 20;
        }

        private void DrawCentered(string value, XFont font, double height)
        {
            EnsureSpace(height);
            graphics.DrawString(value, font, XBrushes.Black, new XRect(ContentLeft, y, ContentWidth, height), XStringFormats.TopCenter);
            y += height;
        }

        private void DrawText(string value, XFont font, double x, double width, double lineHeight)
        {
            IReadOnlyList<string> lines = Wrap(value, font, width);
            EnsureSpace(lines.Count * lineHeight);
            foreach (string line in lines)
            {
                graphics.DrawString(line, font, XBrushes.Black, new XRect(x, y, width, lineHeight), XStringFormats.TopLeft);
                y += lineHeight;
            }
        }

        private IReadOnlyList<string> Wrap(string value, XFont font, double width)
        {
            List<string> result = [];
            foreach (string sourceLine in value.Replace("\r", string.Empty, StringComparison.Ordinal).Split('\n'))
            {
                string current = string.Empty;
                foreach (string word in sourceLine.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    string candidate = current.Length == 0 ? word : $"{current} {word}";
                    if (graphics.MeasureString(candidate, font).Width <= width || current.Length == 0) current = candidate;
                    else { result.Add(current); current = word; }
                }
                result.Add(current.Length == 0 ? " " : current);
            }
            return result;
        }

        private double ContentLeft => PdfMargin;
        private double ContentWidth => page.Width.Point - PdfMargin * 2;

        private void EnsureSpace(double required)
        {
            if (y + required <= page.Height.Point - PdfMargin) return;
            AddPage();
        }

        private void AddPage()
        {
            graphics?.Dispose();
            page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            page.Orientation = PdfSharp.PageOrientation.Landscape;
            graphics = XGraphics.FromPdfPage(page);
            y = PdfMargin;
        }

        public void Dispose() => graphics.Dispose();
    }
}

public sealed class AttestationDocumentServiceException : Exception
{
    public AttestationDocumentServiceException(string message) : base(message) { }
    public AttestationDocumentServiceException(string message, Exception innerException) : base(message, innerException) { }
}
