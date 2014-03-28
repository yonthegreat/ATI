Imports System.Windows.Forms

Public Class frmMainMDI

    Private m_ChildFormNumber As Integer

    Private Sub frmMainMDI_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Dim x As String = ""
        'x = GetAppParameter("Test")
        'SetAppParameter("Test", "TestValue", True)
        'AppendAppParameter("Test", "-NewValue", True)
        'x = GetAppParameter("Test")

        Left = Val(GetSetting(Application.ProductName, Me.Name, "Left"))
        Top = Val(GetSetting(Application.ProductName, Me.Name, "Top"))
        Height = Val(GetSetting(Application.ProductName, Me.Name, "Height"))
        Width = Val(GetSetting(Application.ProductName, Me.Name, "Width"))
        If Left < 0 Then Left = 0
        If Top < 0 Then Top = 0

    End Sub

    Private Sub frmMainMDI_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        SaveSetting(Application.ProductName, Me.Name, "Top", Top)
        SaveSetting(Application.ProductName, Me.Name, "Left", Left)
        SaveSetting(Application.ProductName, Me.Name, "Height", Height)
        SaveSetting(Application.ProductName, Me.Name, "Width", Width)
    End Sub

    Private Sub CascadeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CascadeToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub TileVerticalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TileVerticalToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Private Sub TileHorizontalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TileHorizontalToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub ArrangeIconsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ArrangeIconsToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.ArrangeIcons)
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CloseAllToolStripMenuItem.Click

        ' Close all child forms of the parent.
        For Each ChildForm As Form In Me.MdiChildren
            ChildForm.Close()
        Next

    End Sub


    Private Sub ImportAutoPayAccountData_Click(sender As Object, e As EventArgs) Handles mnuImportAutoPayAccountData.Click, tsImportAutoPayAccountData.Click
        On Error Resume Next

        frmImportAutoPayAccountData.MdiParent = Me
        frmImportAutoPayAccountData.Show()

    End Sub


    Private Sub ImportBatchTSButton_Click(sender As Object, e As EventArgs) Handles ImportBatchTSButton.Click, ReconciliationExportBatchesMenuItem.Click
        On Error Resume Next

        frmAutoPayImportBatches.MdiParent = Me
        frmAutoPayImportBatches.Show()

    End Sub


    Private Sub AutoPayAccountDataTSButton_Click(sender As Object, e As EventArgs) Handles AutoPayAccountDataTSButton.Click, AutoPayAccountDataMenuItem.Click
        On Error Resume Next

        frmAutoPayAccountData.MdiParent = Me
        frmAutoPayAccountData.Show()

    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        '
    End Sub

    Private Sub ParametersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ParametersToolStripMenuItem.Click
        '
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click

        Me.Close()

    End Sub
End Class
