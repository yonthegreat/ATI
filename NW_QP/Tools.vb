Imports Microsoft.VisualBasic
Imports System.Diagnostics
Imports System.Web.HttpContext


Public Class Tools

    Public Shared Sub ClearSessionItems()

        Current.Session.Item("TEST") = ""

        Current.Session.Item("CLIENTID") = ""

        'Current.Session.Item("MERCHANT_ID") = ""
        'Current.Session.Item("WEB_HISTORY_TABLE") = ""
        'Current.Session.Item("CARD_TRANS_TABLE") = ""
        Current.Session.Item("CURRENT_STAGE") = ""
        Current.Session.Item("NAVIGATING_FROM") = ""

        Current.Session.Item("FIRST_NAME") = ""
        Current.Session.Item("LAST_NAME") = ""
        Current.Session.Item("STREET1") = ""
        Current.Session.Item("CITY") = ""
        Current.Session.Item("STATE") = ""
        Current.Session.Item("ZIP") = ""
        Current.Session.Item("SERVICE_ZIP_CODE") = ""

        Current.Session.Item("ENTERED_AMOUNT") = ""
        Current.Session.Item("ENTERED_CARD_NUMBER") = ""
        Current.Session.Item("ENTERED_EXPIRATION") = ""
        Current.Session.Item("ENTERED_SECURITY_CODE") = ""
        Current.Session.Item("ENTERED_SERVICE_ZIP_CODE") = ""

        Current.Session.Item("ACCOUNT_CURRENT") = ""
        Current.Session.Item("ACCOUNT_INFORMATION") = ""
        Current.Session.Item("ACCOUNT_NUMBER") = ""
        Current.Session.Item("ACCOUNT_PAST_DUE") = ""
        Current.Session.Item("ACCOUNT_TYPE") = ""
        Current.Session.Item("AUTHENTICATED") = ""
        Current.Session.Item("AUTHORIZATION_STATUS") = ""


        Current.Session.Item("NWN_POSTED") = ""
        Current.Session.Item("CARD_TRANS_ID") = ""
        Current.Session.Item("WEB_HISTORY_ID") = ""
        Current.Session.Item("USAGE_STATS_UPDATED") = ""

        Current.Session.Item("Cyber_CaptureApproved") = ""
        Current.Session.Item("Cyber_CaptureReasonCode") = ""
        Current.Session.Item("CYBER_ERROR") = ""
        Current.Session.Item("CYBER_REPLY") = ""

        Current.Session.Item("WEB_EVENTLOG") = ""
        Current.Session.Item("EXPIRED_FORBIDDEN_MESSAGE_SENT") = "FALSE"

    End Sub

    Public Shared Function SessionTimedOut(ByVal strSessionTimeout As String, ByRef strLastDateTime As String) As Boolean
        On Error Resume Next
        ' strLastDateTime parameter is passed ByRef so this Function
        ' can change the contents of the variable that is passed.
        Dim dtLastDateTime As Date
        Dim intSessionTimeout As Long
        Dim intElapsedMinutes As Long

        ' Code that used to call this Function
        ' ===================================================================================================================
        'If Tools.SessionTimedOut(Session.Item("SESSION_TIMEOUT"), Session.Item("LAST_ACTIVITY_DATETIME")) Then
        '    SessionInvalid = True
        '    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid. Timeout = " & Session.Item("SESSION_TIMEOUT") & ", LastDateTime = " & Session.Item("LAST_ACTIVITY_DATETIME"))
        'End If

        ' No time existed, set time as Now
        If strLastDateTime = "" Then
            strLastDateTime = Convert.ToString(Now)
            Return False
        End If

        intSessionTimeout = Convert.ToInt64(strSessionTimeout)
        dtLastDateTime = CDate(strLastDateTime)
        intElapsedMinutes = DateDiff(DateInterval.Minute, dtLastDateTime, Now)
        SessionTimedOut = intElapsedMinutes >= intSessionTimeout

        ' Reset Last Activity Time.
        If Not SessionTimedOut Then strLastDateTime = Convert.ToString(Now)

    End Function

    Public Shared Sub AddWebHistoryItem(ByRef History As String, ByVal Value As String)
        On Error Resume Next
        If History Is Nothing Then History = ""
        If History.Trim = "" Then
            History = Now.ToString("MM/dd/yy HH:mm:ss") & " - " & Value
        Else
            History = History & "|" & Now.ToString("MM/dd/yy HH:mm:ss") & " - " & Value
        End If
    End Sub

    Public Shared Function WriteToWebHistoryDB(ByVal Source As String, ByVal Web_HistoryID As String, ByVal SessionID As String, _
                ByVal SessionIP As String, ByVal SessionStart As DateTime, ByVal SessionEnd As DateTime, _
                ByVal WebHistory As String, ByVal ClientID As Long, ByVal AccountNumber As String, _
                ByVal CardTransID As Long, ByVal IsError As Long, ByVal Test As Boolean) As Long



        Dim objWebSvc As Object
        Dim intRecordID As Long = -1

        WebHistory = Right(WebHistory, 8000)

        If Val(Web_HistoryID) < 1 Then

            If Test Then
                objWebSvc = New NWWS_TestRelay.TestRelaySoapClient
            Else
                objWebSvc = New NWWS_Relay.RelaySoapClient
            End If

            Try
                intRecordID = objWebSvc.LogWebHistory(Source, SessionID, SessionIP, SessionStart, _
                                SessionEnd, WebHistory, ClientID, AccountNumber, CardTransID, IsError)

            Catch se As System.Exception
            End Try

            Return intRecordID

        Else

            Try
                'UpdateWebHistoryDB(Web_HistoryID, SessionEnd, WebHistory, CardTransID, IsError, Test)
            Catch ex As System.Exception
            End Try

            Return Web_HistoryID

        End If

    End Function

    Public Shared Function UpdateWebHistoryDB_old(ByVal Web_HistoryID As String, ByVal SessionEnd As DateTime, _
                ByVal WebHistory As String, ByVal CardTransID As Long, ByVal IsError As Long, ByVal Test As Boolean) As Boolean

        Dim objWebSvc As Object
        Dim blnUpdated As Boolean

        WebHistory = Right(WebHistory, 8000)

        If Test Then
            objWebSvc = New NWWS_TestRelay.TestRelaySoapClient
        Else
            objWebSvc = New NWWS_Relay.RelaySoapClient
        End If

        Try

            blnUpdated = objWebSvc.UpdateWebHistory(Web_HistoryID, SessionEnd, WebHistory, CardTransID, IsError)
            Return blnUpdated

        Catch se As System.Exception

            Return blnUpdated

        End Try

    End Function

    Public Shared Function StripDelimChars(ByVal Value As String) As String
        Value.Replace(",", "")
        Value.Replace("'", "")
        Value.Replace(Chr(34), "")
        Return (Value)
    End Function

    Public Shared Function SendEmail(ByVal MailSender As String, ByVal MailToAddrs As String, ByVal MailCCAddrs As String, ByVal MailBCCAddrs As String, ByVal MailSubject As String, ByVal MailMessage As String, ByVal MailAttachments As String, ByVal MailPriority As System.Net.Mail.MailPriority, Optional ByRef Log As String = "") As Boolean
        Dim strMailServer As String = Trim(ConfigurationManager.AppSettings.Get("MAIL_SERVER") & "")
        Dim strMailServerPort As Integer = CInt(ConfigurationManager.AppSettings.Get("MAIL_SERVER_PORT") + 0)
        Dim objMessage As New System.Net.Mail.MailMessage
        Dim objAttachment As System.Net.Mail.Attachment
        Dim objSMTPClient As System.Net.Mail.SmtpClient
        Dim strTo() As String
        Dim strCC() As String
        Dim strBCC() As String
        Dim strAttach() As String
        Dim strToAddr As String = ""
        Dim strAddr As String
        Dim strAttachment As String


        Try

            Log = "Splitting TO Addresses"
            strTo = Split(MailToAddrs, ";")
            For Each strAddr In strTo
                If strAddr.Trim <> "" Then
                    strToAddr = strAddr.Trim
                    Exit For
                End If
            Next

            If strMailServer = "" Or strMailServerPort = 0 Or strToAddr.Trim = "" Then
                Log = Log & vbCrLf & "Invalid MailServer Data"
                Return False
            End If

            Log = Log & vbCrLf & "Creating SMTP Client"
            objSMTPClient = New System.Net.Mail.SmtpClient(strMailServer, strMailServerPort)
            Log = Log & vbCrLf & "Creating Mail Message"
            objMessage = New System.Net.Mail.MailMessage(MailSender, strToAddr, MailSubject, MailMessage)

            Log = Log & vbCrLf & "Splitting FROM Addresses"
            strTo = Split(MailToAddrs, ";")
            For Each strAddr In strTo
                If strAddr.Trim <> strToAddr.Trim And strAddr.Trim <> "" Then objMessage.To.Add(strAddr.Trim)
            Next

            If MailCCAddrs.Trim <> "" Or MailCCAddrs <> String.Empty Then
                Log = Log & vbCrLf & "Splitting CC Addresses"
                strCC = Split(MailCCAddrs, ";")
                For Each strAddr In strCC
                    If strAddr.Trim <> "" Then objMessage.CC.Add(strAddr.Trim)
                Next
            End If

            If MailBCCAddrs.Trim <> "" Or MailBCCAddrs <> String.Empty Then
                Log = Log & vbCrLf & "Splitting BCC Addresses"
                strBCC = Split(MailBCCAddrs, ";")
                For Each strAddr In strBCC
                    If strAddr.Trim <> "" Then objMessage.Bcc.Add(strAddr.Trim)
                Next
            End If

            If MailAttachments.Trim <> "" Or MailAttachments <> String.Empty Then
                Log = Log & vbCrLf & "Splitting Attachments"
                strAttach = Split(MailAttachments, ";")
                For Each strAttachment In strAttach
                    If strAttachment.Trim <> "" Then
                        objAttachment = New Net.Mail.Attachment(strAttachment.Trim)
                        objMessage.Attachments.Add(objAttachment)
                    End If
                Next
            End If

            Log = Log & vbCrLf & "Setting Priority"
            objMessage.Priority = MailPriority
            Log = Log & vbCrLf & "Sending Mail Message"
            objSMTPClient.Send(objMessage)

            Return True

        Catch ex As System.Exception

            Log = Log & vbCrLf & "Exception: " & ex.Message.ToString
            Return False

        End Try

    End Function

    Public Shared Function LogWebHistory(ByVal Web_HistoryTable As String, ByVal Web_ProfileID As Long, ByVal SessionID As String, ByVal SessionIP As String, _
                                    ByVal SessionStart As DateTime, ByVal SessionFinish As DateTime, ByVal Schedule As String, ByVal Credentials As String, _
                                    ByVal CustomerID As Long, ByVal CustomerAccountID As String, ByVal CardTransID As Long, ByVal ACHTransID As Long, ByVal WebError As Boolean, _
                                    ByVal TextCaption1 As String, ByVal TextCaption2 As String, ByVal TextCaption3 As String, ByVal TextCaption4 As String, _
                                    ByVal TextCaption5 As String, ByVal TextCaption6 As String, ByVal TextCaption7 As String, ByVal TextCaption8 As String, _
                                    ByVal Text1 As String, ByVal Text2 As String, ByVal Text3 As String, ByVal Text4 As String, _
                                    ByVal Text5 As String, ByVal Text6 As String, ByVal Text7 As String, ByVal Text8 As String, _
                                    ByVal DateCaption1 As String, ByVal DateCaption2 As String, ByVal DateCaption3 As String, ByVal DateCaption4 As String, _
                                    ByVal Date1 As DateTime, ByVal Date2 As DateTime, ByVal Date3 As DateTime, ByVal Date4 As DateTime, _
                                    ByVal IntegerCaption1 As String, ByVal IntegerCaption2 As String, ByVal IntegerCaption3 As String, ByVal IntegerCaption4 As String, _
                                    ByVal Integer1 As Long, ByVal Integer2 As Long, ByVal Integer3 As Long, ByVal Integer4 As Long, _
                                    ByVal NumericCaption1 As String, ByVal NumericCaption2 As String, ByVal NumericCaption3 As String, ByVal NumericCaption4 As String, _
                                    ByVal Numeric1 As Decimal, ByVal Numeric2 As Decimal, ByVal Numeric3 As Decimal, ByVal Numeric4 As Decimal, _
                                    ByVal MoneyCaption1 As String, ByVal MoneyCaption2 As String, ByVal MoneyCaption3 As String, ByVal MoneyCaption4 As String, _
                                    ByVal Money1 As Double, ByVal Money2 As Double, ByVal Money3 As Double, ByVal Money4 As Double, _
                                    ByVal BitCaption1 As String, ByVal BitCaption2 As String, ByVal BitCaption3 As String, ByVal BitCaption4 As String, _
                                    ByVal Bit1 As Boolean, ByVal Bit2 As Boolean, ByVal Bit3 As Boolean, ByVal Bit4 As Boolean) As Long
        Dim objWS As New IPSToolsWS.ToolsSoapClient
        Dim intRecordID As Long = -1


        Try

            intRecordID = objWS.LogWebHistory(Web_HistoryTable, Web_ProfileID, SessionID, SessionIP, SessionStart, SessionFinish, Schedule, Credentials, _
                                     CustomerID, CustomerAccountID, CardTransID, ACHTransID, WebError, TextCaption1, TextCaption2, TextCaption3, TextCaption4, _
                                     TextCaption5, TextCaption6, TextCaption7, TextCaption8, Text1, Text2, Text3, Text4, Text5, Text6, Text7, Text8, _
                                     DateCaption1, DateCaption2, DateCaption3, DateCaption4, Date1, Date2, Date3, Date4, _
                                     IntegerCaption1, IntegerCaption2, IntegerCaption3, IntegerCaption4, Integer1, Integer2, Integer3, Integer4, _
                                     NumericCaption1, NumericCaption2, NumericCaption3, NumericCaption4, Numeric1, Numeric2, Numeric3, Numeric4, _
                                     MoneyCaption1, MoneyCaption2, MoneyCaption3, MoneyCaption4, Money1, Money2, Money3, Money4, _
                                     BitCaption1, BitCaption2, BitCaption3, BitCaption4, Bit1, Bit2, Bit3, Bit4)


            If Current.Session.Item("LOG_SUCCESSFUL_ACTIVITY") = "TRUE" Then
                AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "LogWebHistory", "WebHistoryTable = " & Web_HistoryTable & ", CustomerAccountID = " & CustomerAccountID, False, Current.Session.Item("TEST") = "TEST")
            End If

        Catch se As System.Exception

            AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "LogWebHistory", "WebHistoryTable = " & Web_HistoryTable & ", CustomerAccountID = " & CustomerAccountID & ", Error: " & se.Message, True, Current.Session.Item("TEST") = "TEST")

        End Try

        Return intRecordID

    End Function


    Public Shared Function UpdateWebHistoryEventLog(ByVal Web_HistoryTable As String, ByVal Web_HistoryID As Long, ByVal Web_EventLog As String, _
                                    ByVal SessionFinish As DateTime, ByVal CustomerAccountID As String, ByVal CardTransID As Long, _
                                    ByVal ACHTransID As Long, ByVal WebError As Boolean)
        Dim objWS As New IPSToolsWS.ToolsSoapClient
        Dim intRecordID As Long = -1


        Try

            intRecordID = objWS.UpdateWebHistoryEventLog(Web_HistoryTable, Web_HistoryID, Web_EventLog, SessionFinish, CustomerAccountID, CardTransID, ACHTransID, WebError)


            If Current.Session.Item("LOG_SUCCESSFUL_ACTIVITY") = "TRUE" Then
                AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "UpdateWebHistoryEventLog", "WebHistoryTable = " & Web_HistoryTable & ", Web_HistoryID = " & Web_HistoryID & ", CustomerAccountID = " & CustomerAccountID, False, Current.Session.Item("TEST") = "TEST")
            End If

        Catch se As System.Exception

            AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "UpdateWebHistoryEventLog", "WebHistoryTable = " & Web_HistoryTable & ", Web_HistoryID = " & Web_HistoryID & ", CustomerAccountID = " & CustomerAccountID & ", Error: " & se.Message, True, Current.Session.Item("TEST") = "TEST")

        End Try

        Return intRecordID

    End Function

    Public Shared Function UpdateWebHistoryCustom(ByVal Web_HistoryTable As String, ByVal Web_HistoryID As Long, SessionFinish As DateTime, ByVal Schedule As String, ByVal Credentials As String, _
                                    ByVal CustomerID As Long, ByVal CustomerAccountID As String, ByVal CardTransID As Long, ByVal ACHTransID As Long, ByVal WebError As Boolean, _
                                    ByVal TextCaption1 As String, ByVal TextCaption2 As String, ByVal TextCaption3 As String, ByVal TextCaption4 As String, _
                                    ByVal TextCaption5 As String, ByVal TextCaption6 As String, ByVal TextCaption7 As String, ByVal TextCaption8 As String, _
                                    ByVal Text1 As String, ByVal Text2 As String, ByVal Text3 As String, ByVal Text4 As String, _
                                    ByVal Text5 As String, ByVal Text6 As String, ByVal Text7 As String, ByVal Text8 As String, _
                                    ByVal DateCaption1 As String, ByVal DateCaption2 As String, ByVal DateCaption3 As String, ByVal DateCaption4 As String, _
                                    ByVal Date1 As DateTime, ByVal Date2 As DateTime, ByVal Date3 As DateTime, ByVal Date4 As DateTime, _
                                    ByVal IntegerCaption1 As String, ByVal IntegerCaption2 As String, ByVal IntegerCaption3 As String, ByVal IntegerCaption4 As String, _
                                    ByVal Integer1 As Long, ByVal Integer2 As Long, ByVal Integer3 As Long, ByVal Integer4 As Long, _
                                    ByVal NumericCaption1 As String, ByVal NumericCaption2 As String, ByVal NumericCaption3 As String, ByVal NumericCaption4 As String, _
                                    ByVal Numeric1 As Decimal, ByVal Numeric2 As Decimal, ByVal Numeric3 As Decimal, ByVal Numeric4 As Decimal, _
                                    ByVal MoneyCaption1 As String, ByVal MoneyCaption2 As String, ByVal MoneyCaption3 As String, ByVal MoneyCaption4 As String, _
                                    ByVal Money1 As Double, ByVal Money2 As Double, ByVal Money3 As Double, ByVal Money4 As Double, _
                                    ByVal BitCaption1 As String, ByVal BitCaption2 As String, ByVal BitCaption3 As String, ByVal BitCaption4 As String, _
                                    ByVal Bit1 As Boolean, ByVal Bit2 As Boolean, ByVal Bit3 As Boolean, ByVal Bit4 As Boolean) As Long
        Dim objWS As New IPSToolsWS.ToolsSoapClient
        Dim intRecordID As Long = -1


        Try

            intRecordID = objWS.UpdateWebHistoryCustom(Web_HistoryTable, Web_HistoryID, SessionFinish, Schedule, Credentials, CustomerID, CustomerAccountID, CardTransID, ACHTransID, WebError, _
                                     TextCaption1, TextCaption2, TextCaption3, TextCaption4, TextCaption5, TextCaption6, TextCaption7, TextCaption8, Text1, Text2, Text3, Text4, Text5, Text6, Text7, Text8, _
                                     DateCaption1, DateCaption2, DateCaption3, DateCaption4, Date1, Date2, Date3, Date4, _
                                     IntegerCaption1, IntegerCaption2, IntegerCaption3, IntegerCaption4, Integer1, Integer2, Integer3, Integer4, _
                                     NumericCaption1, NumericCaption2, NumericCaption3, NumericCaption4, Numeric1, Numeric2, Numeric3, Numeric4, _
                                     MoneyCaption1, MoneyCaption2, MoneyCaption3, MoneyCaption4, Money1, Money2, Money3, Money4, _
                                     BitCaption1, BitCaption2, BitCaption3, BitCaption4, Bit1, Bit2, Bit3, Bit4)


            If Current.Session.Item("LOG_SUCCESSFUL_ACTIVITY") = "TRUE" Then
                AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "UpdateWebHistoryCustom", "WebHistoryTable = " & Web_HistoryTable & ", Web_HistoryID = " & Web_HistoryID & ", CustomerAccountID = " & CustomerAccountID, False, Current.Session.Item("TEST") = "TEST")
            End If

        Catch se As System.Exception

            AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "UpdateWebHistoryCustom", "WebHistoryTable = " & Web_HistoryTable & ", Web_HistoryID = " & Web_HistoryID & ", CustomerAccountID = " & CustomerAccountID & ", Error: " & se.Message, True, Current.Session.Item("TEST") = "TEST")

        End Try

        Return intRecordID

    End Function

    Public Shared Function LogCardTrans(ByVal Card_TransTable As String, ByVal IVR_HistoryID As Long, ByVal IVR_ProfileID As Long, ByVal Web_HistoryID As Long, _
                                        ByVal TransDateTime As DateTime, ByVal CustomerID As Long, ByVal CustomerAccountID As String, ByVal FirstName As String, _
                                        ByVal LastName As String, ByVal BillAddr1 As String, ByVal BillAddr2 As String, ByVal BillCity As String, ByVal BillState As String, _
                                        ByVal BillZip As String, ByVal AccountLast4 As String, ByVal AccountType As String, ByVal Fee As Decimal, _
                                        ByVal Amount As Decimal, ByVal TransAmount As Decimal, ByVal TransIsDebit As Boolean, ByVal TransIsCredit As Boolean, _
                                        ByVal TransIsChargeback As Boolean, ByVal TransIsVoid As Boolean, ByVal WasVoided As Boolean, ByVal VoidReferenceID As Long, _
                                        ByVal Reconciled As Boolean, ByVal DataTokenID As String, ByVal MerchantID As String, ByVal MerchantReference As String, ByVal MerchantRequestID As String, _
                                        ByVal MerchantDecision As String, ByVal MerchantReasonCode As String, ByVal MerchantAuthCode As String, ByVal MerchantAVSCode As String, ByVal MerchantBatchID As Long, _
                                        ByVal ReconBatchID As Long, ByVal ReconDateTime As DateTime, ByVal RecurringPayment As Boolean, ByVal RecurringReferenceID As Long, _
                                        ByVal RecurringReferenceText As String, ByVal Failure As Boolean, ByVal TransNote As String, ByVal Misc1 As String, ByVal Misc2 As String, _
                                        ByVal Misc3 As String, ByVal Misc4 As String) As Long

        Dim objWS As New IPSToolsWS.ToolsSoapClient
        Dim intRecordID As Long = -1


        Try

            intRecordID = objWS.LogCreditCardTransaction2(Card_TransTable, IVR_HistoryID, IVR_ProfileID, Web_HistoryID, _
                                         TransDateTime, CustomerID, CustomerAccountID, FirstName, LastName, BillAddr1, BillAddr2, BillCity, BillState, BillZip, _
                                         AccountLast4, AccountType, Fee, Amount, TransAmount, TransIsDebit, TransIsCredit, _
                                         TransIsChargeback, TransIsVoid, WasVoided, VoidReferenceID, _
                                         Reconciled, DataTokenID, MerchantID, MerchantReference, MerchantRequestID, _
                                         MerchantDecision, MerchantReasonCode, MerchantAuthCode, MerchantAVSCode, MerchantBatchID, _
                                         ReconBatchID, ReconDateTime, RecurringPayment, RecurringReferenceID, _
                                         RecurringReferenceText, Failure, TransNote, Misc1, Misc2, Misc3, Misc4)

            If Current.Session.Item("LOG_SUCCESSFUL_ACTIVITY") = "TRUE" Then
                AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "LogCreditCardTransaction2", "CardTransTable = " & Card_TransTable & ", CardTransID = " & intRecordID & ", WebHistoryID = " & Web_HistoryID & ", CustomerAccountID = " & CustomerAccountID, False, Current.Session.Item("TEST") = "TEST")
            End If

        Catch se As System.Exception

            AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "LogCreditCardTransaction2", "CardTransTable = " & Card_TransTable & ", WebHistoryID = " & Web_HistoryID & ", CustomerAccountID = " & CustomerAccountID & ", Error: " & se.Message, True, Current.Session.Item("TEST") = "TEST")

        End Try

        Return intRecordID

    End Function

    Public Shared Function UpdateCardTrans(ByVal Card_TransTable As String, ByVal Card_TransID As Long, ByVal IVR_HistoryID As Long, ByVal IVR_ProfileID As Long, ByVal Web_HistoryID As Long, _
                                        ByVal TransDateTime As DateTime, ByVal CustomerID As Long, ByVal CustomerAccountID As String, ByVal FirstName As String, _
                                        ByVal LastName As String, ByVal BillAddr1 As String, ByVal BillAddr2 As String, ByVal BillCity As String, ByVal BillState As String, _
                                        ByVal BillZip As String, ByVal AccountLast4 As String, ByVal AccountType As String, ByVal Fee As Decimal, _
                                        ByVal Amount As Decimal, ByVal TransAmount As Decimal, ByVal TransIsDebit As Boolean, ByVal TransIsCredit As Boolean, _
                                        ByVal TransIsChargeback As Boolean, ByVal TransIsVoid As Boolean, ByVal WasVoided As Boolean, ByVal VoidReferenceID As Long, _
                                        ByVal Reconciled As Boolean, ByVal DataTokenID As String, ByVal MerchantID As String, ByVal MerchantReference As String, ByVal MerchantRequestID As String, _
                                        ByVal MerchantDecision As String, ByVal MerchantReasonCode As String, ByVal MerchantAuthCode As String, ByVal MerchantAVSCode As String, ByVal MerchantBatchID As Long, _
                                        ByVal ReconBatchID As Long, ByVal ReconDateTime As DateTime, ByVal RecurringPayment As Boolean, ByVal RecurringReferenceID As Long, _
                                        ByVal RecurringReferenceText As String, ByVal Failure As Boolean, ByVal TransNote As String, ByVal Misc1 As String, ByVal Misc2 As String, _
                                        ByVal Misc3 As String, ByVal Misc4 As String) As Long

        Dim objWS As New IPSToolsWS.ToolsSoapClient
        Dim intRecordID As Long = -1


        Try

            intRecordID = objWS.UpdateCreditCardTransaction(Card_TransTable, Card_TransID, IVR_HistoryID, IVR_ProfileID, Web_HistoryID, _
                                         TransDateTime, CustomerID, CustomerAccountID, FirstName, LastName, BillAddr1, BillAddr2, BillCity, BillState, BillZip, _
                                         AccountLast4, AccountType, Fee, Amount, TransAmount, TransIsDebit, TransIsCredit, _
                                         TransIsChargeback, TransIsVoid, WasVoided, VoidReferenceID, _
                                         Reconciled, DataTokenID, MerchantID, MerchantReference, MerchantRequestID, _
                                         MerchantDecision, MerchantReasonCode, MerchantAuthCode, MerchantAVSCode, MerchantBatchID, _
                                         ReconBatchID, ReconDateTime, RecurringPayment, RecurringReferenceID, _
                                         RecurringReferenceText, Failure, TransNote, Misc1, Misc2, Misc3, Misc4)

            If Current.Session.Item("LOG_SUCCESSFUL_ACTIVITY") = "TRUE" Then
                AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "UpdateCreditCardTrans", "CardTransTable = " & Card_TransTable & ", CardTransID = " & intRecordID & ", WebHistoryID = " & Web_HistoryID & ", CustomerAccountID = " & CustomerAccountID, False, Current.Session.Item("TEST") = "TEST")
            End If

        Catch se As System.Exception

            AppTextLog.WriteLogEntry(Current.Session.Item("LOG_DRIVE"), "UpdateCreditCardTrans", "CardTransTable = " & Card_TransTable & ", WebHistoryID = " & Web_HistoryID & ", CustomerAccountID = " & CustomerAccountID & ", Error: " & se.Message, True, Current.Session.Item("TEST") = "TEST")

        End Try

        Return intRecordID

    End Function



    'Public Shared Sub ApplicationEvent(ByVal AppName As String, ByVal EventType As EventLogEntryType, ByVal EventName As String, ByVal EventText As String)
    '    On Error Resume Next

    '    Dim objLog As EventLog
    '    Dim strLog As String = "Application"
    '    Dim strMachine As String = "."


    '    If Not EventLog.SourceExists(AppName, strMachine) Then EventLog.CreateEventSource(AppName, strLog)

    '    objLog = New EventLog(strLog, strMachine, AppName)

    '    objLog.WriteEntry(EventName)
    '    objLog.WriteEntry(EventName, EventType, 234, CType(3, Short))

    'End Sub

    Public Shared Function CheckUserAgentString(userAgent As String)
        Dim isMoble As Boolean
        Dim bString, vString As String
        Dim b As Regex
        Dim v As Regex


        bString = "android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|meego.+mobile|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino"
        vString = "1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-"

        isMoble = False

        If Not (String.IsNullOrEmpty(userAgent)) Then

            b = New Regex(bString, RegexOptions.IgnoreCase Or RegexOptions.Multiline)
            v = New Regex(vString, RegexOptions.IgnoreCase Or RegexOptions.Multiline)

            If Not [String].IsNullOrEmpty(userAgent) Then

                Try
                    isMoble = ((b.IsMatch(userAgent) OrElse v.IsMatch(userAgent.Substring(0, 4))))

                    ' don't care, just return false

                Catch

                    ' Do nothing 

                End Try

            End If

        End If

        Return isMoble

    End Function



End Class
