using EmployeeAttestation.Controls;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private SidebarControl sidebarControl = null!;
    private Panel contentPanel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        sidebarControl = new SidebarControl();
        contentPanel = new Panel();
        SuspendLayout();
        // 
        // sidebarControl
        // 
        sidebarControl.Dock = DockStyle.Left;
        sidebarControl.Font = new Font("Segoe UI", 10F);
        sidebarControl.Location = new Point(0, 0);
        sidebarControl.Margin = new Padding(0);
        sidebarControl.Name = "sidebarControl";
        sidebarControl.Size = new Size(483, 1369);
        sidebarControl.TabIndex = 0;
        // 
        // contentPanel
        // 
        contentPanel.BackColor = Color.FromArgb(247, 249, 252);
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Location = new Point(483, 0);
        contentPanel.Margin = new Padding(0);
        contentPanel.Name = "contentPanel";
        contentPanel.Size = new Size(2051, 1369);
        contentPanel.TabIndex = 1;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(247, 249, 252);
        ClientSize = new Size(2534, 1369);
        Controls.Add(contentPanel);
        Controls.Add(sidebarControl);
        Font = new Font("Segoe UI", 9F);
        Margin = new Padding(6);
        MinimumSize = new Size(1280, 720);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Аттестация";
        ResumeLayout(false);
    }
}
