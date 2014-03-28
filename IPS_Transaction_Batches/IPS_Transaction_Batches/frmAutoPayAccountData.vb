Imports System.Data
Imports System.Data.SqlClient



Public Class frmAutoPayAccountData


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

        cmbRecordStatus.Items.Add("Active")
        cmbRecordStatus.Items.Add("Accepted")
        cmbRecordStatus.Items.Add("Declined")
        cmbRecordStatus.Items.Add("Void")
        cmbRecordStatus.Items.Add("Duplicate")
        cmbRecordStatus.Items.Add("Invalid")

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

        DisplayBatchData()

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

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        blnCloseClicked = True
        Me.Close()
    End Sub


    Private Sub DisplayBatchData()
        Dim objDS As DataSet
        Dim strDateRange As String = "30 DAYS"

        If rb30Days.Checked Then strDateRange = "30 DAYS"
        If rb90Days.Checked Then strDateRange = "90 DAYS"
        If rb180Days.Checked Then strDateRange = "180 DAYS"
        If rbNone.Checked Then strDateRange = "NONE"

        objDR = Nothing

        lvwDisplay.Items.Clear()
        cmbBatchDisplayed.Items.Clear()

        Try

            objDS = objIPSUtil.GetAutoPayImportBatches(strDateRange, cmbBatchStatusFilter.Text)

            If objDS.Tables.Count = 1 Then

                For Each objRow In objDS.Tables(0).Rows

                    cmbBatchDisplayed.Items.Add("BatchID: " & objRow("AutoPay_BatchID") & "     -     Imported: " & Format(objRow("ImportedDate"), "MM/dd/yyyy") & "     -     [" & objRow("Status") & "]    -      Records:   " & objRow("ImportedCount") & ",   $" & Format(objRow("ImportedSum"), "0.00"))

                    'objItem = lvwDisplay.Items.Add(objRow("AutoPay_BatchID"))
                    'objItem.SubItems.Add(objRow("Status"))
                    'objItem.SubItems.Add(Format(objRow("ImportedDate"), "MM/dd/yyyy"))
                    'objItem.SubItems.Add(objRow("ImportedCount") & "")
                    'objItem.SubItems.Add(Format(objRow("ImportedSum"), "0.00"))
                    'objItem.SubItems.Add(objRow("AcceptedCount") & "")
                    'objItem.SubItems.Add(Format(objRow("AcceptedSum"), "0.00"))
                    'objItem.SubItems.Add(objRow("DuplicatesCount") & "")
                    'objItem.SubItems.Add(Format(objRow("DuplicatesSum"), "0.00"))
                    'objItem.SubItems.Add(objRow("InvalidCount") & "")
                    'objItem.ToolTipText = objRow("ImportedFile") & ""

                Next

            End If


        Catch ex As Exception


        End Try

        Try
            If cmbBatchDisplayed.Items.Count > 0 Then

                cmbBatchDisplayed.Focus()
                cmbBatchDisplayed.SelectedIndex = 0
                cmbBatchDisplayed.Enabled = True

            Else

                cmbBatchDisplayed.Enabled = False
                blnStatusIsActive = False
                lblAutoPayDate.Text = ""
                lblAutoPayAmount.Text = ""

                cmbRecordStatus.Items.Clear()
                cmbRecordStatus.Enabled = False

                btnSave.Visible = False
                btnCancel.Visible = False
                btnProcessTrans.Visible = False

            End If

        Catch ex As Exception
        End Try

    End Sub

    Private Sub DisplayData()
        Dim objDS As DataSet
        Dim objItem As ListViewItem
        Dim lngBatchID As Long


        objDR = Nothing

        blnStatusIsActive = False
        lblAutoPayDate.Text = ""
        lblAutoPayAmount.Text = ""
        lblAutoPayAccountID.Text = ""

        cmbRecordStatus.Items.Clear()
        cmbRecordStatus.Enabled = False

        btnSave.Visible = False
        btnCancel.Visible = False
        btnProcessTrans.Visible = False
        lvwDisplay.Items.Clear()

        If cmbBatchDisplayed.SelectedIndex = -1 Then Exit Sub

        lngBatchID = Val(cmbBatchDisplayed.Text.Replace("BatchID: ", "").Split("-").GetValue(0))

        gbBatch.Enabled = cmbBatchDisplayed.Text.Contains("[Active]")

        Try

            objDS = objIPSUtil.GetAutoPayBatchAccountData(lngBatchID)

            If objDS.Tables.Count = 1 Then

                For Each objRow In objDS.Tables(0).Rows

                    objItem = lvwDisplay.Items.Add(objRow("AutoPay_ID"))
                    objItem.SubItems.Add(objRow("Status"))
                    objItem.SubItems.Add(Format(objRow("AutoPay_Date"), "MM/dd/yyyy"))
                    objItem.SubItems.Add(objRow("Customer_AccountID") & "")
                    objItem.SubItems.Add(objRow("FirstName") & "")
                    objItem.SubItems.Add(objRow("LastName") & "")
                    objItem.SubItems.Add(Format(objRow("AutoPay_Amount"), "0.00"))

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
        Dim objDR As DataRow = Nothing


        blnStatusIsActive = False
        lblAutoPayDate.Text = ""
        lblAutoPayAmount.Text = ""
        lblAutoPayAccountID.Text = ""
        gbBatch.Text = "Record ID: "

        cmbRecordStatus.Items.Clear()

        btnSave.Visible = False
        btnCancel.Visible = False
        btnProcessTrans.Visible = False

        If lvwDisplay.SelectedItems.Count = 0 Then Exit Sub

        Try

            lngRecordID = lvwDisplay.SelectedItems(lvwDisplay.SelectedItems.Count - 1).Text

            gbBatch.Text = "Record ID: " & lngRecordID

            objDR = objIPSUtil.GetAutoPayAccountData(lngRecordID)

            cmbRecordStatus.Items.Clear()

            If Not (objDR Is Nothing) Then

                blnStatusIsActive = (objDR("Status") & "") = "Active"

                If (objDR("Status") & "") = "Active" Then

                    cmbRecordStatus.Items.Add("Active")
                    cmbRecordStatus.Items.Add("Void")
                    cmbRecordStatus.Enabled = True
                    btnProcessTrans.Visible = True
                    btnProcessTrans.Enabled = True

                Else

                    cmbRecordStatus.Items.Add("Active")
                    cmbRecordStatus.Items.Add("Accepted")
                    cmbRecordStatus.Items.Add("Declined")
                    cmbRecordStatus.Items.Add("Void")
                    cmbRecordStatus.Items.Add("Duplicate")
                    cmbRecordStatus.Items.Add("Invalid")
                    cmbRecordStatus.Enabled = False

                End If

                cmbRecordStatus.Text = (objDR("Status") & "")

                blnStatusIsActive = (cmbRecordStatus.Text = "Active")

                lblAutoPayDate.Text = Format(objDR("AutoPay_Date"), "MM/dd/yyyy")
                lblAutoPayAmount.Text = Format(objDR("AutoPay_Amount"), "0.00")
                lblAutoPayAccountID.Text = objDR("Customer_AccountID") & ""

            End If

        Catch ex As Exception


        End Try

    End Sub


    Private Sub cmbBatchStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbRecordStatus.SelectedIndexChanged

        If blnStatusIsActive And cmbRecordStatus.Text = "Void" Then

            btnSave.Visible = True
            btnSave.Enabled = True
            btnCancel.Visible = True
            btnCancel.Enabled = True
            btnProcessTrans.Visible = False

            gbFilters.Enabled = False
            lvwDisplay.Enabled = False
            btnClose.Enabled = False

        Else

            btnSave.Visible = False
            btnCancel.Visible = False

            gbFilters.Enabled = True
            lvwDisplay.Enabled = True
            btnClose.Enabled = True
            btnProcessTrans.Visible = blnStatusIsActive

        End If

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        cmbRecordStatus.Text = "Active"
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim blnWasError As Boolean = False
        Dim strErrMessage As String = "* Invalid Update Process Response."


        If MessageBox.Show("You are about to 'Void' this AutoPay Account Data.  This process is NOT reversible." & vbCrLf & vbCrLf & "Are you sure?", _
                           "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub

        Try

            If objIPSUtil.UpdateAutoPayAccountData(objDR("AutoPay_ID"), objDR("AutoPay_Date"), Val(objDR("AutoPay_Amount") & ""), _
                                            objDR("Customer_AccountID") & "", objDR("LastName") & "", objDR("FirstName") & "", _
                                            objDR("BillAddr1") & "", objDR("BillCity") & "", objDR("BillState") & "", _
                                            objDR("BillZip") & "", "Void") <> objDR("AutoPay_ID") Then blnWasError = True

        Catch ex As Exception

            blnWasError = True
            strErrMessage = "* " & ex.Message

        End Try

        If blnWasError Then
            MessageBox.Show("There was a problem updating the AutoPay Account Data." & vbCrLf & vbCrLf & strErrMessage, _
                            "System Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else

            lvwDisplay.SelectedItems(lvwDisplay.SelectedItems.Count - 1).SubItems(1).Text = "Void"

            gbFilters.Enabled = True
            lvwDisplay.Enabled = True
            btnClose.Enabled = True
            btnSave.Visible = False
            btnCancel.Visible = False
            btnProcessTrans.Visible = False
            cmbRecordStatus.Enabled = False

        End If

    End Sub

    Private Sub btnProcessTrans_Click(sender As Object, e As EventArgs) Handles btnProcessTrans.Click
        Dim strTransResult As String
        Dim blnWasError As Boolean = False
        Dim strErrMessage As String = "* Invalid Update Process Response."


        If MessageBox.Show("You are about to process a Transaction using this AutoPay Account Data." & vbCrLf & vbCrLf & "Are you sure?", _
                   "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub

        Try



            'If strCardToken = "" Then

            '    MessageBox.Show("There is no Card Token on file for this AutoPay Account." & vbCrLf & vbCrLf & "This record will have the Status changed to 'No Token'.", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            '    If objIPSUtil.UpdateAutoPayAccountData(objDR("AutoPay_ID"), objDR("AutoPay_Date"), Val(objDR("AutoPay_Amount") & ""), _
            '                                    objDR("Customer_AccountID") & "", objDR("LastName") & "", objDR("FirstName") & "", _
            '                                    objDR("BillAddr1") & "", objDR("BillCity") & "", objDR("BillState") & "", _
            '                                    objDR("BillZip") & "", "No Token") <> objDR("AutoPay_ID") Then
            '        blnWasError = True
            '    Else
            '        lvwDisplay.SelectedItems(lvwDisplay.SelectedItems.Count - 1).SubItems(1).Text = "No Token"
            '    End If

            'End If


            strTransResult = objIPSUtil.ProcessTempusTokenAuthSettle(lblAutoPayAccountID.Text, lblAutoPayAmount.Text, "", "", 1)

        Catch ex As Exception

            blnWasError = True
            strErrMessage = "* " & ex.Message

        End Try





        If blnWasError Then
            MessageBox.Show("There was a problem updating the AutoPay Account Data." & vbCrLf & vbCrLf & strErrMessage, _
                            "System Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else

            lvwDisplay.SelectedItems(lvwDisplay.SelectedItems.Count - 1).SubItems(1).Text = "Void"

            gbFilters.Enabled = True
            lvwDisplay.Enabled = True
            btnClose.Enabled = True
            btnSave.Visible = False
            btnCancel.Visible = False
            btnProcessTrans.Visible = False
            cmbRecordStatus.Enabled = False

        End If
    End Sub

    Private Sub Filter_Changed(sender As Object, e As EventArgs) Handles cmbBatchStatusFilter.SelectedIndexChanged, _
                        rb30Days.CheckedChanged, rb90Days.CheckedChanged, rb180Days.CheckedChanged, rbNone.CheckedChanged
        DisplayBatchData()
    End Sub

    Private Sub cmbBatchDisplayed_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBatchDisplayed.SelectedIndexChanged
        DisplayData()
        cmbBatchDisplayed.Focus()
    End Sub

    'Public Function GetAppParameter(ByVal ParameterName As String) As String
    '    Dim objConn As SqlConnection
    '    Dim objAdapter As SqlDataAdapter
    '    Dim objDS As New DataSet
    '    Dim objRows As DataRowCollection
    '    Dim strSQL As String


    '    Try

    '        strSQL = "SELECT * FROM [AutoPayParameters] WHERE ParameterName = '" & ParameterName & "'"

    '        objConn = New SqlConnection(My.Settings.BATCH_DB_CONN)
    '        objAdapter = New SqlDataAdapter(strSQL, objConn)
    '        objDS = New DataSet
    '        objAdapter.Fill(objDS, "SystemData")
    '        objRows = objDS.Tables("SystemData").Rows

    '        If objRows.Count = 0 Then

    '            GetAppParameter = ""

    '        Else

    '            GetAppParameter = objRows(0).Item("ParameterValue") & ""

    '        End If

    '        objRows = Nothing

    '        objDS.Dispose()
    '        objDS = Nothing

    '        objAdapter.Dispose()
    '        objAdapter = Nothing

    '        objConn.Dispose()
    '        objConn = Nothing

    '    Catch ex As Exception

    '        GetAppParameter = ""

    '        'WriteLog("frmProfiles LoadClients: Unable to Load Client Data", True)
    '        MsgBox("Unable to Get App Parameter Data. (" & ParameterName & ")" & vbCrLf & ex.Message)

    '    End Try

    'End Function

    'Public Function SetAppParameter(ByVal ParameterName As String, ByVal Value As String, ByVal CreateIfNotFound As Boolean) As Boolean
    '    Dim objConn As SqlConnection
    '    Dim objAdapter As SqlDataAdapter
    '    Dim objDS As New DataSet
    '    Dim objRows As DataRowCollection
    '    Dim strSQL As String

    '    'WriteLog("frmProfiles LoadClients", True)


    '    Try

    '        strSQL = "SELECT * FROM [Parameters] WHERE ParameterName = '" & ParameterName & "'"

    '        objConn = New SqlConnection(My.Settings.BATCH_DB_CONN)
    '        objAdapter = New SqlDataAdapter(strSQL, objConn)
    '        objDS = New DataSet
    '        objAdapter.Fill(objDS, "SystemData")
    '        objRows = objDS.Tables("SystemData").Rows

    '        If objRows.Count = 0 Then

    '            If CreateIfNotFound Then

    '                strSQL = "INSERT INTO [Parameters] (ParameterName, ParameterValue) VALUES ('" & ParameterName & "', '" & Value & "')"
    '                objAdapter = New SqlDataAdapter(strSQL, objConn)
    '                objDS = New DataSet
    '                objAdapter.Fill(objDS, "SystemData")

    '            End If

    '        Else

    '            strSQL = "UPDATE [Parameters] SET ParameterValue = '" & Value & "' WHERE ParameterName = '" & ParameterName & "'"
    '            objAdapter = New SqlDataAdapter(strSQL, objConn)
    '            objDS = New DataSet
    '            objAdapter.Fill(objDS, "SystemData")

    '        End If

    '        objRows = Nothing

    '        objDS.Dispose()
    '        objDS = Nothing

    '        objAdapter.Dispose()
    '        objAdapter = Nothing

    '        objConn.Dispose()
    '        objConn = Nothing

    '        SetAppParameter = True

    '    Catch ex As Exception

    '        SetAppParameter = False

    '        'WriteLog("frmProfiles LoadClients: Unable to Load Client Data", True)
    '        MsgBox("Unable to Set App Parameter Data. (" & ParameterName & ", " & Value & ")" & vbCrLf & ex.Message)

    '    End Try

    'End Function

    'Public Function AppendAppParameter(ByVal ParameterName As String, ByVal Value As String, ByVal CreateIfNotFound As Boolean) As Boolean
    '    Dim objConn As SqlConnection
    '    Dim objAdapter As SqlDataAdapter
    '    Dim objDS As New DataSet
    '    Dim objRows As DataRowCollection
    '    Dim strSQL As String
    '    Dim strExistingValue As String

    '    'WriteLog("frmProfiles LoadClients", True)


    '    Try

    '        strSQL = "SELECT * FROM [Parameters] WHERE ParameterName = '" & ParameterName & "'"

    '        objConn = New SqlConnection(My.Settings.BATCH_DB_CONN)
    '        objAdapter = New SqlDataAdapter(strSQL, objConn)
    '        objDS = New DataSet
    '        objAdapter.Fill(objDS, "SystemData")
    '        objRows = objDS.Tables("SystemData").Rows

    '        If objRows.Count = 0 Then

    '            If CreateIfNotFound Then

    '                strSQL = "INSERT INTO [Parameters] (ParameterName, ParameterValue) VALUES ('" & ParameterName & "', '" & Value & "')"
    '                objAdapter = New SqlDataAdapter(strSQL, objConn)
    '                objDS = New DataSet
    '                objAdapter.Fill(objDS, "SystemData")

    '            End If

    '        Else

    '            strExistingValue = objRows(0).Item("ParameterValue") & ""
    '            strSQL = "UPDATE [Parameters] SET ParameterValue = '" & strExistingValue & Value & "' WHERE ParameterName = '" & ParameterName & "'"
    '            objAdapter = New SqlDataAdapter(strSQL, objConn)
    '            objDS = New DataSet
    '            objAdapter.Fill(objDS, "SystemData")

    '        End If

    '        objRows = Nothing

    '        objDS.Dispose()
    '        objDS = Nothing

    '        objAdapter.Dispose()
    '        objAdapter = Nothing

    '        objConn.Dispose()
    '        objConn = Nothing

    '        AppendAppParameter = True

    '    Catch ex As Exception

    '        AppendAppParameter = False

    '        'WriteLog("frmProfiles LoadClients: Unable to Load Client Data", True)
    '        MsgBox("Unable to Append App Parameter Data. (" & ParameterName & ", " & Value & ")" & vbCrLf & ex.Message)

    '    End Try

    'End Function





End Class
