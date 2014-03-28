Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.VisualBasic
Imports System.Windows.Forms
Imports System.Xml



<ComClass(Library.ClassId, Library.InterfaceId, Library.EventsId)> _
Public Class Library

#Region "COM GUIDs"
    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.
    Public Const ClassId As String = "1517070f-5a71-4b33-bd12-1ee103530211"
    Public Const InterfaceId As String = "19a6970b-d279-4a2c-b8aa-b61eaa5c3c9d"
    Public Const EventsId As String = "8150e596-fc43-4f11-b4d8-c16f5ffe1e0e"
#End Region

#Region "COM SubNew"
    ' A creatable COM class must have a Public Sub New() with no parameters, otherwise, the class will not be 
    ' registered in the COM registry and cannot be created via CreateObject.
    Public Sub New()
        MyBase.New()
    End Sub
#End Region

    Public Event ImportingStats(ByVal BatchID As Long, ByVal ImportedCount As Long, ByVal ImportedSum As Decimal, _
                             ByVal AcceptedCount As Long, ByVal AcceptedSum As Decimal, _
                             ByVal DuplicatesCount As Long, ByVal DuplicatesSum As Decimal, ByVal InvalidCount As Long)



    Public Function ImportAutoPayFile(ByVal ImportFile As String, ByRef ImportStatus As String, Optional ByVal ImportedDate As Date = #1/1/1900#) As Long
        Dim objSR As StreamReader = Nothing
        Dim lngBatchID As Long
        Dim lngUpdatedBatchID As Long
        Dim strLine As String = ""
        Dim strFileStatus As String = "Active"
        Dim lngCounter As Long = 0

        Dim lngImportedCount As Long
        Dim decImportedSum As Decimal
        Dim lngAcceptedCount As Long
        Dim decAcceptedSum As Decimal
        Dim lngDuplicateCount As Long
        Dim decDuplicateSum As Decimal
        Dim lngInvalidCount As Long

        Dim strAutoPay_AccountID As String
        Dim dtAutoPay_Date As Date
        Dim decAutoPay_Amount As Decimal
        Dim strLastName As String
        Dim strFirstName As String
        Dim strBillAddr1 As String
        Dim strBillCity As String
        Dim strBillState As String
        Dim strBillZip As String
        Dim blnDataError As Boolean
        Dim strAccountStatus As String


        ImportFile = ImportFile.Trim

        If ImportedDate = #1/1/1900# Then ImportedDate = Today

        If Not ValidFile(ImportFile) Then
            MessageBox.Show("Data File does not appear to be Valid, select a Valid File.", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            ImportStatus = "Invalid File"
            Return -1
        End If

        If MessageBox.Show("You are about to Import AutoPay Data from:" & vbCrLf & vbCrLf & _
                    ImportFile.Trim & vbCrLf & vbCrLf & "Are you sure?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Return 0


        lngBatchID = NextAutoPayImportBatchID(ImportFile, ImportedDate)


        If lngBatchID = -1 Then

            MessageBox.Show("Unable to create Next AutoPay Batch Record.", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            ImportStatus = "Unable to Create Import BatchID"
            Return -1

        End If

        Try

            objSR = New StreamReader(ImportFile)

            Do While (strLine Is Nothing) = False

                blnDataError = False

                strLine = objSR.ReadLine()

                If strLine Is Nothing Then Exit Do

                If strLine.Length <> 78 Then blnDataError = True
                If Not blnDataError Then If Not (strLine.Substring(0, 16) Like "################") Then blnDataError = True
                If Not blnDataError Then If Not (strLine.Substring(65, 13) Like "#############") Then blnDataError = True
                If Not blnDataError Then If Not Val(strLine.Substring(65, 13)) > 0 Then blnDataError = True
                If Not blnDataError Then If Not IsDate(strLine.Substring(12, 2) & "-" & strLine.Substring(14, 2) & "-" & strLine.Substring(8, 4)) Then blnDataError = True

                If Not blnDataError Then

                    ' No Errors detected in data.
                    ' Store values to variables.
                    strAutoPay_AccountID = strLine.Substring(0, 8)
                    dtAutoPay_Date = CDate(strLine.Substring(12, 2) & "-" & strLine.Substring(14, 2) & "-" & strLine.Substring(8, 4))
                    decAutoPay_Amount = CDec(strLine.Substring(65, 13)) / 100
                    strLastName = strLine.Substring(16, 30).Trim
                    strFirstName = strLine.Substring(46, 19).Trim
                    strBillAddr1 = ""
                    strBillCity = ""
                    strBillState = ""
                    strBillZip = ""
                    strAccountStatus = "Active"

                    lngImportedCount = lngImportedCount + 1
                    decImportedSum = decImportedSum + decAutoPay_Amount



                    ' Check to see if a Duplicate record exists (regardless of Batch)
                    ' A Duplicate is a record with a matching Pay Date, Amount, and AccountID
                    If DupeAutoPayAccountDataExists(dtAutoPay_Date, decAutoPay_Amount, strAutoPay_AccountID) Then

                        ' Change Status
                        strFileStatus = "Import Error"
                        strAccountStatus = "Duplicate"
                        lngDuplicateCount = lngDuplicateCount + 1
                        decDuplicateSum = decDuplicateSum + decAutoPay_Amount

                    Else

                        lngAcceptedCount = lngAcceptedCount + 1
                        decAcceptedSum = decAcceptedSum + decAutoPay_Amount

                    End If

                Else

                    ' Set values to indicate the record is a Invalid.
                    strFileStatus = "Import Error"
                    strAutoPay_AccountID = "Record Invalid"
                    dtAutoPay_Date = #1/1/1900#
                    decAutoPay_Amount = 0.0
                    strLastName = "Record Invalid"
                    strFirstName = "Record Invalid"
                    strBillAddr1 = ""
                    strBillCity = ""
                    strBillState = ""
                    strBillZip = ""
                    strAccountStatus = "Invalid"

                    lngInvalidCount = lngInvalidCount + 1

                End If

                lngCounter = lngCounter + 1

                If lngCounter = 1 Then

                    lngCounter = 0
                    RaiseEvent ImportingStats(lngBatchID, lngImportedCount, decImportedSum, lngAcceptedCount, decAcceptedSum, lngDuplicateCount, decDuplicateSum, lngInvalidCount)

                End If


                ' Store values
                If InsertAutoPayAccountData(lngBatchID, dtAutoPay_Date, decAutoPay_Amount, strAutoPay_AccountID, strFirstName, strLastName, strBillAddr1, strBillCity, strBillState, strBillZip, strAccountStatus) = -1 Then

                    strFileStatus = "Import Error(s)"
                    ImportStatus = "Import Error(s)"

                End If

            Loop

            objSR.Close()

        Catch SE As Exception

            MessageBox.Show("An error was encountered while Importing File." & vbCrLf & vbCrLf & SE.Message, "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            objSR.Close()
            strFileStatus = "Import Error"
            ImportStatus = "Import Exception"

        End Try

        lngUpdatedBatchID = UpdateAutoPayImportBatch(lngBatchID, lngImportedCount, decImportedSum, lngAcceptedCount, decAcceptedSum, lngDuplicateCount, decDuplicateSum, lngInvalidCount, 0, 0, 0, 0, strFileStatus)

        If lngUpdatedBatchID = -1 Then

            MessageBox.Show("An error was encountered while Updating the Batch Record.", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            strFileStatus = "Import Error"
            ImportStatus = "Import Error(s)"

        End If

        ImportStatus = IIf(lngDuplicateCount > 0, "Duplicate Records", "")
        ImportStatus = ImportStatus & IIf(lngInvalidCount > 0, IIf(ImportStatus <> "", ", ", "") & "Invalid Records", "")

        Return lngBatchID

    End Function

    Public Function ProcessAutoPayBatch(ByVal AutoPay_BatchID As Long) As Long


    End Function

    Public Function ProcessTempusTokenAuthSettle(ByVal AutoPay_ID As Long, ByVal Amount As Decimal, ByVal FirstName As String, ByVal LastName As String, ByVal ReferenceID As Long) As String
        'Dim wrapperServiceClient As New WrapperService.WrapperServiceClient()
        'Dim wrapperResult As WrapperService.WrapperResult
        Dim objRow As DataRow
        Dim strToken As String = ""

        '====================================================================
        ' Steps required for processing a transaction:
        '====================================================================
        ' 1 ) Get AutoPay Account Data
        ' 2 ) Get Card Token from Customer_AccountID
        ' 3 ) Get Transaction ID from IPSToolsWS Web Service
        ' 4 ) Pass Customer Data to Tempus WrapperService for processing
        ' 5 ) Update Transcation Record with results
        ' 6 ) Pass data to SP CreateTempusCCAuthTransaction



        '====================================================================
        ' 1 ) Get AutoPay Account Record
        '====================================================================
        Try

            objRow = GetAutoPayAccountData(AutoPay_ID)
            If objRow Is Nothing Then Throw New System.Exception("Invalid AutoPay_ID (" & AutoPay_ID & "), no valid Customer Data returned")

        Catch ex As Exception
            Throw New System.Exception("Exception calling GetAutoPayAccountData (" & AutoPay_ID & ") - " & ex.Message)
        End Try



        '====================================================================
        ' 2 ) Get Card Token from Customer_AccountID
        '====================================================================
        Try

            strToken = GetCustomerCardToken(objRow("Customer_AccountID") & "")

            If strToken = "" Then

                UpdateAutoPayAccountData(objRow("AutoPay_ID"), objRow("AutoPay_Date"), Val(objRow("AutoPay_Amount") & ""), objRow("Customer_AccountID") & "", objRow("LastName") & "", _
                                         objRow("FirstName") & "", objRow("BillAddr1") & "", objRow("BillCity") & "", objRow("BillState") & "", objRow("BillZip") & "", "No Token")

                If objRow Is Nothing Then Throw New System.Exception("No Card Token for AutoPay_RecordID, no valid Customer Data returned." & _
                                                                    vbCrLf & vbCrLf & "Record Status has been changed to 'No Token'")

            End If
        Catch ex As Exception
            Throw New System.Exception("Exception calling GetCustomerCardToken (" & objRow("Customer_AccountID") & "" & ") - " & ex.Message)
        End Try



        '====================================================================
        ' 3 ) Get Transaction ID from IPSToolsWS Web Service
        '====================================================================
        Dim objWS As New IPSTools.ToolsSoapClient
        Dim lngTransID As Long = -1
        Dim strLast4 As String = ""
        Dim strAccountType As String = ""
        Dim strMerchantID As String = ""
        Dim strMerchantDecision As String = ""
        Dim strMerchantReasonCode As String = ""
        Dim strMerchantAuthCode As String = ""
        Dim strMerchantRequestID As String = ""
        Dim dtReconDateTime As Date = CDate("1900-01-01 00:00")

        Try
            lngTransID = objWS.LogCreditCardTransaction2(My.Settings.CARDTRANS_TABLE, 0, 0, 0, Now, 190, objRow("Customer_AccountID") & "", objRow("FirstName") & "", objRow("LastName") & "", _
                                                         objRow("BillAddr1") & "", "", objRow("BillCity") & "", objRow("BillState") & "", objRow("BillZip") & "", strLast4, strAccountType, _
                                                          0.0, Amount, Amount, True, False, False, False, False, False, False, strToken, strMerchantID, objRow("Customer_AccountID") & "", strMerchantRequestID, _
                                                           strMerchantDecision, strMerchantReasonCode, strMerchantAuthCode, "", objRow("AutoPay_BatchID"), 0, dtReconDateTime, True, objRow("AutoPay_ID"), _
                                                            "", False, "NW AutoPay", "", "", "", "")

            If lngTransID = -1 Then Throw New System.Exception("Unable to Log Card Transaction to Web Service. [LogCreditCardTransaction2]" )

        Catch ex As Exception
            Throw New System.Exception("Exception Logging Card Transaction to Web Service (" & objRow("Customer_AccountID") & "" & ") - " & ex.Message)
        End Try



        '====================================================================
        ' 4 ) Pass Customer Data to Tempus WrapperService for processing
        '====================================================================

        Try
            'wrapperResult = wrapperServiceClient.TestService(5, "AtiCCAuth", String.Format("<CCAMT>{0}</CCAMT>  <REPTOKEN>{1}</REPTOKEN>", Amount, strToken))

            'Dim response As New AutoPayEnrollCreditCardResponse()
            'Dim response As New WrapperService.WrapperResult

            'If wrapperResult.ResultStatus.Equals(WrapperService.WrapperResult.StatusEnum.Success) Then

            '    Dim doc As XDocument = XDocument.Parse(wrapperResult.Result.ToString())

            '    response.AccountNumber = doc.Root.Elements("AccountNumber").FirstOrDefault().Value
            '    response.Confirmation = Long.Parse(doc.Root.Elements("Confirmation").FirstOrDefault().Value)
            '    response.Response = DirectCast([Enum].Parse(GetType(AutoPayServiceResponse), doc.Root.Elements("Response").FirstOrDefault().Value), AutoPayServiceResponse)

            '    Dim strAccountNumber As String = doc.Root.Elements("AccountNumber").FirstOrDefault().Value
            '    Dim lngConfirmation As Long = Long.Parse(doc.Root.Elements("Confirmation").FirstOrDefault().Value)
            '    Dim response.Response = DirectCast([Enum].Parse(GetType(AutoPayServiceResponse), doc.Root.Elements("Response").FirstOrDefault().Value), AutoPayServiceResponse)

            'Else

            '    response.AccountNumber = model.AccountNumber
            '    response.Response = AutoPayServiceResponse.[Error]

            'End If

        Catch ex As Exception

        End Try




        '====================================================================
        ' 5 ) Update Transcation Record with results
        '====================================================================
        objWS = New IPSTools.ToolsSoapClient
        strLast4 = ""
        strAccountType = ""
        strMerchantID = ""
        strMerchantDecision = ""
        strMerchantReasonCode = ""
        strMerchantAuthCode = ""
        strMerchantRequestID = ""

        Try
            lngTransID = objWS.UpdateCreditCardTransaction(My.Settings.CARDTRANS_TABLE, lngTransID, 0, 0, 0, Now, 190, objRow("Customer_AccountID") & "", objRow("FirstName") & "", objRow("LastName") & "", _
                                                         objRow("BillAddr1") & "", "", objRow("BillCity") & "", objRow("BillState") & "", objRow("BillZip") & "", strLast4, strAccountType, _
                                                           0.0, Amount, Amount, True, False, False, False, False, False, False, strToken, strMerchantID, objRow("Customer_AccountID") & "", strMerchantRequestID, _
                                                           strMerchantDecision, strMerchantReasonCode, strMerchantAuthCode, "", objRow("AutoPay_BatchID"), 0, dtReconDateTime, True, objRow("AutoPay_ID"), _
                                                            "", False, "NW AutoPay", "", "", "", "")

            If lngTransID = -1 Then Throw New System.Exception("Unable to Update Card Transaction to Web Service. [UpdateCreditCardTransaction]")

        Catch ex As Exception
            Throw New System.Exception("Exception Updating Card Transaction to Web Service (" & objRow("Customer_AccountID") & "" & ") - " & ex.Message)
        End Try



        '====================================================================
        ' 6 ) Pass data to SP CreateTempusCCAuthTransaction
        '====================================================================
        Dim lngTempusTransID As Long = -1
        Dim strXML As String = ""

        Try
            lngTempusTransID = CreateTempusTransReference(strToken, strXML, lngTransID)

            If lngTransID = -1 Then Throw New System.Exception("Unable to Create Tempus Transaction Reference. [SP CreateTempusCCAuthTransaction]")

        Catch ex As Exception
            Throw New System.Exception("Exception Creating Tempus Transaction Reference. [SP CreateTempusCCAuthTransaction] - " & ex.Message)

        End Try




    End Function

    Public Function CreateTempusTransReference(ByVal CardToken As String, ByVal XmlTransaction As String, ByVal TransactionID As Long) As Long
        Dim objConn As New SqlConnection
        Dim objCommand As New SqlCommand
        Dim objParam As New SqlParameter
        Dim objAdapter As New SqlDataAdapter
        Dim objDS As New DataSet("Tempup_Trans")
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim objReturnParam As New SqlParameter("RETURN", SqlDbType.Int)
        Dim objParamColl As New List(Of SqlParameter)
        Dim lngReturnValue As Long = -1


        '@CardToken nvarchar = NULL,
        '@XmlTransaction xml = NULL,
        '@BatchTransactionId INT = 0,
        '@TransactionId int = 0 OUTPUT

        Try

            objConn = New SqlConnection(DB_CONN)
            objCommand.Connection = objConn
            objCommand.CommandText = "CreateTempusCCAuthTransaction"
            objCommand.CommandType = CommandType.StoredProcedure

            objReturnParam.Direction = ParameterDirection.ReturnValue
            objCommand.Parameters.Add(objReturnParam)


            With objParamColl

                .Add(New SqlParameter("@CardToken", CardToken))
                .Add(New SqlParameter("@XmlTransaction", XmlTransaction))
                .Add(New SqlParameter("@BatchTransactionId", TransactionID))

                objCommand.Parameters.AddRange(.ToArray)

            End With

            objConn.Open()
            objCommand.ExecuteNonQuery()
            lngReturnValue = CType(objReturnParam.Value, Integer)

            If lngReturnValue < 1 Then lngReturnValue = -1


        Catch ex As Exception

            'Write(My.Settings.ERROR_LOG_NAME, My.Settings.ERROR_LOG_PATH, "LogACHTransaction".PadRight(35), "ACH_TransTable = " & ACH_TransTable & ", CustomerAccountID = " & CustomerAccountID & ", " & ex.Message)

        End Try

        objConn.Close()
        objCommand.Dispose()
        objAdapter.Dispose()
        objConn.Dispose()

        Return lngReturnValue

    End Function




    Public Function NextAutoPayImportBatchID(ByVal ImportedFile As String, ByVal ImportedDate As Date) As Long
        Dim objConn As New SqlConnection
        Dim objCommand As New SqlCommand
        Dim objParam As New SqlParameter
        Dim objAdapter As New SqlDataAdapter
        Dim objDS As New DataSet("AutoPay_Batch_ID")
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim objReturnParam As New SqlParameter("RETURN", SqlDbType.Int)
        Dim objParamColl As New List(Of SqlParameter)
        Dim lngReturnValue As Long = -1


        Try

            objConn = New SqlConnection(DB_CONN)
            objCommand.Connection = objConn
            objCommand.CommandText = "usp_AutoPay_Batch_Add"
            objCommand.CommandType = CommandType.StoredProcedure

            objReturnParam.Direction = ParameterDirection.ReturnValue
            objCommand.Parameters.Add(objReturnParam)


            With objParamColl

                .Add(New SqlParameter("@ImportedFile", ImportedFile))
                .Add(New SqlParameter("@ImportedDate", ImportedDate))

                objCommand.Parameters.AddRange(.ToArray)

            End With

            objConn.Open()
            objCommand.ExecuteNonQuery()
            lngReturnValue = CType(objReturnParam.Value, Integer)

            If lngReturnValue < 1 Then lngReturnValue = -1


        Catch ex As Exception

            'Write(My.Settings.ERROR_LOG_NAME, My.Settings.ERROR_LOG_PATH, "LogACHTransaction".PadRight(35), "ACH_TransTable = " & ACH_TransTable & ", CustomerAccountID = " & CustomerAccountID & ", " & ex.Message)

        End Try

        objConn.Close()
        objCommand.Dispose()
        objAdapter.Dispose()
        objConn.Dispose()

        Return lngReturnValue

    End Function

    Public Function UpdateAutoPayImportBatch(ByVal AutoPay_BatchID As Long, ByVal ImportedCount As Long, ByVal ImportedSum As Decimal, _
                                      ByVal AcceptedCount As Long, ByVal AcceptedSum As Decimal, _
                                      ByVal DuplicatesCount As Long, ByVal DuplicatesSum As Decimal, _
                                      ByVal InvalidCount As Long, ByVal ApprovedCount As Long, ByVal ApprovedSum As Decimal, _
                                      ByVal DeclinedCount As Long, ByVal DeclinedSum As Decimal, _
                                      ByVal Status As String) As Long

        Dim objConn As New SqlConnection
        Dim objCommand As New SqlCommand
        Dim objParam As New SqlParameter
        Dim objAdapter As New SqlDataAdapter
        Dim objDS As New DataSet("AutoPay_Batch_ID")
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim objReturnParam As New SqlParameter("RETURN", SqlDbType.Int)
        Dim objParamColl As New List(Of SqlParameter)
        Dim lngReturnValue As Long = -1



        Try

            objConn = New SqlConnection(DB_CONN)
            objCommand.Connection = objConn
            objCommand.CommandText = "usp_AutoPay_Batch_Update"
            objCommand.CommandType = CommandType.StoredProcedure

            objReturnParam.Direction = ParameterDirection.ReturnValue
            objCommand.Parameters.Add(objReturnParam)


            With objParamColl

                .Add(New SqlParameter("@AutoPay_BatchID", AutoPay_BatchID))
                .Add(New SqlParameter("@ImportedCount", ImportedCount))
                .Add(New SqlParameter("@ImportedSum", ImportedSum))
                .Add(New SqlParameter("@AcceptedCount", AcceptedCount))
                .Add(New SqlParameter("@AcceptedSum", AcceptedSum))
                .Add(New SqlParameter("@DuplicatesCount", DuplicatesCount))
                .Add(New SqlParameter("@DuplicatesSum", DuplicatesSum))
                .Add(New SqlParameter("@InvalidCount", InvalidCount))
                .Add(New SqlParameter("@ApprovedCount", ApprovedCount))
                .Add(New SqlParameter("@ApprovedSum", ApprovedSum))
                .Add(New SqlParameter("@DeclinedCount", DeclinedCount))
                .Add(New SqlParameter("@DeclinedSum", DeclinedSum))
                .Add(New SqlParameter("@Status", Status))

                objCommand.Parameters.AddRange(.ToArray)

            End With

            objConn.Open()
            objCommand.ExecuteNonQuery()
            lngReturnValue = CType(objReturnParam.Value, Integer)

            If lngReturnValue < 1 Then lngReturnValue = -1


        Catch ex As Exception


        End Try

        objConn.Close()
        objCommand.Dispose()
        objAdapter.Dispose()
        objConn.Dispose()

        Return lngReturnValue

    End Function

    Private Function DupeAutoPayAccountDataExists(ByVal AutoPay_Date As Date, ByVal AutoPay_Amount As Decimal, ByRef Customer_AccountID As String) As Boolean
        Dim objConn As New SqlConnection
        Dim objCommand As New SqlCommand
        Dim objParam As New SqlParameter
        Dim objAdapter As New SqlDataAdapter
        Dim objDS As New DataSet("AutoPay_Account")
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim objReturnParam As New SqlParameter("RETURN", SqlDbType.Int)
        Dim objParamColl As New List(Of SqlParameter)
        Dim lngReturnValue As Long = -1

        DupeAutoPayAccountDataExists = True

        Try

            objConn = New SqlConnection(DB_CONN)
            objCommand.Connection = objConn
            objCommand.CommandText = "usp_AutoPay_AccountData_Dupe_Exists"
            objCommand.CommandType = CommandType.StoredProcedure

            objReturnParam.Direction = ParameterDirection.ReturnValue
            objCommand.Parameters.Add(objReturnParam)


            With objParamColl

                .Add(New SqlParameter("@AutoPay_Date", AutoPay_Date))
                .Add(New SqlParameter("@AutoPay_Amount", AutoPay_Amount))
                .Add(New SqlParameter("@Customer_AccountID", Customer_AccountID))

                objCommand.Parameters.AddRange(.ToArray)

            End With

            objConn.Open()
            objCommand.ExecuteNonQuery()
            lngReturnValue = CType(objReturnParam.Value, Integer)

            DupeAutoPayAccountDataExists = (lngReturnValue > 0)


        Catch ex As Exception

            DupeAutoPayAccountDataExists = False

        End Try

        objConn.Close()
        objCommand.Dispose()
        objAdapter.Dispose()
        objConn.Dispose()

        Return DupeAutoPayAccountDataExists

    End Function

    Public Function InsertAutoPayAccountData(ByVal AutoPay_BatchID As Long, ByVal AutoPay_Date As Date, ByVal AutoPay_Amount As Decimal, _
                                            ByRef Customer_AccountID As String, ByVal FirstName As String, ByVal LastName As String, _
                                            ByVal BillAddr1 As String, ByVal BillCity As String, ByVal BillState As String, _
                                            ByVal BillZip As String, ByVal Status As String) As Long

        Dim objConn As New SqlConnection
        Dim objCommand As New SqlCommand
        Dim objParam As New SqlParameter
        Dim objAdapter As New SqlDataAdapter
        Dim objDS As New DataSet("AutoPay_Accounts")
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim objReturnParam As New SqlParameter("RETURN", SqlDbType.Int)
        Dim objParamColl As New List(Of SqlParameter)
        Dim lngReturnValue As Long = -1


        Try

            objConn = New SqlConnection(DB_CONN)
            objCommand.Connection = objConn
            objCommand.CommandText = "usp_AutoPay_AccountData_Add"
            objCommand.CommandType = CommandType.StoredProcedure

            objReturnParam.Direction = ParameterDirection.ReturnValue
            objCommand.Parameters.Add(objReturnParam)


            With objParamColl

                .Add(New SqlParameter("@AutoPay_BatchID", AutoPay_BatchID))
                .Add(New SqlParameter("@AutoPay_Date", AutoPay_Date))
                .Add(New SqlParameter("@AutoPay_Amount", AutoPay_Amount))
                .Add(New SqlParameter("@Customer_AccountID", Customer_AccountID))
                .Add(New SqlParameter("@FirstName", FirstName))
                .Add(New SqlParameter("@LastName", LastName))
                .Add(New SqlParameter("@BillAddr1", BillAddr1))
                .Add(New SqlParameter("@BillCity", BillCity))
                .Add(New SqlParameter("@BillState", BillState))
                .Add(New SqlParameter("@BillZip", BillZip))
                .Add(New SqlParameter("@Status", Status))

                objCommand.Parameters.AddRange(.ToArray)

            End With

            objConn.Open()
            objCommand.ExecuteNonQuery()
            lngReturnValue = CType(objReturnParam.Value, Integer)

            If lngReturnValue < 1 Then lngReturnValue = -1

        Catch ex As Exception


        End Try

        objConn.Close()
        objCommand.Dispose()
        objAdapter.Dispose()
        objConn.Dispose()

        Return lngReturnValue

    End Function

    Public Function UpdateAutoPayAccountData(ByVal AutoPay_ID As Long, ByVal AutoPay_Date As Date, ByVal AutoPay_Amount As Decimal, _
                                  ByVal Customer_AccountID As String, ByVal FirstName As String, _
                                  ByVal LastName As String, ByVal BillAddr1 As String, _
                                  ByVal BillCity As String, ByVal BillState As String, _
                                  ByVal BillZip As String, ByVal Status As String) As Long

        Dim objConn As New SqlConnection
        Dim objCommand As New SqlCommand
        Dim objParam As New SqlParameter
        Dim objAdapter As New SqlDataAdapter
        Dim objDS As New DataSet("AutoPay_ID")
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim objReturnParam As New SqlParameter("RETURN", SqlDbType.Int)
        Dim objParamColl As New List(Of SqlParameter)
        Dim lngReturnValue As Long = -1



        Try

            objConn = New SqlConnection(DB_CONN)
            objCommand.Connection = objConn
            objCommand.CommandText = "usp_AutoPay_AccountData_Update"
            objCommand.CommandType = CommandType.StoredProcedure

            objReturnParam.Direction = ParameterDirection.ReturnValue
            objCommand.Parameters.Add(objReturnParam)


            With objParamColl

                .Add(New SqlParameter("@AutoPay_ID", AutoPay_ID))
                .Add(New SqlParameter("@AutoPay_Date", AutoPay_Date))
                .Add(New SqlParameter("@AutoPay_Amount", AutoPay_Amount))
                .Add(New SqlParameter("@Customer_AccountID", Customer_AccountID))
                .Add(New SqlParameter("@FirstName", FirstName))
                .Add(New SqlParameter("@LastName", LastName))
                .Add(New SqlParameter("@BillAddr1", BillAddr1))
                .Add(New SqlParameter("@BillCity", BillCity))
                .Add(New SqlParameter("@BillState", BillState))
                .Add(New SqlParameter("@BillZip", BillZip))
                .Add(New SqlParameter("@Status", Status))

                objCommand.Parameters.AddRange(.ToArray)

            End With

            objConn.Open()
            objCommand.ExecuteNonQuery()
            lngReturnValue = CType(objReturnParam.Value, Integer)

            If lngReturnValue < 1 Then lngReturnValue = -1


        Catch ex As Exception


        End Try

        objConn.Close()
        objCommand.Dispose()
        objAdapter.Dispose()
        objConn.Dispose()

        Return lngReturnValue

    End Function

    Public Function ValidFile(ByVal FileName As String) As Boolean
        Dim strLine As String


        Try

            Dim objSR As StreamReader = New StreamReader(FileName)

            strLine = objSR.ReadLine()

            objSR.Close()

            If strLine.Length <> 78 Then Return False
            If Not (strLine.Substring(0, 16) Like "################") Then Return False
            If Not (strLine.Substring(65, 13) Like "#############") Then Return False
            If Not Val(strLine.Substring(65, 13)) > 0 Then Return False
            If Not IsDate(strLine.Substring(12, 2) & "-" & strLine.Substring(14, 2) & "-" & strLine.Substring(8, 4)) Then Return False

            Return True

        Catch E As Exception


            Return False

        End Try

    End Function

    Public Function GetAutoPayImportBatches(ByVal DateRange As String, Optional Status As String = "") As DataSet
        Dim objConn As SqlConnection
        Dim objAdapter As SqlDataAdapter
        Dim objDS As New DataSet
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim strSQL As String
        Dim strSQLFilter As String



        Try

            strSQL = "SELECT * FROM AutoPay_Batches"

            Select Case DateRange.ToUpper
                Case "30 DAYS"
                    strSQLFilter = " WHERE (ImportedDate BETWEEN CONVERT(DATETIME, '" & Today.AddDays(-30) & " 00:00:00', 102) AND CONVERT(DATETIME, '" & Today & " 00:00:00', 102))"
                Case "90 DAYS"
                    strSQLFilter = " WHERE (ImportedDate BETWEEN CONVERT(DATETIME, '" & Today.AddDays(-90) & " 00:00:00', 102) AND CONVERT(DATETIME, '" & Today & " 00:00:00', 102))"
                Case "6 MONTHS"
                    strSQLFilter = " WHERE (ImportedDate BETWEEN CONVERT(DATETIME, '" & Today.AddDays(-180) & " 00:00:00', 102) AND CONVERT(DATETIME, '" & Today & " 00:00:00', 102))"
                Case Else
                    strSQLFilter = ""
            End Select

            If Status <> "" And Status <> "* All *" Then

                If strSQLFilter = "" Then

                    strSQLFilter = " WHERE [Status] = '" & Status & "' ORDER BY AutoPay_BatchID DESC"

                Else

                    strSQLFilter = strSQLFilter & " AND [Status] = '" & Status & "' ORDER BY AutoPay_BatchID DESC"

                End If


            End If

            strSQL = strSQL & strSQLFilter

            objConn = New SqlConnection(DB_CONN)
            objAdapter = New SqlDataAdapter(strSQL, objConn)
            objDS = New DataSet
            objAdapter.Fill(objDS, "SystemData")

        Catch ex As Exception

            MessageBox.Show("Unable to Select AutoPay Import Records.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End Try

        Return objDS

    End Function

    Public Function GetAutoPayImportBatch(ByVal AutoPay_BatchID As String) As DataSet
        Dim objConn As SqlConnection
        Dim objAdapter As SqlDataAdapter
        Dim objDS As New DataSet
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim strSQL As String


        Try

            strSQL = "SELECT * FROM AutoPay_Batches WHERE AutoPay_BatchID = " & AutoPay_BatchID

            objConn = New SqlConnection(DB_CONN)
            objAdapter = New SqlDataAdapter(strSQL, objConn)
            objDS = New DataSet
            objAdapter.Fill(objDS, "SystemData")

        Catch ex As Exception

            MessageBox.Show("Unable to Select AutoPay Import Record.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End Try

        Return objDS

    End Function

    Public Function GetAutoPayBatchAccountData(ByVal AutoPay_BatchID As Long) As DataSet
        Dim objConn As SqlConnection
        Dim objAdapter As SqlDataAdapter
        Dim objDS As New DataSet
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim strSQL As String


        Try

            strSQL = "SELECT * FROM AutoPay_AccountData WHERE AutoPay_BatchID = " & AutoPay_BatchID

            objConn = New SqlConnection(DB_CONN)
            objAdapter = New SqlDataAdapter(strSQL, objConn)
            objDS = New DataSet
            objAdapter.Fill(objDS, "SystemData")

        Catch ex As Exception

            Throw New System.Exception("Unable to Select AutoPay Account Data Records for this Batch (" & AutoPay_BatchID & "). [GetAutoPayBatchAccountData]", ex.InnerException)

        End Try

        Return objDS

    End Function

    Public Function GetAutoPayAccountData(ByVal AutoPay_ID As String) As DataRow
        Dim objConn As SqlConnection
        Dim objAdapter As SqlDataAdapter
        Dim objDS As New DataSet
        Dim objDR As DataRow = Nothing
        Dim DB_CONN As String = My.Settings.BATCH_DB_CONN
        Dim strSQL As String


        Try

            strSQL = "SELECT * FROM AutoPay_AccountData WHERE AutoPay_ID = " & AutoPay_ID

            objConn = New SqlConnection(DB_CONN)
            objAdapter = New SqlDataAdapter(strSQL, objConn)
            objDS = New DataSet
            objAdapter.Fill(objDS, "SystemData")

            If objDS.Tables.Count > 0 Then
                If objDS.Tables(0).Rows.Count > 0 Then
                    objDR = objDS.Tables(0).Rows(0)
                End If

            End If


        Catch ex As Exception

            Throw New System.Exception("Unable to Select AutoPay Data Record. [GetAutoPayAccountData]")

        End Try

        Return objDR

    End Function

    Public Function GetCustomerCardToken(ByVal Customer_AccountID As String) As String
        Dim objConn As New SqlConnection
        Dim objCommand As New SqlCommand
        Dim objParam As New SqlParameter
        Dim objAdapter As New SqlDataAdapter
        Dim objDS As New DataSet("AutoPay_ID")
        Dim DB_CONN As String = My.Settings.SERVICES_DB_CONN
        Dim objParamColl As New List(Of SqlParameter)
        Dim strCardToken As String = ""


        Try

            objConn = New SqlConnection(DB_CONN)
            objCommand.Connection = objConn
            objCommand.CommandText = "GetTempusTokenFromAccount"
            objCommand.CommandType = CommandType.StoredProcedure

            With objParamColl

                .Add(New SqlParameter("@AccountNumber", Customer_AccountID))
                objCommand.Parameters.AddRange(.ToArray)

            End With

            objAdapter.SelectCommand = objCommand
            objAdapter.Fill(objDS)

            If objDS.Tables.Count > 0 Then strCardToken = objDS.Tables(0).Rows(0).Item(0).ToString

        Catch ex As Exception


        End Try

        objConn.Close()
        objCommand.Dispose()
        objAdapter.Dispose()
        objConn.Dispose()

        Return strCardToken

    End Function



End Class


