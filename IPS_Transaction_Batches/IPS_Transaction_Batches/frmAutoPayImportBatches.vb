Imports System.Data
Imports System.Data.SqlClient



Public Class frmAutoPayImportBatches

    Protected Friend WithEvents objIPSUtil As New IPS_Utilities.Library
    Private blnStatusIsActive As Boolean
    Private objDR As DataRow
    Private blnCloseClicked = False



    Private Sub ImportBatches_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim strDateRange As String

        Left = Val(GetSetting(Application.ProductName, Me.Name, "Left"))
        Top = Val(GetSetting(Application.ProductName, Me.Name, "Top"))

        cmbBatchStatusFilter.Items.Add("* All *")
        cmbBatchStatusFilter.Items.Add("Active")
        cmbBatchStatusFilter.Items.Add("Import Error")
        cmbBatchStatusFilter.Items.Add("Completed")
        cmbBatchStatusFilter.Items.Add("Void")
        cmbBatchStatusFilter.SelectedIndex = 0

        cmbBatchStatus.Items.Add("Active")
        cmbBatchStatus.Items.Add("Import Error")
        cmbBatchStatus.Items.Add("Completed")
        cmbBatchStatus.Items.Add("Void")

        strDateRange = GetSetting(Application.ProductName, Me.Name, "DateRange")
        Select Case strDateRange.ToUpper
            Case "30 DAYS"
                rb30Days.Checked = True
            Case "90 DAYS"
                rb90Days.Checked = True
            Case "180 DAYS"
                rb180Days.Checked = True
            Case Else
                rb30Days.Checked = True
        End Select

        cmbBatchStatusFilter.Text = GetSetting(Application.ProductName, Me.Name, "Status")
        If cmbBatchStatusFilter.SelectedIndex = -1 Then cmbBatchStatusFilter.SelectedIndex = 0

        DisplayData()

    End Sub

    Private Sub ImportBatches_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If e.CloseReason = CloseReason.UserClosing And Not blnCloseClicked Then
            e.Cancel = True
        Else
            SaveSetting(Application.ProductName, Me.Name, "Top", Top)
            SaveSetting(Application.ProductName, Me.Name, "Left", Left)
            If rb30Days.Checked Then SaveSetting(Application.ProductName, Me.Name, "DateRange", "30 DAYS")
            If rb90Days.Checked Then SaveSetting(Application.ProductName, Me.Name, "DateRange", "90 DAYS")
            If rb180Days.Checked Then SaveSetting(Application.ProductName, Me.Name, "DateRange", "180 DAYS")
            If rbNone.Checked Then SaveSetting(Application.ProductName, Me.Name, "DateRange", "30 DAYS")
            SaveSetting(Application.ProductName, Me.Name, "Status", cmbBatchStatusFilter.Text)

        End If

    End Sub

    Private Sub btnImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImport.Click
        On Error Resume Next

        frmImportAutoPayAccountData.ShowDialog(Me)
        frmImportAutoPayAccountData.BringToFront()

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        blnCloseClicked = True
        Me.Close()
    End Sub

    Private Sub DisplayData()
        Dim objDS As DataSet
        Dim objItem As ListViewItem
        Dim strDateRange As String = "30 DAYS"

        objDR = Nothing

        blnStatusIsActive = False
        lblImportDate.Text = ""

        lblImportedCount.Text = ""
        lblImportedSum.Text = ""
        lblAcceptedCount.Text = ""
        lblAcceptedSum.Text = ""
        lblDuplicateCount.Text = ""
        lblDuplicateSum.Text = ""
        lblInvalidCount.Text = ""

        cmbBatchStatus.Items.Clear()
        cmbBatchStatus.Enabled = False

        lblApprovedCount.Text = ""
        lblApprovedSum.Text = ""
        lblDeclinedCount.Text = ""
        lblDeclinedSum.Text = ""

        btnSave.Visible = False
        btnCancel.Visible = False
        btnProcessTrans.Visible = False

        If rb30Days.Checked Then strDateRange = "30 DAYS"
        If rb90Days.Checked Then strDateRange = "90 DAYS"
        If rb180Days.Checked Then strDateRange = "180 DAYS"
        If rbNone.Checked Then strDateRange = "NONE"

        objDR = Nothing

        lvwDisplay.Items.Clear()

        Try

            objDS = objIPSUtil.GetAutoPayImportBatches(strDateRange, cmbBatchStatusFilter.Text)

            If objDS.Tables.Count = 1 Then

                For Each objRow In objDS.Tables(0).Rows

                    objItem = lvwDisplay.Items.Add(objRow("AutoPay_BatchID"))
                    objItem.SubItems.Add(objRow("Status"))
                    objItem.SubItems.Add(Format(objRow("ImportedDate"), "MM/dd/yyyy"))
                    objItem.SubItems.Add(objRow("ImportedCount") & "")
                    objItem.SubItems.Add(Format(objRow("ImportedSum"), "0.00"))
                    objItem.SubItems.Add(objRow("AcceptedCount") & "")
                    objItem.SubItems.Add(Format(objRow("AcceptedSum"), "0.00"))
                    objItem.SubItems.Add(objRow("DuplicatesCount") & "")
                    objItem.SubItems.Add(Format(objRow("DuplicatesSum"), "0.00"))
                    objItem.SubItems.Add(objRow("InvalidCount") & "")
                    objItem.ToolTipText = objRow("ImportedFile") & ""

                Next

            End If


        Catch ex As Exception


        End Try

        Try
            If lvwDisplay.Items.Count > 0 Then
                lvwDisplay.Focus()
                lvwDisplay.Items(0).Selected = True
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub lvwDisplay_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvwDisplay.SelectedIndexChanged
        Dim lngRecordID As Long
        Dim objDS As DataSet

        objDR = Nothing

        blnStatusIsActive = False
        lblImportDate.Text = ""

        lblImportedCount.Text = ""
        lblImportedSum.Text = ""
        lblAcceptedCount.Text = ""
        lblAcceptedSum.Text = ""
        lblDuplicateCount.Text = ""
        lblDuplicateSum.Text = ""
        lblInvalidCount.Text = ""

        cmbBatchStatus.Items.Clear()

        lblApprovedCount.Text = ""
        lblApprovedSum.Text = ""
        lblDeclinedCount.Text = ""
        lblDeclinedSum.Text = ""

        btnSave.Visible = False
        btnCancel.Visible = False
        btnProcessTrans.Visible = False

        If lvwDisplay.SelectedItems.Count = 0 Then Exit Sub

        Try

            lngRecordID = lvwDisplay.SelectedItems(lvwDisplay.SelectedItems.Count - 1).Text

            objDS = objIPSUtil.GetAutoPayImportBatch(lngRecordID)

            cmbBatchStatus.Items.Clear()

            If objDS.Tables.Count = 1 Then

                If objDS.Tables(0).Rows.Count = 1 Then

                    objDR = objDS.Tables(0).Rows(0)

                    blnStatusIsActive = (objDR("Status") & "") = "Active"

                    If (objDR("Status") & "") = "Active" Then

                        cmbBatchStatus.Items.Add("Active")
                        cmbBatchStatus.Items.Add("Void")
                        cmbBatchStatus.Enabled = True
                        btnProcessTrans.Visible = True
                        btnProcessTrans.Enabled = True

                    Else

                        cmbBatchStatus.Items.Add("Import Error")
                        cmbBatchStatus.Items.Add("Completed")
                        cmbBatchStatus.Items.Add("Void")
                        cmbBatchStatus.Enabled = False

                    End If

                    cmbBatchStatus.Text = (objDR("Status") & "")

                    blnStatusIsActive = (cmbBatchStatus.Text = "Active")

                    lblImportDate.Text = Format(objDR("ImportedDate"), "MM/dd/yyyy")

                    lblImportedCount.Text = Val(objDR("ImportedCount") & "")
                    lblImportedSum.Text = Format(Val(objDR("ImportedSum") & ""), "0.00")
                    lblAcceptedCount.Text = Val(objDR("AcceptedCount") & "")
                    lblAcceptedSum.Text = Format(Val(objDR("AcceptedSum") & ""), "0.00")
                    lblDuplicateCount.Text = Val(objDR("DuplicatesCount") & "")
                    lblDuplicateSum.Text = Format(Val(objDR("DuplicatesSum") & ""), "0.00")
                    lblInvalidCount.Text = Val(objDR("InvalidCount") & "")

                    lblApprovedCount.Text = Val(objDR("ApprovedCount") & "")
                    lblApprovedSum.Text = Format(Val(objDR("ApprovedSum") & ""), "0.00")
                    lblDeclinedCount.Text = Val(objDR("DeclinedCount") & "")
                    lblDeclinedSum.Text = Format(Val(objDR("DeclinedSum") & ""), "0.00")

                End If

            End If

        Catch ex As Exception


        End Try

    End Sub


    Private Sub cmbBatchStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBatchStatus.SelectedIndexChanged

        If blnStatusIsActive And cmbBatchStatus.Text = "Void" Then

            btnSave.Visible = True
            btnSave.Enabled = True
            btnCancel.Visible = True
            btnCancel.Enabled = True
            btnProcessTrans.Visible = False

            gbFilters.Enabled = False
            lvwDisplay.Enabled = False
            btnImport.Enabled = False
            btnClose.Enabled = False

        Else

            btnSave.Visible = False
            btnCancel.Visible = False

            gbFilters.Enabled = True
            lvwDisplay.Enabled = True
            btnImport.Enabled = True
            btnClose.Enabled = True
            btnProcessTrans.Visible = blnStatusIsActive

        End If

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        cmbBatchStatus.Text = "Active"
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim blnWasError As Boolean = False
        Dim strErrMessage As String = "* Invalid Update Process Response."


        If MessageBox.Show("You are about to 'Void' this AutoPay Import Batch.  This process is NOT reversible." & vbCrLf & vbCrLf & "Are you sure?", _
                           "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub

        Try

            If objIPSUtil.UpdateAutoPayImportBatch(objDR("AutoPay_BatchID"), Val(objDR("ImportedCount") & ""), Val(objDR("ImportedSum") & ""), _
                                            Val(objDR("AcceptedCount") & ""), Val(objDR("AcceptedSum") & ""), Val(objDR("DuplicatesCount") & ""), _
                                            Val(objDR("DuplicatesSum") & ""), Val(objDR("InvalidCount") & ""), Val(objDR("ApprovedCount") & ""), _
                                            Val(objDR("ApprovedSum") & ""), Val(objDR("DeclinedCount") & ""), Val(objDR("DeclinedSum") & ""), "Void") <> objDR("AutoPay_BatchID") Then blnWasError = True

        Catch ex As Exception

            blnWasError = True
            strErrMessage = "* " & ex.Message

        End Try

        If blnWasError Then
            MessageBox.Show("There was a problem updating the AutoPay Import Batch." & vbCrLf & vbCrLf & strErrMessage, _
                            "System Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else

            lvwDisplay.SelectedItems(lvwDisplay.SelectedItems.Count - 1).SubItems(1).Text = "Void"

            gbFilters.Enabled = True
            lvwDisplay.Enabled = True
            btnImport.Enabled = True
            btnClose.Enabled = True
            btnSave.Visible = False
            btnCancel.Visible = False
            btnProcessTrans.Visible = False
            cmbBatchStatus.Enabled = False

        End If

    End Sub

    Private Sub btnProcessTrans_Click(sender As Object, e As EventArgs) Handles btnProcessTrans.Click
        '
    End Sub

    Private Sub Filter_Changed(sender As Object, e As EventArgs) Handles cmbBatchStatusFilter.SelectedIndexChanged, _
                        rb30Days.CheckedChanged, rb90Days.CheckedChanged, rb180Days.CheckedChanged, rbNone.CheckedChanged
        DisplayData()
    End Sub

End Class