Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.ApplicationBlocks.Data
Imports Microsoft.VisualBasic



Public Class TransDataImport

    Private Const DB_CONNECTION As String = "Server=MTK2;Database=NWTrans;User ID=RptLogin;Password=xray13;Trusted_Connection=False"
    'Private Const DB_CONNECTION As String = "Server=MTCLARK;Database=NWTrans;User ID=RptLogin;Password=xray13;Trusted_Connection=False"


    Private Sub ImportTransData_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Left = Val(GetSetting(Application.ProductName, Me.Name, "Left"))
        Top = Val(GetSetting(Application.ProductName, Me.Name, "Top"))
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
    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Dim OpenFileDialog As New OpenFileDialog
        Dim FileName As String = OpenFileDialog.FileName

        OpenFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        OpenFileDialog.Multiselect = False
        OpenFileDialog.FileName = txtFileName.Text
        If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            txtFileName.Text = OpenFileDialog.FileName
        End If

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImport.Click
        Dim strLine As String
        Dim objSR As StreamReader = Nothing
        Dim lngBatchID As Long


        If Not ValidFile(txtFileName.Text) Then Exit Sub

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

        If lngBatchID = -1 Then
            MessageBox.Show("Unable to create new Import Batch Record.", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            Exit Sub
            txtFileName.Enabled = True
            btnBrowse.Enabled = True
            btnImport.Enabled = True
            btnClose.Enabled = True
        End If

        gbBatch.Text = "Batch ID: " & lngBatchID

        Try

            objSR = New StreamReader(txtFileName.Text)

            strLine = objSR.ReadLine() ' Go past Header Record

            Do While (strLine Is Nothing) = False

                CustomDoEvents(100)

                strLine = objSR.ReadLine()

                If strLine Is Nothing Then Exit Do

                CustomDoEvents(100)

                If Not InsertMerchantTrans(lngBatchID, strLine) Then
                    objSR.Close()
                    MessageBox.Show("An error was encountered while writing a Merchant Transaction record.", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                    txtFileName.Enabled = True
                    btnBrowse.Enabled = True
                    btnImport.Enabled = True
                    btnClose.Enabled = True
                    Exit Sub
                End If

                CustomDoEvents(100)

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

                If arrElements(0) = "Merchant ID" And arrElements(1) = "Date and Time" And arrElements(2) = "Request ID" And arrElements(3) = "Merchant Reference Number" Then

                    strLine = objSR.ReadLine()
                    arrElements = Split(strLine, ",")

                    If arrElements(0) = "nw_nat" Then
                        objSR.Close()
                        Return True
                    Else
                        objSR.Close()
                        MessageBox.Show("Data File does not appear to be Valid, select a Valid File.", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                        Return False
                    End If

                Else
                    objSR.Close()
                    MessageBox.Show("Data File does not appear to be Valid, select a Valid File.", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                    Return False
                End If

            End Using
        Catch E As Exception

            MessageBox.Show("Data File does not appear to be Valid, select a Valid File.", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Hand)
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
                Else
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
        Dim strRequestID As String
        Dim strCustomerAccountID As String
        Dim strLastName As String
        Dim strFirstName As String
        Dim decAmount As Decimal
        Dim decClientAmount As Decimal
        Dim strApplication As String
        Dim blnTypeIsVoid As Boolean = False
        Dim blnTypeIsCredit As Boolean = False
        Dim blnTypeIsDebit As Boolean = False
        Dim blnTypeIsReject As Boolean = False
        Dim blnVoid As Boolean = False
        Dim lngVoidReferenceID As Long = 0
        Dim dtReconciled As Object = DBNull.Value
        Dim lngReconBatchID As Long = 0


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

        Try

            arrElements = Split(RecordData, ",")

            dtDateTime = Convert.ToDateTime(arrElements(1) & "")
            strRequestID = Trim(arrElements(2) & "")
            strCustomerAccountID = Trim(arrElements(3) & "")
            strLastName = StrConv(Trim(arrElements(4) & ""), VbStrConv.ProperCase)
            strFirstName = StrConv(Trim(arrElements(5) & ""), VbStrConv.ProperCase)
            strApplication = Trim(arrElements(10) & "")
            If UBound(arrElements) > 10 Then
                strApplication = strApplication & "," & Trim(arrElements(11) & "")
            End If
            strApplication = strApplication.Replace(Chr(34), "")
            If Val(arrElements(7) & "") > 0 Then
                decAmount = Convert.ToDecimal(arrElements(7) & "")
                decClientAmount = decAmount - 3.95
            Else
                decAmount = 0.0
                decClientAmount = 0
            End If

        Catch ex As Exception

            Exit Function

        End Try

        lblRecords.Text = Val(lblRecords.Text) + 1

        If MerchantTransExists(dtDateTime, strRequestID, strCustomerAccountID) Then

            lblDuplicates.Text = Val(lblDuplicates.Text) + 1

        Else

            Select Case strApplication

                Case "Credit Card Authorization(Accept),Credit Card Settlement(Accept)"
                    blnTypeIsDebit = True
                    lblAccepted.Text = Val(lblAccepted.Text) + 1
                    lblAcceptedAmounts.Text = Format(Val(lblAcceptedAmounts.Text) + decAmount, "0.00")

                Case "Credit Card Settlement(Ignore),Credit Card Authorization(Reject)"
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

                Case Else
                    MessageBox.Show("Unknown Type: " & strApplication)
                    Stop

            End Select
            Try

                Dim paramArrUpdate() = {BatchID, dtDateTime, strRequestID, strCustomerAccountID, strLastName, strFirstName, decAmount, decClientAmount, strApplication, blnTypeIsVoid, blnTypeIsCredit, blnTypeIsDebit, blnTypeIsReject, blnVoid, lngVoidReferenceID, dtReconciled, lngReconBatchID}
                CustomDoEvents(100)
                objDR = SqlHelper.ExecuteReader(DB_CONNECTION, "usp_MerchantTrans_Put", paramArrUpdate)
                CustomDoEvents(100)

                If objDR.Read() Then
                    If objDR.IsDBNull(0) Then Return False
                Else
                    Return False
                End If

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
            CustomDoEvents(100)
            objDR = SqlHelper.ExecuteReader(DB_CONNECTION, "usp_MerchantTrans_Get", paramArrUpdate)
            CustomDoEvents(100)

            If objDR.Read() Then
                If objDR.IsDBNull(0) Then Return False
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Private Sub CustomDoEvents(ByVal NumberOfTimes As Long)
        Dim lngCounter As Long
        For lngCounter = 1 To NumberOfTimes
            Application.DoEvents()
        Next
    End Sub

End Class