using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class CommissionCompositionForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!;
    private Label titleLabel = null!;
    private Label commissionNameLabel = null!;
    private DataGridView compositionGrid = null!;
    private TableLayoutPanel editorLayout = null!;
    private Label memberLabel = null!;
    private ComboBox memberComboBox = null!;
    private Label roleLabel = null!;
    private ComboBox roleComboBox = null!;
    private Label sortOrderLabel = null!;
    private NumericUpDown sortOrderNumeric = null!;
    private FlowLayoutPanel actionsPanel = null!;
    private Button addButton = null!;
    private Button updateButton = null!;
    private Button removeButton = null!;
    private FlowLayoutPanel footerPanel = null!;
    private Button closeButton = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel(); titleLabel = new Label(); commissionNameLabel = new Label(); compositionGrid = new DataGridView();
        editorLayout = new TableLayoutPanel(); memberLabel = new Label(); memberComboBox = new ComboBox(); roleLabel = new Label();
        roleComboBox = new ComboBox(); sortOrderLabel = new Label(); sortOrderNumeric = new NumericUpDown(); actionsPanel = new FlowLayoutPanel();
        addButton = new Button(); updateButton = new Button(); removeButton = new Button(); footerPanel = new FlowLayoutPanel(); closeButton = new Button();
        pageLayout.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)compositionGrid).BeginInit(); editorLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sortOrderNumeric).BeginInit(); actionsPanel.SuspendLayout(); footerPanel.SuspendLayout(); SuspendLayout();
        pageLayout.ColumnCount = 1; pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(titleLabel, 0, 0); pageLayout.Controls.Add(commissionNameLabel, 0, 1); pageLayout.Controls.Add(compositionGrid, 0, 2);
        pageLayout.Controls.Add(editorLayout, 0, 3); pageLayout.Controls.Add(actionsPanel, 0, 4); pageLayout.Controls.Add(footerPanel, 0, 5);
        pageLayout.Dock = DockStyle.Fill; pageLayout.Padding = new Padding(28); pageLayout.RowCount = 6;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 4); titleLabel.Text = "Состав комиссии";
        commissionNameLabel.AutoSize = true; commissionNameLabel.Font = new Font("Segoe UI", 11F); commissionNameLabel.ForeColor = AppColors.TextSecondary;
        commissionNameLabel.Margin = new Padding(0, 0, 0, 18); commissionNameLabel.Text = "Комиссия";
        compositionGrid.AllowUserToAddRows = false; compositionGrid.AllowUserToDeleteRows = false; compositionGrid.AllowUserToResizeRows = false;
        compositionGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; compositionGrid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "ФИО", Name = "fullNameColumn", FillWeight = 150 },
            new DataGridViewTextBoxColumn { HeaderText = "Роль", Name = "roleColumn", FillWeight = 100 },
            new DataGridViewTextBoxColumn { HeaderText = "Порядок", Name = "sortOrderColumn", FillWeight = 45 });
        compositionGrid.Dock = DockStyle.Fill; compositionGrid.MultiSelect = false; compositionGrid.ReadOnly = true;
        compositionGrid.RowHeadersVisible = false; compositionGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        compositionGrid.SelectionChanged += CompositionGrid_SelectionChanged;
        editorLayout.BackColor = AppColors.Surface; editorLayout.ColumnCount = 3; editorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
        editorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37F)); editorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
        editorLayout.Controls.Add(memberLabel, 0, 0); editorLayout.Controls.Add(roleLabel, 1, 0); editorLayout.Controls.Add(sortOrderLabel, 2, 0);
        editorLayout.Controls.Add(memberComboBox, 0, 1); editorLayout.Controls.Add(roleComboBox, 1, 1); editorLayout.Controls.Add(sortOrderNumeric, 2, 1);
        editorLayout.Dock = DockStyle.Fill; editorLayout.Margin = new Padding(0, 14, 0, 0); editorLayout.Padding = new Padding(12, 8, 12, 8);
        editorLayout.RowCount = 2; editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); editorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        ConfigureEditorLabel(memberLabel, "Член комиссии"); ConfigureEditorLabel(roleLabel, "Роль"); ConfigureEditorLabel(sortOrderLabel, "Порядок");
        ConfigureComboBox(memberComboBox); ConfigureComboBox(roleComboBox); sortOrderNumeric.Dock = DockStyle.Fill;
        sortOrderNumeric.Font = new Font("Segoe UI", 10F); sortOrderNumeric.Margin = new Padding(6, 2, 0, 0); sortOrderNumeric.Maximum = 100000;
        actionsPanel.Controls.Add(addButton); actionsPanel.Controls.Add(updateButton); actionsPanel.Controls.Add(removeButton);
        actionsPanel.Dock = DockStyle.Fill; actionsPanel.Margin = new Padding(0, 12, 0, 0); actionsPanel.WrapContents = false;
        ConfigureActionButton(addButton, "Добавить", 125); addButton.Click += AddButton_Click;
        ConfigureActionButton(updateButton, "Изменить", 125); updateButton.Click += UpdateButton_Click;
        ConfigureActionButton(removeButton, "Удалить", 125); removeButton.Click += RemoveButton_Click;
        footerPanel.Controls.Add(closeButton); footerPanel.Dock = DockStyle.Fill; footerPanel.FlowDirection = FlowDirection.RightToLeft;
        footerPanel.Margin = new Padding(0, 12, 0, 0); footerPanel.WrapContents = false; ConfigureActionButton(closeButton, "Закрыть", 130);
        closeButton.DialogResult = DialogResult.Cancel;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        CancelButton = closeButton; ClientSize = new Size(900, 600); Controls.Add(pageLayout); Font = new Font("Segoe UI", 9F);
        MinimizeBox = false; Name = "CommissionCompositionForm"; StartPosition = FormStartPosition.CenterParent; Text = "Состав комиссии";
        pageLayout.ResumeLayout(false); pageLayout.PerformLayout(); ((System.ComponentModel.ISupportInitialize)compositionGrid).EndInit();
        editorLayout.ResumeLayout(false); editorLayout.PerformLayout(); ((System.ComponentModel.ISupportInitialize)sortOrderNumeric).EndInit();
        actionsPanel.ResumeLayout(false); footerPanel.ResumeLayout(false); ResumeLayout(false);
    }

    private static void ConfigureEditorLabel(Label label, string text)
    {
        label.AutoSize = true; label.Font = new Font("Segoe UI Semibold", 9F); label.ForeColor = AppColors.TextPrimary;
        label.Margin = new Padding(0, 0, 0, 4); label.Text = text;
    }

    private static void ConfigureComboBox(ComboBox comboBox)
    {
        comboBox.Dock = DockStyle.Fill; comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.Font = new Font("Segoe UI", 10F); comboBox.Margin = new Padding(0, 2, 10, 0);
    }

    private static void ConfigureActionButton(Button button, string text, int width)
    {
        button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(0, 0, 10, 0);
        button.Size = new Size(width, 44); button.Text = text; button.UseVisualStyleBackColor = false;
    }
}
