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
        sidebarControl.Size = new Size(260, 900);
        sidebarControl.TabIndex = 0;
        // 
        // contentPanel
        // 
        contentPanel.AutoScroll = true;
        contentPanel.BackColor = Color.FromArgb(247, 249, 252);
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Location = new Point(260, 0);
        contentPanel.Margin = new Padding(0);
        contentPanel.Name = "contentPanel";
        contentPanel.Size = new Size(1340, 900);
        contentPanel.TabIndex = 1;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(247, 249, 252);
        ClientSize = new Size(1600, 900);
        Controls.Add(contentPanel);
        Controls.Add(sidebarControl);
        Font = new Font("Segoe UI", 9F);
        Margin = new Padding(3);
        MinimumSize = new Size(800, 450);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Аттестация";
        ResumeLayout(false);
    }
}
