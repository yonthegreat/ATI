<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMainMDI
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub


    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMainMDI))
        Me.MenuStrip = New System.Windows.Forms.MenuStrip()
        Me.FileMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuImportAutoPayAccountData = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReconciliationExportBatchesMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AutoPayAccountDataMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ParametersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WindowsMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewWindowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CascadeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TileVerticalToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TileHorizontalToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ArrangeIconsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.tsImportAutoPayAccountData = New System.Windows.Forms.ToolStripButton()
        Me.ImportBatchTSButton = New System.Windows.Forms.ToolStripButton()
        Me.AutoPayAccountDataTSButton = New System.Windows.Forms.ToolStripButton()
        Me.ReconExportBatchesTSButton = New System.Windows.Forms.ToolStripButton()
        Me.tsButtonEnterChargeback = New System.Windows.Forms.ToolStripButton()
        Me.tsButtonEnterDebit = New System.Windows.Forms.ToolStripButton()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.tsVersion = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MenuStrip.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.StatusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip
        '
        Me.MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileMenu, Me.ToolStripMenuItem1, Me.WindowsMenu})
        Me.MenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip.MdiWindowListItem = Me.WindowsMenu
        Me.MenuStrip.Name = "MenuStrip"
        Me.MenuStrip.Size = New System.Drawing.Size(896, 24)
        Me.MenuStrip.TabIndex = 5
        Me.MenuStrip.Text = "MenuStrip"
        '
        'FileMenu
        '
        Me.FileMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuImportAutoPayAccountData, Me.ReconciliationExportBatchesMenuItem, Me.AutoPayAccountDataMenuItem, Me.ToolStripSeparator5, Me.ExitToolStripMenuItem})
        Me.FileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder
        Me.FileMenu.Name = "FileMenu"
        Me.FileMenu.Size = New System.Drawing.Size(37, 20)
        Me.FileMenu.Text = "&File"
        '
        'mnuImportAutoPayAccountData
        '
        Me.mnuImportAutoPayAccountData.Image = CType(resources.GetObject("mnuImportAutoPayAccountData.Image"), System.Drawing.Image)
        Me.mnuImportAutoPayAccountData.ImageTransparentColor = System.Drawing.Color.Black
        Me.mnuImportAutoPayAccountData.Name = "mnuImportAutoPayAccountData"
        Me.mnuImportAutoPayAccountData.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys)
        Me.mnuImportAutoPayAccountData.Size = New System.Drawing.Size(279, 22)
        Me.mnuImportAutoPayAccountData.Text = "&Import AutoPay Account Data..."
        '
        'ReconciliationExportBatchesMenuItem
        '
        Me.ReconciliationExportBatchesMenuItem.Image = CType(resources.GetObject("ReconciliationExportBatchesMenuItem.Image"), System.Drawing.Image)
        Me.ReconciliationExportBatchesMenuItem.Name = "ReconciliationExportBatchesMenuItem"
        Me.ReconciliationExportBatchesMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.E), System.Windows.Forms.Keys)
        Me.ReconciliationExportBatchesMenuItem.Size = New System.Drawing.Size(279, 22)
        Me.ReconciliationExportBatchesMenuItem.Text = "AutoPay Import Batches..."
        '
        'AutoPayAccountDataMenuItem
        '
        Me.AutoPayAccountDataMenuItem.Image = CType(resources.GetObject("AutoPayAccountDataMenuItem.Image"), System.Drawing.Image)
        Me.AutoPayAccountDataMenuItem.Name = "AutoPayAccountDataMenuItem"
        Me.AutoPayAccountDataMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.AutoPayAccountDataMenuItem.Size = New System.Drawing.Size(279, 22)
        Me.AutoPayAccountDataMenuItem.Text = "AutoPay Account Data..."
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(276, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(279, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem2, Me.ParametersToolStripMenuItem})
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(57, 20)
        Me.ToolStripMenuItem1.Text = "&System"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(142, 22)
        Me.ToolStripMenuItem2.Text = "&Defaults..."
        '
        'ParametersToolStripMenuItem
        '
        Me.ParametersToolStripMenuItem.Name = "ParametersToolStripMenuItem"
        Me.ParametersToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
        Me.ParametersToolStripMenuItem.Text = "Parameters..."
        '
        'WindowsMenu
        '
        Me.WindowsMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewWindowToolStripMenuItem, Me.CascadeToolStripMenuItem, Me.TileVerticalToolStripMenuItem, Me.TileHorizontalToolStripMenuItem, Me.CloseAllToolStripMenuItem, Me.ArrangeIconsToolStripMenuItem})
        Me.WindowsMenu.Name = "WindowsMenu"
        Me.WindowsMenu.Size = New System.Drawing.Size(68, 20)
        Me.WindowsMenu.Text = "&Windows"
        '
        'NewWindowToolStripMenuItem
        '
        Me.NewWindowToolStripMenuItem.Name = "NewWindowToolStripMenuItem"
        Me.NewWindowToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.NewWindowToolStripMenuItem.Text = "&New Window"
        '
        'CascadeToolStripMenuItem
        '
        Me.CascadeToolStripMenuItem.Name = "CascadeToolStripMenuItem"
        Me.CascadeToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.CascadeToolStripMenuItem.Text = "&Cascade"
        '
        'TileVerticalToolStripMenuItem
        '
        Me.TileVerticalToolStripMenuItem.Name = "TileVerticalToolStripMenuItem"
        Me.TileVerticalToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.TileVerticalToolStripMenuItem.Text = "Tile &Vertical"
        '
        'TileHorizontalToolStripMenuItem
        '
        Me.TileHorizontalToolStripMenuItem.Name = "TileHorizontalToolStripMenuItem"
        Me.TileHorizontalToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.TileHorizontalToolStripMenuItem.Text = "Tile &Horizontal"
        '
        'CloseAllToolStripMenuItem
        '
        Me.CloseAllToolStripMenuItem.Name = "CloseAllToolStripMenuItem"
        Me.CloseAllToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.CloseAllToolStripMenuItem.Text = "C&lose All"
        '
        'ArrangeIconsToolStripMenuItem
        '
        Me.ArrangeIconsToolStripMenuItem.Name = "ArrangeIconsToolStripMenuItem"
        Me.ArrangeIconsToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.ArrangeIconsToolStripMenuItem.Text = "&Arrange Icons"
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsImportAutoPayAccountData, Me.ImportBatchTSButton, Me.AutoPayAccountDataTSButton, Me.ReconExportBatchesTSButton, Me.tsButtonEnterChargeback, Me.tsButtonEnterDebit})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 24)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(896, 25)
        Me.ToolStrip1.TabIndex = 9
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'tsImportAutoPayAccountData
        '
        Me.tsImportAutoPayAccountData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsImportAutoPayAccountData.Image = CType(resources.GetObject("tsImportAutoPayAccountData.Image"), System.Drawing.Image)
        Me.tsImportAutoPayAccountData.ImageTransparentColor = System.Drawing.Color.Black
        Me.tsImportAutoPayAccountData.Name = "tsImportAutoPayAccountData"
        Me.tsImportAutoPayAccountData.Size = New System.Drawing.Size(23, 22)
        Me.tsImportAutoPayAccountData.Text = "Import AutoPay Account Data"
        '
        'ImportBatchTSButton
        '
        Me.ImportBatchTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ImportBatchTSButton.Image = CType(resources.GetObject("ImportBatchTSButton.Image"), System.Drawing.Image)
        Me.ImportBatchTSButton.ImageTransparentColor = System.Drawing.Color.Black
        Me.ImportBatchTSButton.Name = "ImportBatchTSButton"
        Me.ImportBatchTSButton.Size = New System.Drawing.Size(23, 22)
        Me.ImportBatchTSButton.Text = "AutoPay Import Batches"
        '
        'AutoPayAccountDataTSButton
        '
        Me.AutoPayAccountDataTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.AutoPayAccountDataTSButton.Image = CType(resources.GetObject("AutoPayAccountDataTSButton.Image"), System.Drawing.Image)
        Me.AutoPayAccountDataTSButton.ImageTransparentColor = System.Drawing.Color.Black
        Me.AutoPayAccountDataTSButton.Name = "AutoPayAccountDataTSButton"
        Me.AutoPayAccountDataTSButton.Size = New System.Drawing.Size(23, 22)
        Me.AutoPayAccountDataTSButton.Text = "AutoPay Account Data..."
        '
        'ReconExportBatchesTSButton
        '
        Me.ReconExportBatchesTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ReconExportBatchesTSButton.Image = CType(resources.GetObject("ReconExportBatchesTSButton.Image"), System.Drawing.Image)
        Me.ReconExportBatchesTSButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ReconExportBatchesTSButton.Name = "ReconExportBatchesTSButton"
        Me.ReconExportBatchesTSButton.Size = New System.Drawing.Size(23, 22)
        Me.ReconExportBatchesTSButton.Text = "Reconciliation Export Batches"
        Me.ReconExportBatchesTSButton.Visible = False
        '
        'tsButtonEnterChargeback
        '
        Me.tsButtonEnterChargeback.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsButtonEnterChargeback.Image = CType(resources.GetObject("tsButtonEnterChargeback.Image"), System.Drawing.Image)
        Me.tsButtonEnterChargeback.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsButtonEnterChargeback.Name = "tsButtonEnterChargeback"
        Me.tsButtonEnterChargeback.Size = New System.Drawing.Size(23, 22)
        Me.tsButtonEnterChargeback.Text = "Enter ChargeBack"
        Me.tsButtonEnterChargeback.Visible = False
        '
        'tsButtonEnterDebit
        '
        Me.tsButtonEnterDebit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsButtonEnterDebit.Image = CType(resources.GetObject("tsButtonEnterDebit.Image"), System.Drawing.Image)
        Me.tsButtonEnterDebit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsButtonEnterDebit.Name = "tsButtonEnterDebit"
        Me.tsButtonEnterDebit.Size = New System.Drawing.Size(23, 22)
        Me.tsButtonEnterDebit.Text = "Enter Debit"
        Me.tsButtonEnterDebit.Visible = False
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsVersion})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 503)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(896, 22)
        Me.StatusStrip.TabIndex = 10
        Me.StatusStrip.Text = "StatusStrip"
        '
        'tsVersion
        '
        Me.tsVersion.Name = "tsVersion"
        Me.tsVersion.Size = New System.Drawing.Size(121, 17)
        Me.tsVersion.Text = "ToolStripStatusLabel1"
        '
        'frmMainMDI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(896, 525)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.MenuStrip)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.MenuStrip
        Me.MinimumSize = New System.Drawing.Size(700, 500)
        Me.Name = "frmMainMDI"
        Me.Text = "IPS AutoPay Batch Management"
        Me.MenuStrip.ResumeLayout(False)
        Me.MenuStrip.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents MenuStrip As System.Windows.Forms.MenuStrip
    Friend WithEvents FileMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuImportAutoPayAccountData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReconciliationExportBatchesMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AutoPayAccountDataMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ParametersToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WindowsMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewWindowToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CascadeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TileVerticalToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TileHorizontalToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ArrangeIconsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents tsImportAutoPayAccountData As System.Windows.Forms.ToolStripButton
    Friend WithEvents ImportBatchTSButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents AutoPayAccountDataTSButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ReconExportBatchesTSButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsButtonEnterChargeback As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsButtonEnterDebit As System.Windows.Forms.ToolStripButton
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents tsVersion As System.Windows.Forms.ToolStripStatusLabel

End Class
