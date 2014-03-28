Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.VisualBasic



Public Class frmImportAutoPayAccountData

    Protected Friend WithEvents objIPSUtil As New IPS_Utilities.Library




    Private Sub frmImportPendingCustTransData_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Left = Val(GetSetting(Application.ProductName, Me.Name, "Left"))
        Top = Val(GetSetting(Application.ProductName, Me.Name, "Top"))
        txtFileName.Text = GetSetting(Application.ProductName, Me.Name, "ImportFilename").Trim
        If txtFileName.Text = "" Then txtFileName.Text = GetSetting(Application.ProductName, Me.Name, "ImportFilename")

        lblImportDate.Text = Format(Now, "MM/dd/yyyy")

        lblImportedCount.Text = "0"
        lblImportedSum.Text = "$ 0.00"
        lblAcceptedCount.Text = "0"
        lblAcceptedSum.Text = "$ 0.00"
        lblDuplicateCount.Text = "0"
        lblDuplicateSum.Text = "$ 0.00"
        lblInvalidCount.Text = "0"

    End Sub

    Private Sub frmImportPendingCustTransData_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        SaveSetting(Application.ProductName, Me.Name, "Top", Top)
        SaveSetting(Application.ProductName, Me.Name, "Left", Left)
        SaveSetting(Application.ProductName, Me.Name, "ImportFilename", txtFileName.Text.Trim)

    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Dim OpenFileDialog As New OpenFileDialog
        Dim FileName As String = OpenFileDialog.FileName

        OpenFileDialog.Filter = "Text Files (*.txt)|*.txt|Comma Separated Values (*.csv)|*.csv|All Files (*.*)|*.*"
        OpenFileDialog.Multiselect = False

        If FileExists(txtFileName.Text) Then

            Try
                OpenFileDialog.FileName = txtFileName.Text
                If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    txtFileName.Text = OpenFileDialog.FileName
                End If
            Catch ex As Exception

            End Try
        Else
            Try
                OpenFileDialog.FileName = FilePath(txtFileName.Text) & "*.txt"
                If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    txtFileName.Text = OpenFileDialog.FileName
                End If
            Catch ex As Exception

            End Try
        End If

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImport.Click
        Dim objSR As StreamReader = Nothing
        Dim lngBatchID As Long
        Dim strLine As String = ""
        Dim strStatus As String = "Active"
        Dim strImportStatus As String = ""

        Try

            txtFileName.Enabled = False
            btnBrowse.Enabled = False
            btnImport.Enabled = False
            btnClose.Enabled = False

            lblImportDate.Text = Format(Now, "MM/dd/yyyy")

            lblImportedCount.Text = "0"
            lblImportedSum.Text = "$ 0.00"
            lblAcceptedCount.Text = "0"
            lblAcceptedSum.Text = "$ 0.00"
            lblDuplicateCount.Text = "0"
            lblDuplicateSum.Text = "$ 0.00"
            lblInvalidCount.Text = "0"

            lngBatchID = objIPSUtil.ImportAutoPayFile(txtFileName.Text, strImportStatus, lblImportDate.Text)

        Catch SE As Exception

            MessageBox.Show("An error was encountered while Importing File." & vbCrLf & vbCrLf & SE.Message, "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            lngBatchID = -1

        End Try

        If Not (Me.Owner Is Nothing) Then
            Dim lngIndex As Long = frmAutoPayImportBatches.cmbBatchStatusFilter.SelectedIndex
            frmAutoPayImportBatches.cmbBatchStatusFilter.SelectedIndex = -1
            frmAutoPayImportBatches.cmbBatchStatusFilter.SelectedIndex = lngIndex
        End If

        txtFileName.Enabled = True
        btnBrowse.Enabled = True
        btnImport.Enabled = True
        btnClose.Enabled = True

        MessageBox.Show("AutoPay Import Process completed.  Batch ID = " & lngBatchID & vbCrLf & IIf(strImportStatus = "", "", vbCrLf & "Import Message: " & strImportStatus), "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)

    End Sub



    ' Handle Event
    Private Sub objIPSUtil_ImportingStats(BatchID As Long, ImportedCount As Long, ImportedSum As Decimal, AcceptedCount As Long, AcceptedSum As Decimal, DuplicatesCount As Long, DuplicatesSum As Decimal, InvalidCount As Long) Handles objIPSUtil.ImportingStats

        gbBatch.Text = "Batch ID: " & BatchID
        lblImportedCount.Text = ImportedCount & ""
        lblImportedSum.Text = FormatCurrency(ImportedSum)
        lblAcceptedCount.Text = AcceptedCount & ""
        lblAcceptedSum.Text = FormatCurrency(AcceptedSum)
        lblDuplicateCount.Text = DuplicatesCount & ""
        lblDuplicateSum.Text = FormatCurrency(DuplicatesSum)
        lblInvalidCount.Text = InvalidCount & ""

        gbBatch.Refresh()
        Application.DoEvents()

    End Sub

End Class