Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.ApplicationBlocks.Data
Imports Microsoft.VisualBasic



Public Class MerchantTransImport

    Private Sub ImportTransData_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Left = Val(GetSetting(Application.ProductName, Me.Name, "Left"))
        Top = Val(GetSetting(Application.ProductName, Me.Name, "Top"))
        txtFileName.Text = GetSetting(Application.ProductName, Me.Name, "ImportFilename").Trim
        If txtFileName.Text = "" Then txtFileName.Text = GetAppParameter("DEFAULT_IMPORT_PATH")
        lblImportDate.Text = Format(Now, "MM/dd/yyyy")
        lblRecords.Text = "0"
        lblDuplicates.Text = "0"
        lblAccepted.Text = "0"
        lblAcceptedAmounts.Text = "0"
        lblRejected.Text = "0"
        lblRejectedAmounts.Text = "0"
        lblCredited.Text = "0"
        lblCreditedAmounts.Text = "0"
        lblVoided.Text = "0"
    End Sub

    Private Sub ImportTransData_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        SaveSetting(Application.ProductName, Me.Name, "Top", Top)
        SaveSetting(Application.ProductName, Me.Name, "Left", Left)
        SaveSetting(Application.ProductName, Me.Name, "ImportFilename", txtFileName.Text.Trim)
    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Dim OpenFileDialog As New OpenFileDialog
        Dim FileName As String = OpenFileDialog.FileName

        OpenFileDialog.Filter = "Comma Separated Values (*.csv)|*.csv|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
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
        Dim strLine As String
        Dim objSR As StreamReader = Nothing
        Dim lngBatchID As Long


        If Not ValidFile(txtFileName.Text) Then
            MessageBox.Show("Data File does not appear to be Valid, select a Valid File.", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            Exit Sub
        End If

        If MessageBox.Show("You are about to Import Transaction Data from" & vbCrLf & vbCrLf & _
                    txtFileName.Text & vbCrLf & vbCrLf & "Are you sure?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub

        txtFileName.Enabled = False
        btnBrowse.Enabled = False
        btnImport.Enabled = False
        btnClose.Enabled = False

        lblImportDate.Text = Format(Now, "MM/dd/yyyy")
        lblRecords.Text = "0"
        lblDuplicates.Text = "0"
        lblAccepted.Text = "0"
        lblAcceptedAmounts.Text = "0"
        lblRejected.Text = "0"
        lblRejectedAmounts.Text = "0"
        lblCredited.Text = "0"
        lblCreditedAmounts.Text = "0"
        lblVoided.Text = "0"

        lngBatchID = GetNewImportBatchID(txtFileName.Text, lblImportDate.Text)
        'lngBatchID = 0

        If lngBatchID = -1 Then
            MessageBox.Show("Unable to create new Import Batch Record.", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            txtFileName.Enabled = True
            btnBrowse.Enabled = True
            btnImport.Enabled = True
            btnClose.Enabled = True
            Exit Sub
        End If

        gbBatch.Text = "Batch ID: " & lngBatchID

        Try

            objSR = New StreamReader(txtFileName.Text)

            strLine = objSR.ReadLine() ' Go past Header Record

            Do While (strLine Is Nothing) = False

                CustomDoEvents(2000)

                strLine = objSR.ReadLine()

                If strLine Is Nothing Then Exit Do

                CustomDoEvents(2000)

                If Not InsertMerchantTrans(lngBatchID, strLine) Then
                    objSR.Close()
                    MessageBox.Show("An error was encountered while writing a Merchant Transaction record.", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                    txtFileName.Enabled = True
                    btnBrowse.Enabled = True
                    btnImport.Enabled = True
                    btnClose.Enabled = True
                    Exit Sub
                End If

                CustomDoEvents(2000)

            Loop

            objSR.Close()

        Catch SE As Exception

            MessageBox.Show("An error was encountered while Importing File." & vbCrLf & vbCrLf & SE.Message, "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            objSR.Close()

        End Try

        If Not UpdateImportBatch(lngBatchID, txtFileName.Text, lblImportDate.Text, Val(lblAccepted.Text), Val(lblAcceptedAmounts.Text), Val(lblRejected.Text), Val(lblRejectedAmounts.Text), Val(lblCredited.Text), Val(lblCreditedAmounts.Text), Val(lblVoided.Text), Val(lblDuplicates.Text), Val(lblRecords.Text)) Then
            MessageBox.Show("An error was encountered while Updating the Batch Record.", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
        End If

        txtFileName.Enabled = True
        btnBrowse.Enabled = True
        btnImport.Enabled = True
        btnClose.Enabled = True

    End Sub

    Private Function ValidFile(ByVal Filename As String) As Boolean
        Dim strLine As String
        Dim arrElements As Object

        Try

            Using objSR As StreamReader = New StreamReader(Filename)

                strLine = objSR.ReadLine()
                arrElements = Split(strLine, ",")

                If arrElements(0) = "CyberSource Merchant ID" And arrElements(2) = "Date and Time" And arrElements(3) = "Request ID" And arrElements(4) = "Merchant Reference Number" Then

                    strLine = objSR.ReadLine()
                    arrElements = Split(strLine, ",")

                    If arrElements(0) = "nw_nat" Then
                        objSR.Close()
                        Return True
                    Else
                        objSR.Close()
                        Return False
                    End If

                Else
                    objSR.Close()
                    Return False
                End If

            End Using
        Catch E As Exception

            Return False

        End Try

    End Function

    Private Function GetNewImportBatchID(ByVal FileName As String, ByVal ImportDate As Date) As Long
        Dim objDR As SqlDataReader
        Dim paramArrUpdate() = {FileName, ImportDate}


        Try

            objDR = SqlHelper.ExecuteReader(DB_CONNECTION, "usp_ImportBatches_Put", paramArrUpdate)

            If objDR.Read() Then

                If Not objDR.IsDBNull(0) Then
                    GetNewImportBatchID = objDR.GetValue(0)
                    objDR.Close()
                Else
                    objDR.Close()
                    Return -1
                End If

            End If


        Catch ex As Exception

            Return -1

        End Try

    End Function

    Private Function UpdateImportBatch(ByVal ImportBatchID As Long, ByVal FileName As String, ByVal ImportDate As DateTime, ByVal Accepts As Long, ByVal AcceptAmounts As Decimal, ByVal Rejects As Long, ByVal RejectsAmounts As Decimal, ByVal Credits As Long, ByVal CreditsAmounts As Decimal, ByVal Voids As Long, ByVal Duplicates As Long, ByVal Total As Long) As Boolean
        Dim objDR As SqlDataReader
        Dim paramArrUpdate() = {ImportBatchID, FileName, ImportDate, Accepts, AcceptAmounts, Rejects, RejectsAmounts, Credits, CreditsAmounts, Voids, Duplicates, Total}

        Try

            objDR = SqlHelper.ExecuteReader(DB_CONNECTION, "usp_ImportBatches_Update", paramArrUpdate)

        Catch ex As Exception

            Return False

        End Try

        Return True

    End Function

    Private Function InsertMerchantTrans(ByVal BatchID As Long, ByVal RecordData As String) As Boolean
        Dim objDR As SqlDataReader
        Dim arrElements As Object
        Dim dtDateTime As Date
        Dim strRequestID As String = ""
        Dim strCustomerAccountID As String = ""
        Dim strLastName As String = ""
        Dim strFirstName As String = ""
        Dim decAmount As Decimal
        Dim decClientAmount As Decimal
        Dim strApplication As String = ""
        Dim blnTypeIsVoid As Boolean = False
        Dim blnTypeIsCredit As Boolean = False
        Dim blnTypeIsCreditNoFee As Boolean = False
        Dim blnTypeIsDebit As Boolean = False
        Dim blnTypeIsReject As Boolean = False
        Dim blnTypeIsChargeback As Boolean = False
        Dim blnVoid As Boolean = False
        Dim lngVoidReferenceID As Long = 0
        Dim dtReconciled As Object = DBNull.Value
        Dim lngReconBatchID As Long = 0
        Dim blnPossibleErrors As Boolean = False

        ' OLD VERSION: (TXT)
        '0  Merchant ID,
        '1  Date and Time,
        '2  Request ID,
        '3  Merchant Reference Number,
        '4  Last Name,
        '5  First Name,
        '6  Email Address,
        '7  Amount,
        '8  Currency,
        '9  Account Suffix,
        '10 Applications

        ' NEW VERSION: (CSV)
        '0  CyberSource Merchant ID,
        '1  Client User,
        '2  Date and Time,
        '3  Request ID,
        '4  Merchant Reference Number,
        '5  Last Name,
        '5  First Name,
        '7  Email Address,
        '8  Amount,
        '9  Currency,
        '10 Account Suffix,
        '11 Applications

        ' NEW VERSION: (CSV) -  2009/08/10
        '0  CyberSource Merchant ID,
        '1  Client User,
        '2  Date and Time,
        '3  Request ID,
        '4  Merchant Reference Number,
        '5  Last Name,
        '5  First Name,
        '7  Email Address,
        '8  Amount,
        '9  Currency,
        '10 Account Suffix,
        '11 Applications,
        '12 Card Expiration Month,
        '13 Card Expiration Year,
        '14 Account Type,
        '15 Check Routing Number,
        '16 Transaction Reference Number

        Try

            arrElements = Split(RecordData, ",")

            dtDateTime = Convert.ToDateTime(arrElements(2) & "")
            strRequestID = Trim(arrElements(3) & "")
            strCustomerAccountID = Trim(arrElements(4) & "")

            ' Normal RequestID is 22 in length
            '         1        10        20
            '         ======================
            'Sample:  1845078363570176177166
            '
            If strRequestID.Contains("+") Or strRequestID.Length < 16 Then blnPossibleErrors = True


            Select Case UBound(arrElements)

                Case 16 ' 11
                    ' "Credit Card Authorization(Accept)"
                    strLastName = StrConv(Trim(arrElements(5) & ""), VbStrConv.ProperCase)
                    strFirstName = StrConv(Trim(arrElements(6) & ""), VbStrConv.ProperCase)
                    strApplication = Trim(arrElements(11) & "")
                    If Val(arrElements(8) & "") > 0 Then
                        decAmount = Convert.ToDecimal(arrElements(8) & "")
                        ' Removed 11/5/2012
                        'decClientAmount = decAmount - 3.95
                        decClientAmount = decAmount
                    Else
                        decAmount = 0.0
                        decClientAmount = 0
                    End If

                Case 17 ' 12

                    strLastName = StrConv(Trim(arrElements(5) & ""), VbStrConv.ProperCase)
                    strFirstName = StrConv(Trim(arrElements(6) & ""), VbStrConv.ProperCase)
                    strApplication = Trim(arrElements(11) & "")
                    strApplication = strApplication & "," & Trim(arrElements(12) & "")
                    If Val(arrElements(8) & "") > 0 Then
                        decAmount = Convert.ToDecimal(arrElements(8) & "")
                        ' Removed 11/5/2012
                        'decClientAmount = decAmount - 3.95
                        decClientAmount = decAmount
                    Else
                        decAmount = 0.0
                        decClientAmount = 0
                    End If

                Case 18 ' 13

                    strLastName = StrConv(Trim(arrElements(5) & arrElements(6) & ""), VbStrConv.ProperCase)
                    strFirstName = StrConv(Trim(arrElements(7) & ""), VbStrConv.ProperCase)
                    strApplication = Trim(arrElements(12) & "")
                    strApplication = strApplication & "," & Trim(arrElements(13) & "")
                    If Val(arrElements(9) & "") > 0 Then
                        decAmount = Convert.ToDecimal(arrElements(9) & "")
                        ' Removed 11/5/2012
                        'decClientAmount = decAmount - 3.95
                        decClientAmount = decAmount
                    Else
                        decAmount = 0.0
                        decClientAmount = 0
                    End If

                Case 19 ' 14

                    strLastName = StrConv(Trim(arrElements(5) & arrElements(6) & ""), VbStrConv.ProperCase)
                    strFirstName = StrConv(Trim(arrElements(7) & arrElements(8) & ""), VbStrConv.ProperCase)
                    strApplication = Trim(arrElements(13) & "")
                    strApplication = strApplication & "," & Trim(arrElements(14) & "")
                    If Val(arrElements(10) & "") > 0 Then
                        decAmount = Convert.ToDecimal(arrElements(10) & "")
                        ' Removed 11/5/2012
                        'decClientAmount = decAmount - 3.95
                        decClientAmount = decAmount
                    Else
                        decAmount = 0.0
                        decClientAmount = 0
                    End If

            End Select

            strApplication = strApplication.Replace(Chr(34), "")


        Catch ex As Exception

            Exit Function

        End Try


        lblRecords.Text = Val(lblRecords.Text) + 1

        If MerchantTransExists(dtDateTime, strRequestID, strCustomerAccountID) Then

            lblDuplicates.Text = Val(lblDuplicates.Text) + 1

        Else

            Select Case strApplication

                Case "Credit Card Authorization(Accept),Credit Card Settlement(Accept)", "Credit Card Settlement(Accept),Credit Card Authorization(Accept)"
                    blnTypeIsDebit = True
                    lblAccepted.Text = Val(lblAccepted.Text) + 1
                    lblAcceptedAmounts.Text = Format(Val(lblAcceptedAmounts.Text) + decAmount, "0.00")

                Case "Credit Card Settlement(Ignore),Credit Card Authorization(Reject)", "Credit Card Credit(Reject)", "Credit Card Authorization(Reject),Credit Card Settlement(Ignore)"
                    blnTypeIsReject = True
                    lblRejected.Text = Val(lblRejected.Text) + 1
                    lblRejectedAmounts.Text = Format(Val(lblRejectedAmounts.Text) + decAmount, "0.00")

                Case "Credit Card Settlement(Ignore),Credit Card Authorization(Error)"
                    blnTypeIsReject = True
                    lblRejected.Text = Val(lblRejected.Text) + 1
                    lblRejectedAmounts.Text = Format(Val(lblRejectedAmounts.Text) + decAmount, "0.00")

                Case "Credit Card Credit(Accept)"
                    blnTypeIsCredit = True
                    lblCredited.Text = Val(lblCredited.Text) + 1
                    lblCreditedAmounts.Text = Format(Val(lblCreditedAmounts.Text) + decAmount, "0.00")

                Case "Voided Transactions(Accept)"
                    blnTypeIsVoid = True
                    lblVoided.Text = Val(lblVoided.Text) + 1

                Case "Credit Card Authorization(Ignore),Credit Card Settlement(Ignore)"
                    blnTypeIsReject = True
                    lblRejected.Text = Val(lblRejected.Text) + 1
                    lblRejectedAmounts.Text = Format(Val(lblRejectedAmounts.Text) + decAmount, "0.00")

                Case "Credit Card Authorization Reversal(Reject)"

                Case Else
                    MessageBox.Show("Unknown Type: " & strApplication)

            End Select

            Try

                Dim paramArrUpdate() = {BatchID, dtDateTime, strRequestID, strCustomerAccountID, strLastName, strFirstName, decAmount, decClientAmount, strApplication, blnTypeIsVoid, blnTypeIsCredit, blnTypeIsCreditNoFee, blnTypeIsDebit, blnTypeIsReject, blnTypeIsChargeback, blnVoid, lngVoidReferenceID, dtReconciled, lngReconBatchID}
                CustomDoEvents(2000)
                objDR = SqlHelper.ExecuteReader(DB_CONNECTION, "usp_MerchantTrans_Put_New", paramArrUpdate)
                CustomDoEvents(2000)

                If objDR.Read() Then

                    If blnPossibleErrors Then
                        MessageBox.Show("Transaction(s) were Imported with a malformed RequestID." & vbCrLf & vbCrLf & "See System Administrator with this informaiton.", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                    If objDR.IsDBNull(0) Then
                        objDR.Close()
                        Return False
                    End If

                Else
                    If blnPossibleErrors Then
                        MessageBox.Show("Transaction(s) were Imported with a malformed RequestID." & vbCrLf & vbCrLf & "See System Administrator with this informaiton.", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                    objDR.Close()
                    Return False
                End If

                objDR.Close()

            Catch ex As Exception
                Return False
            End Try

        End If

        Return True

    End Function

    Private Function MerchantTransExists(ByVal DateTimeValue As DateTime, ByVal RequestID As String, ByVal CustomerAccountID As String) As Boolean
        Dim objDR As SqlDataReader

        Try

            Dim paramArrUpdate() = {DateTimeValue, RequestID, CustomerAccountID}
            CustomDoEvents(2000)
            objDR = SqlHelper.ExecuteReader(DB_CONNECTION, "usp_MerchantTrans_Get_New", paramArrUpdate)
            CustomDoEvents(2000)

            If objDR.Read() Then

                If objDR.IsDBNull(0) Then
                    objDR.Close()
                    Return False
                End If

            Else
                objDR.Close()
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try

        objDR.Close()
        Return True

    End Function

End Class