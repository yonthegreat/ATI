Public Class Receipt
    Inherits System.Web.UI.Page
    Protected creditCardRequestInfo As CreditCardRequestModel
    Private Const rep As String = "WEB"
    Private Const source As String = "INTERACTIVE PAYMENTS"
    Private Const ACTUAL_PAGE As String = "Receipt"
    Private Const STAGE_PAGE As String = "Receipt"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim maintenanceClient As App_In_Maintenance.UtilitySoapClient = New App_In_Maintenance.UtilitySoapClient()
        'TEST UnComment To force maintenance Page to appear
        'bool result = maintenanceClient.App_In_Maintenance("NWNAutoPaySite_True");
        Dim appName As String = String.Empty

        Try
            appName = ConfigurationManager.AppSettings("AppName")
            Exit Try
        Catch we As System.Exception
            Throw New Exception("Web.Config AppName is not defined")
        End Try

        Dim maintenanceMode As Boolean = maintenanceClient.App_In_Maintenance(appName)

        If (maintenanceMode) Then
            Response.Redirect(System.Web.VirtualPathUtility.ToAbsolute("~/MaintenancePage.aspx"))
        End If


        creditCardRequestInfo = Session("CreditCardRequestInfo")
        If Not creditCardRequestInfo Is Nothing Then
            'If CallCreditCard() Then
            'If CallSubmitPaymentNotificaiton() Then
            'ClearSession()
            ' Else
            'ClearSession()
            'Dim errorMessages As New List(Of [String])
            'errorMessages.Add("Issue contacting NW Natural")
            'Session.Add("ErrorMessages", errorMessages)
            'Response.Redirect("SubmissionIssue.aspx")
            'End If
            If CallCreditCard() <> True Then
                'ClearSession()
                Dim errorMessages As New List(Of [String])
                errorMessages.Add("Issue processing credit card: Code " & Session.Item("Cyber_CaptureReasonCode") & ". " & CodeDescription(Session.Item("Cyber_CaptureReasonCode")))
                Session.Add("ErrorMessages", errorMessages)
                Response.Redirect("SubmissionIssue.aspx")
            End If
        End If

        Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": PageLoad, non Postback.")

        Session.Item("NAVIGATING_FROM") = STAGE_PAGE

        AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", "Displayed Transaction Results to Customer. Account = " & Session.Item("ACCOUNT_NUMBER"), False, Session.Item("TEST") & "" = "TEST")

        ' ========================================================
        ' Update Final Web History
        ' ========================================================
        Try

            With Session

                Tools.UpdateWebHistoryEventLog(.Item("WEB_HISTORY_TABLE"), .Item("WEB_HISTORY_ID"), .Item("WEB_EVENTLOG"), Now, .Item("ACCOUNT_NUMBER"), .Item("CARD_TRANS_ID"), 0, False)

                ' Field Reference:
                'UpdateWebHistoryEventLog(ByVal Web_HistoryTable As String, ByVal Web_HistoryID As Long, ByVal Web_EventLog As String, _
                '               ByVal SessionFinish As DateTime, ByVal CustomerAccountID As String, ByVal CardTransID As Long, _
                '               ByVal ACHTransID As Long, ByVal WebError As Boolean)

            End With


        Catch se As SystemException

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Exception Calling UpdateWebHistoryEventLog. " & se.Message)

            AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                    "Exception Calling UpdateWebHistoryEventLog. " & _
                                    "CurrentStage = " & Session.Item("CURRENT_STAGE") & _
                                    ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                    ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                    ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                    ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                    ", CardTransID = " & Session.Item("CARD_TRANS_ID"), True, Session.Item("TEST") & "" = "TEST")

        End Try


    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

        If creditCardRequestInfo Is Nothing Then
            ClearSession()
            Response.Redirect("ExpiredForbidden.aspx")
        End If

    End Sub

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        If Request.Browser.IsMobileDevice Or SharedModules.CheckUserAgentString(Request.UserAgent) Then
            MasterPageFile = "~/NWNMobile.Master"
        End If
    End Sub

    Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
       
        ClearSession()
        Response.Redirect("http://www.nwnatural.com/AccountDashboard")
    End Sub

    Protected Sub ClearSession()
        Session.Clear()
        'Session.Abandon()
    End Sub

    Protected Function CallCreditCard() As Boolean
        If Not creditCardRequestInfo Is Nothing Then
            'creditCardRequestInfo.ConfirmationNumber = 123456
            Dim result As Boolean = LogAndAuthorize()
            If result = True And Not Session.Item("Cyber_AuthCode") Is Nothing And Not String.IsNullOrEmpty(Session.Item("Cyber_AuthCode").ToString()) Then
                creditCardRequestInfo.ConfirmationNumber = Session.Item("Cyber_AuthCode").ToString()
            End If
            Return result
        End If
        Return False
    End Function

    Private Sub SetReplyValues(ByVal objDS As DataSet)
        On Error Resume Next
        Dim LOG_CYBER_RETURN_VALUES As Boolean = IIf(UCase(ConfigurationManager.AppSettings("LOG_CYBER_RETURN_VALUES")) = "TRUE", True, False)

        Session.Item("Cyber_ResponseStatus") = ""
        Session.Item("Cyber_ResponseMessage") = ""
        Session.Item("Cyber_AuthCode") = ""
        Session.Item("Cyber_AuthReplyMessage") = ""
        Session.Item("Cyber_AuthAVSResult") = ""
        Session.Item("Cyber_CaptureDecision") = ""
        Session.Item("Cyber_CaptureReasonCode") = ""
        Session.Item("Cyber_CaptureReconciliationID") = ""
        Session.Item("Cyber_CaptureRequestID") = ""
        Session.Item("Cyber_CaptureApproved") = ""
        Session.Item("Cyber_CaptureReferenceNumber") = ""
        Session.Item("Cyber_CaptureRequestToken") = ""
        Session.Item("Cyber_CaptureDateTime") = ""

        If objDS.Tables.Count < 1 Then Exit Sub

        With objDS.Tables(0).Rows(0)
            
            Session.Item("Cyber_ResponseStatus") = .Item("ResponseStatus").ToString
            Session.Item("Cyber_ResponseMessage") = .Item("ResponseMessage").ToString
            If objDS.Tables(0).Columns.Count > 2 Then
                Session.Item("Cyber_AuthCode") = .Item("AuthCode").ToString
                Session.Item("Cyber_AuthReplyMessage") = .Item("AuthReplyMessage").ToString
                Session.Item("Cyber_AuthAVSResult") = .Item("AuthAVSResult").ToString
                Session.Item("Cyber_CaptureDecision") = .Item("CaptureDecision").ToString
                Session.Item("Cyber_CaptureReasonCode") = .Item("CaptureReasonCode").ToString
                Session.Item("Cyber_CaptureReconciliationID") = .Item("CaptureReconciliationID").ToString
                Session.Item("Cyber_CaptureRequestID") = .Item("CaptureRequestID").ToString
                Session.Item("Cyber_CaptureApproved") = .Item("CaptureApproved").ToString.ToUpper
                Session.Item("Cyber_CaptureReferenceNumber") = .Item("CaptureReferenceNumber").ToString
                Session.Item("Cyber_CaptureRequestToken") = .Item("CaptureRequestToken").ToString
                Session.Item("Cyber_CaptureDateTime") = .Item("CaptureDateTime").ToString
            End If
        End With

        If LOG_CYBER_RETURN_VALUES Then

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Cyber Results:")
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": ResponseStatus: " & Session.Item("Cyber_ResponseStatus"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": ResponseMessage: " & Session.Item("Cyber_ResponseMessage"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": AuthCode: " & Session.Item("Cyber_AuthCode"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": AuthReplyMessage: " & Session.Item("Cyber_AuthReplyMessage"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": AuthAVSResult: " & Session.Item("Cyber_AuthAVSResult"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": CaptureDecision: " & Session.Item("Cyber_CaptureDecision"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": CaptureReasonCode: " & Session.Item("Cyber_CaptureReasonCode"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": CaptureReconciliationID: " & Session.Item("Cyber_CaptureReconciliationID"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": CaptureRequestID: " & Session.Item("Cyber_CaptureRequestID"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": CaptureApproved: " & Session.Item("Cyber_CaptureApproved"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": CaptureReferenceNumber: " & Session.Item("Cyber_CaptureReferenceNumber"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": CaptureRequestToken: " & Session.Item("Cyber_CaptureRequestToken"))
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": CaptureDateTime: " & Session.Item("Cyber_CaptureDateTime"))

        End If

    End Sub
    Private Function CodeDescription(ByVal Value As String) As String

        Session.Item("LAST_CODE_SECTION") = "CodeDescription"

        Select Case Val(Value)
            Case -1
                CodeDescription = "Error: General System Error."   ' <  -1 was added as a custom code
            Case 100
                CodeDescription = "Successful."
            Case 101
                CodeDescription = "The request is missing one or more required fields. "
            Case 102
                CodeDescription = "One or more fields in the request contains invalid data."
            Case 150
                CodeDescription = "Error: General System Error."
            Case 151
                CodeDescription = "Error: The request was received but there was a server timeout. This error does not include timeouts between the client and the server."
            Case 152
                CodeDescription = "Error: The request was received, but a service did not finish running in time. "
            Case 200
                CodeDescription = "The authorization request was approved by the issuing bank but declined because it did not pass the Address Verification Service (AVS) check."
            Case 201
                CodeDescription = "The issuing bank has questions about the request. You do not receive an authorization code programmatically, but you might receive one verbally by calling the processor."
            Case 202
                CodeDescription = "Expired card."
            Case 203
                CodeDescription = "General decline of the card. No other information provided by the issuing bank."
            Case 204
                CodeDescription = "Insufficient funds in the account."
            Case 205
                CodeDescription = "General decline of the card." '"Stolen or lost card."
            Case 207
                CodeDescription = "Issuing bank unavailable."
            Case 208
                CodeDescription = "Inactive card or card not authorized for card-not-present transactions."
            Case 209
                CodeDescription = "American Express Card Identification Digits (CID) did not match."
            Case 210
                CodeDescription = "The card has reached the credit limit."
            Case 211
                CodeDescription = "Invalid card verification number or expiration date."
            Case 221
                CodeDescription = "General decline of the card." '"The customer matched an entry on the processor’s negative file."
            Case 230
                CodeDescription = "The authorization request was approved by the issuing bank, but declined because it did not pass the card verification (CV) check."
            Case 231
                CodeDescription = "Invalid account number."
            Case 232
                CodeDescription = "The card type is not accepted by the payment processor."
            Case 233
                CodeDescription = "General decline by the processor."
            Case 234
                CodeDescription = "Merchant configuration error."
            Case 235
                CodeDescription = "The requested amount exceeds the originally authorized amount. Occurs, for example, if you try to capture an amount larger than the original authorization amount."
            Case 236
                CodeDescription = "Processor failure."
            Case 237
                CodeDescription = "The authorization has already been reversed."
            Case 238
                CodeDescription = "The authorization has already been captured."
            Case 239
                CodeDescription = "The requested transaction amount must match the previous transaction amount."
            Case 240
                CodeDescription = "The card type sent is invalid or does not correlate with the credit card number."
            Case 241
                CodeDescription = "The request ID is invalid."
            Case 242
                CodeDescription = "You requested a capture, but there is no corresponding, unused authorization record. Occurs if there was not a previously successful authorization request or if the previously successful authorization has already been used by another capture request."
            Case 250
                CodeDescription = "Error: The request was received, but there was a timeout at the payment processor."
            Case Else
                CodeDescription = "No description available."
        End Select

    End Function


    Private Function SessionInvalid() As Boolean

        SessionInvalid = False

        If Session.Item("AUTHENTICATED") <> "TRUE" Then

            SessionInvalid = True
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid. Authenticated = False")
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "SessionInValid.  Authenticated = False")
            AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. Authenticated = FALSE, LastCodeSection = " & Session.Item("LAST_CODE_SECTION") & _
                            ", CurrentStage = " & Session.Item("CURRENT_STAGE") & ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"), True, Session.Item("TEST") & "" = "TEST")

        End If

        If STAGE_PAGE <> "" And Session.Item("CURRENT_STAGE") <> STAGE_PAGE Then

            If Session.Item("NAVIGATING_FROM") <> STAGE_PAGE Then

                SessionInvalid = True
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  NavigatingFrom = " & Session.Item("NAVIGATING_FROM"))
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  Should be NavigatingFrom = " & STAGE_PAGE)
                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. Navigating from (<>) " & Session.Item("NAVIGATING_FROM") & ", should be " & STAGE_PAGE & ", LastCodeSection = " & Session.Item("LAST_CODE_SECTION") & _
                                ", CurrentStage = " & Session.Item("CURRENT_STAGE") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"), True, Session.Item("TEST") & "" = "TEST")
            Else

                ' Was previously Removed
                SessionInvalid = True
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  Stage = " & Session.Item("CURRENT_STAGE"))
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  NavigatingFrom = " & Session.Item("NAVIGATING_FROM"))
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  Should be Stage = " & STAGE_PAGE)
                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. Navigating from " & Session.Item("NAVIGATING_FROM") & ", should be " & STAGE_PAGE & ", LastCodeSection = " & Session.Item("LAST_CODE_SECTION") & _
                                ", CurrentStage = " & Session.Item("CURRENT_STAGE") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"), True, Session.Item("TEST") & "" = "TEST")

            End If

        Else

            If STAGE_PAGE = "" Then

                SessionInvalid = True
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  STAGE_PAGE = ''")
                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. STAGE_PAGE = '', LastCodeSection = " & Session.Item("LAST_CODE_SECTION") & _
                                ", CurrentStage = " & Session.Item("CURRENT_STAGE") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"), True, Session.Item("TEST") & "" = "TEST")

            End If

        End If

        If SessionInvalid Then

            Session.Item("EXPIRED_FORBIDDEN_MESSAGE_SENT") = "TRUE"
            Response.Redirect(System.Web.VirtualPathUtility.ToAbsolute("~/ExpiredForbidden.aspx"))

        End If

    End Function


    Protected Function LogAndAuthorize() As Boolean
        Dim test = Session.Item("LAST_CODE_SECTION")
        Dim returnValue As Boolean
        returnValue = True
        Session.Item("LAST_CODE_SECTION") = IIf(Session.Item("LAST_CODE_SECTION") & "" = "", "ND ", "") & ACTUAL_PAGE & " Authorize_Click"

        If SessionInvalid() Then Return False


        Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Clicked Authorize.")

        ' For Testing/Debugging
        'Session.Item("AUTHORIZATION_STATUS") = "UNSENT"
        'Session.Item("AUTHORIZATION_STATUS") = "PROCESSING"
        'Session.Item("AUTHORIZATION_STATUS") = "COMPLETED"

        Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Authorization Status = " & Session.Item("AUTHORIZATION_STATUS"))

        If Session.Item("AUTHORIZATION_STATUS") = "UNSENT" Then

            Session.Item("AUTHORIZATION_STATUS") = "PROCESSING"


            ' ========================================================
            ' Create Web History Record
            ' ========================================================
            Try

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": LogWebHistory. ")

                If Val(Session.Item("WEB_HISTORY_ID")) = 0 Then
                    Session.Item("START_DATETIME") = Now.ToString
                    Session.Item("LAST_ACTIVITY_DATETIME") = Now.ToString
                    With Session
                        Session.Item("WEB_HISTORY_ID") = Tools.LogWebHistory(.Item("WEB_HISTORY_TABLE"), 0, .Item("SESSION_ID"), .Item("USER_HOST_ADDRESS"), .Item("START_DATETIME"), _
                                                             .Item("LAST_ACTIVITY_DATETIME"), "n/a", "", .Item("CLIENTID"), .Item("ACCOUNT_NUMBER"), 0, 0, True, _
                                                             "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", _
                                                             "", "", "", "", "1900-01-01", "1900-01-01", "1900-01-01", "1900-01-01", _
                                                             "", "", "", "", 0, 0, 0, 0, _
                                                             "", "", "", "", 0, 0, 0, 0, _
                                                             "", "", "", "", 0, 0, 0, 0, _
                                                             "", "", "", "", False, False, False, False)

                    End With

                End If

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Web_HistoryID = " & Session.Item("WEB_HISTORY_ID"))

            Catch se As SystemException

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Exception Logging Web_History Record. " & se.Message)

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                        "Exception Logging Web_History Record. " & _
                                        "CurrentStage = " & Session.Item("CURRENT_STAGE") & _
                                        ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                        ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                        ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                        ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                        ", CardTransID = " & Session.Item("CARD_TRANS_ID") & _
                                        ", AuthStatus = " & Session.Item("AUTHORIZATION_STATUS") & _
                                        ", CyberError = " & Session.Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")
                returnValue = False
            End Try



            ' ========================================================
            ' Create Card Trans Record
            ' ========================================================
            Try

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": LogCardTrans. ")


                With Session

                    Session.Item("CARD_TRANS_ID") = Tools.LogCardTrans(Session.Item("CARD_TRANS_TABLE"), 0, 0, Val(Session.Item("WEB_HISTORY_ID")), Now, Val(Session.Item("CLIENTID")), Session.Item("ACCOUNT_NUMBER"), _
                                                                     "", "", "", "", "", "", "", "", "", 0, Convert.ToDecimal(Session.Item("ENTERED_AMOUNT")), Convert.ToDecimal(Session.Item("ENTERED_AMOUNT")), True, False, False, False, False, 0, False, "", "", _
                                                                    Session.Item("ACCOUNT_NUMBER"), "", "", "", "", "", 0, 0, "1900-01-01", False, 0, "", False, "", "", "", "", "")

                    ' Field References:
                    'LogCreditCardTransaction(ByVal Card_TransTable As String, ByVal IVR_HistoryID As Long, ByVal IVR_ProfileID As Long, ByVal Web_HistoryID As Long, _
                    '                                        ByVal TransDateTime As DateTime, ByVal CustomerID As Long, ByVal CustomerAccountID As String, ByVal FirstName As String, _
                    '                                        ByVal LastName As String, ByVal BillAddr1 As String, ByVal BillAddr2 As String, ByVal BillCity As String, ByVal BillState As String, _
                    '                                        ByVal BillZip As String, ByVal AccountLast4 As String, ByVal AccountType As String, ByVal Fee As Decimal, _
                    '                                        ByVal Amount As Decimal, ByVal TransAmount As Decimal, ByVal TransIsDebit As Boolean, ByVal TransIsCredit As Boolean, _
                    '                                        ByVal TransIsChargeback As Boolean, ByVal TransIsVoid As Boolean, ByVal WasVoided As Boolean, ByVal VoidReferenceID As Long, _
                    '                                        ByVal Reconciled As Boolean, ByVal DataTokenID As String, ByVal MerchantID As String, ByVal MerchantReference As String, _
                    '                                        ByVal MerchantDecision As String, ByVal MerchantReasonCode As String, ByVal MerchantAuthCode As String, ByVal MerchantAVSCode As String, ByVal MerchantBatchID As Long, _
                    '                                        ByVal ReconBatchID As Long, ByVal ReconDateTime As DateTime, ByVal RecurringPayment As Boolean, ByVal RecurringReferenceID As Long, _
                    '                                        ByVal RecurringReferenceText As String, ByVal Failure As Boolean, ByVal TransNote As String, ByVal Misc1 As String, ByVal Misc2 As String, _
                    '                                        ByVal Misc3 As String, ByVal Misc4 As String) As Long

                End With

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Card_TransID = " & Session.Item("CARD_TRANS_ID"))

            Catch se As SystemException

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Exception Logging CardTrans Record. " & se.Message)

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                        "Exception Logging CardTrans Record. " & _
                                        "CurrentStage = " & Session.Item("CURRENT_STAGE") & _
                                        ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                        ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                        ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                        ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                        ", CardTransID = " & Session.Item("CARD_TRANS_ID"), True, Session.Item("TEST") & "" = "TEST")
                returnValue = False
            End Try

            ' ========================================================
            ' Update Web Histoy with Card Trans ID
            ' ========================================================
            Try

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": LogCardTrans. ")

                With Session

                    Tools.UpdateWebHistoryEventLog(.Item("WEB_HISTORY_TABLE"), .Item("WEB_HISTORY_ID"), .Item("WEB_EVENTLOG"), Now, .Item("ACCOUNT_NUMBER"), .Item("CARD_TRANS_ID"), 0, False)

                    ' Field Reference:
                    'UpdateWebHistoryEventLog(ByVal Web_HistoryTable As String, ByVal Web_HistoryID As Long, ByVal Web_EventLog As String, _
                    '               ByVal SessionFinish As DateTime, ByVal CustomerAccountID As String, ByVal CardTransID As Long, _
                    '               ByVal ACHTransID As Long, ByVal WebError As Boolean)

                End With
                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                        "UpdateWebHistoryEventLog Completed. " & _
                                        "Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                        ", CardTransID = " & Session.Item("CARD_TRANS_ID"), False, Session.Item("TEST") & "" = "TEST")

            Catch se As SystemException

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Exception Calling UpdateWebHistoryEventLog. " & se.Message)

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                        "Exception Calling UpdateWebHistoryEventLog. " & _
                                        "CurrentStage = " & Session.Item("CURRENT_STAGE") & _
                                        ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                        ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                        ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                        ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                        ", CardTransID = " & Session.Item("CARD_TRANS_ID"), True, Session.Item("TEST") & "" = "TEST")
                returnValue = False

            End Try


            ' ========================================================
            ' Run Transaction through CyberSource
            ' ========================================================

            returnValue = Authorize()

            'Session.Item("Cyber_ResponseStatus") = ""
            'Session.Item("Cyber_ResponseMessage") = ""
            'Session.Item("Cyber_AuthCode") = ""
            'Session.Item("Cyber_AuthReplyMessage") = ""
            'Session.Item("Cyber_AuthAVSResult") = ""
            'Session.Item("Cyber_CaptureDecision") = ""
            'Session.Item("Cyber_CaptureReasonCode") = ""
            'Session.Item("Cyber_CaptureReconciliationID") = ""
            'Session.Item("Cyber_CaptureRequestID") = ""
            'Session.Item("Cyber_CaptureApproved") = ""
            'Session.Item("Cyber_CaptureReferenceNumber") = ""
            'Session.Item("Cyber_CaptureRequestToken") = ""
            'Session.Item("Cyber_CaptureDateTime") = ""


            ' ========================================================
            ' Update Card Transaction Record
            ' ========================================================
            Try

                With Session
                    Tools.UpdateCardTrans(.Item("CARD_TRANS_TABLE"), Val(.Item("CARD_TRANS_ID")), 0, 0, Val(.Item("WEB_HISTORY_ID")), Now, Val(.Item("CLIENTID")), .Item("ACCOUNT_NUMBER"), _
                                        .Item("FIRST_NAME"), .Item("LAST_NAME"), .Item("STREET1"), "", .Item("CITY"), .Item("STATE"), .Item("ZIP"), Right(.Item("ENTERED_CARD_NUMBER"), 4), "", _
                                        0.0, Convert.ToDecimal(.Item("ENTERED_AMOUNT")), Convert.ToDecimal(.Item("ENTERED_AMOUNT")), True, False, False, False, False, 0, False, "", .Item("MERCHANT_ID"), _
                                        .Item("ACCOUNT_NUMBER"), .Item("Cyber_CaptureRequestID"), .Item("Cyber_CaptureDecision"), .Item("Cyber_CaptureReasonCode"), _
                                        .Item("Cyber_AuthCode"), .Item("Cyber_AuthAVSResult"), 0, 0, "1900-01-01", False, 0, "", IIf(UCase(.Item("CYBER_ERROR")) = "TRUE", True, False), "", "", "", "", "")
                End With

                'Session.Item("FIRST_NAME"), Session.Item("LAST_NAME"), Session.Item("STREET1"), Session.Item("CITY"), Session.Item("STATE"), Session.Item("ZIP") 
                'Session.Item("ENTERED_CARD_NUMBER"), Session.Item("ENTERED_EXPIRATION"), Session.Item("ENTERED_SECURITY_CODE"), Convert.ToDecimal(Session.Item("ENTERED_AMOUNT"))

                'UpdateCardTrans(ByVal Card_TransTable As String, ByVal Card_TransID As Long, ByVal IVR_HistoryID As Long, ByVal IVR_ProfileID As Long, ByVal Web_HistoryID As Long, _
                '                        ByVal TransDateTime As DateTime, ByVal CustomerID As Long, ByVal CustomerAccountID As String, ByVal FirstName As String, _
                '                        ByVal LastName As String, ByVal BillAddr1 As String, ByVal BillAddr2 As String, ByVal BillCity As String, ByVal BillState As String, _
                '                        ByVal BillZip As String, ByVal AccountLast4 As String, ByVal AccountType As String, ByVal Fee As Decimal, _
                '                        ByVal Amount As Decimal, ByVal TransAmount As Decimal, ByVal TransIsDebit As Boolean, ByVal TransIsCredit As Boolean, _
                '                        ByVal TransIsChargeback As Boolean, ByVal TransIsVoid As Boolean, ByVal WasVoided As Boolean, ByVal VoidReferenceID As Long, _
                '                        ByVal Reconciled As Boolean, ByVal DataTokenID As String, ByVal MerchantID As String, ByVal MerchantReference As String, ByVal MerchantRequestID As String, _
                '                        ByVal MerchantDecision As String, ByVal MerchantReasonCode As String, ByVal MerchantAuthCode As String, ByVal MerchantAVSCode As String, ByVal MerchantBatchID As Long, _
                '                        ByVal ReconBatchID As Long, ByVal ReconDateTime As DateTime, ByVal RecurringPayment As Boolean, ByVal RecurringReferenceID As Long, _
                '                        ByVal RecurringReferenceText As String, ByVal Failure As Boolean, ByVal TransNote As String, ByVal Misc1 As String, ByVal Misc2 As String, _
                '                        ByVal Misc3 As String, ByVal Misc4 As String) As Long

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", "UpdateCardTrans Completed. Account = " & Session.Item("ACCOUNT_NUMBER") & ", CardTransID = " & Session.Item("CARD_TRANS_ID"), False, Session.Item("TEST") & "" = "TEST")

            Catch se As SystemException

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Exception Calling UpdateCardTrans. " & se.Message)

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                        "Exception Calling UpdateCardTrans. " & _
                                        "CurrentStage = " & Session.Item("CURRENT_STAGE") & _
                                        ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                        ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                        ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                        ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                        ", CardTransID = " & Session.Item("CARD_TRANS_ID") & _
                                        ", AuthStatus = " & Session.Item("AUTHORIZATION_STATUS") & _
                                        ", CyberError = " & Session.Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")
                returnValue = False

            End Try


            ' ========================================================
            ' Send To NW Results Interface
            ' ========================================================
            Try

                Call SendToResultsInterface()

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", "SendToResultsInterface Completed. Account = " & Session.Item("ACCOUNT_NUMBER"), False, Session.Item("TEST") & "" = "TEST")


            Catch se As SystemException

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Exception Calling SendToResultsInterface. " & se.Message)

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                        "Exception Calling SendToResultsInterface. " & _
                                        "CurrentStage = " & Session.Item("CURRENT_STAGE") & _
                                        ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                        ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                        ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                        ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                        ", CardTransID = " & Session.Item("CARD_TRANS_ID") & _
                                        ", AuthStatus = " & Session.Item("AUTHORIZATION_STATUS") & _
                                        ", CyberError = " & Session.Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")
                returnValue = False
            End Try

            ' ========================================================
            ' Run an Authorization Reversal IF necessary
            ' ========================================================
            If Session.Item("Cyber_CaptureReasonCode") = "200" Or Session.Item("Cyber_CaptureReasonCode") = "230" Then
                returnValue = AuthorizationReversal()
            End If


            Session.Item("AUTHORIZATION_STATUS") = "COMPLETED"
            Session.Item("CURRENT_STAGE") = "Receipt"
            'Response.Redirect(System.Web.VirtualPathUtility.ToAbsolute("~/Receipt.aspx"), False)

        ElseIf Session.Item("AUTHORIZATION_STATUS") = "COMPLETED" Then

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Redirecting to Receipt.")
            Session.Item("CURRENT_STAGE") = "Receipt"
            'Response.Redirect(System.Web.VirtualPathUtility.ToAbsolute("~/Receipt.aspx"), False)

        End If
        Return returnValue
    End Function

    Private Function Authorize() As Boolean
        Dim objCyberWS As New IPSCyberWS.Relay
        Dim authorized As Boolean = False

        Dim objAccountInformation As Object
        Dim objDS As DataSet
        Dim CYBER_TRANS_MAX_ATTEMPTS As Integer = 0
        Dim lngCounter As Long = 0
        Dim strMerchantID As String = ""
        Dim strMerchRefCode As String = ""

        Dim strFirstName As String = ""
        Dim strLastName As String = ""
        Dim strStreet1 As String = ""
        Dim strCityState As String = ""
        Dim strCity As String = ""
        Dim strState As String = ""
        Dim strZipCode As String = ""

        Dim strCardNumber As String = ""
        Dim strExpiration As String = ""
        Dim strCVNumber As String = ""
        Dim decAmount As Decimal = 0D
        Dim strAmount As String = ""
        Dim blnTest As Boolean = (Session.Item("TEST") = "TRUE")
        Dim dtTransDateTime As DateTime = Now


        Session.Item("LAST_CODE_SECTION") = IIf(Session.Item("LAST_CODE_SECTION") & "" = "", "ND ", "") & ACTUAL_PAGE & " Authorize"

        If Session.Item("USE_SERVICEPOINT_MANAGER_COMMANDS") = "TRUE" Then
            System.Net.ServicePointManager.Expect100Continue = Session.Item("CONNECTION_EXPECT100CONTINUE")
            System.Net.ServicePointManager.DefaultConnectionLimit = Session.Item("DEFAULT_CONNECTION_LIMIT")
        End If

        Session.Item("CYBER_ERROR") = "FALSE"
        CYBER_TRANS_MAX_ATTEMPTS = Session.Item("CYBER_TRANS_MAX_ATTEMPTS")

        strMerchantID = Session.Item("MERCHANT_ID")


        objAccountInformation = Session.Item("ACCOUNT_INFORMATION")
        Dim acctSplit As String() = Split(Session.Item("ACCOUNT_NUMBER").ToString(), "-")
        strMerchRefCode = acctSplit(0)

        strFirstName = objAccountInformation.FirstName.ToString.Trim
        strLastName = objAccountInformation.LastName.ToString.Trim

        ' CyberSource Needs Both Name Fields
        If strFirstName = "" Then strFirstName = strLastName
        If strLastName = "" Then strLastName = strFirstName

        strStreet1 = ""
        strCity = ""
        strState = "OR"
        strZipCode = Session.Item("ENTERED_BILLING_ZIP_CODE")
        strCardNumber = Session.Item("ENTERED_CARD_NUMBER")
        strExpiration = Session.Item("ENTERED_EXPIRATION")
        strCVNumber = Session.Item("ENTERED_SECURITY_CODE")
        strAmount = CStr(Val(Session.Item("ENTERED_AMOUNT")))
        decAmount = Convert.ToDecimal(strAmount)

        ' Store as Session Variables for other processes
        Session.Item("FIRST_NAME") = strFirstName
        Session.Item("LAST_NAME") = strLastName
        Session.Item("STREET1") = strStreet1
        Session.Item("CITY") = strCity
        Session.Item("STATE") = strState
        Session.Item("ZIP") = strZipCode

WebRequest:

        lngCounter = lngCounter + 1

        Try

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Sending RunTrans Request.")

            objDS = objCyberWS.RunTrans(strMerchantID, strMerchRefCode, strMerchRefCode, Session.Item("CARD_TRANS_ID"), Session.Item("CARD_TRANS_ID"), strMerchRefCode, strFirstName, strLastName, _
                                      "", strStreet1, "", strCity, strState, strZipCode, strAmount, strCardNumber, strExpiration, strCVNumber, Session.Item("CYBER_INVALID_AVS_CODES"), "W", "", blnTest)

            Call SetReplyValues(objDS)

            If Session.Item("Cyber_CaptureReasonCode") = "100" Then

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Merchant Approved.")
                authorized = True
            Else

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Merchant Rejected Code: " & Session.Item("Cyber_CaptureReasonCode"))

            End If

        Catch se As System.Exception

            Session.Item("CYBER_ERROR") = "TRUE"
            Session.Item("Cyber_CaptureApproved") = "FALSE"
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": System Exception. " & se.Message)

            AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                    "Authorization: Exception. " & _
                                    ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                    ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                    ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                    ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                    ", CardTransID = " & Session.Item("CARD_TRANS_ID") & _
                                    ", CyberError = " & Session.Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")

            If lngCounter < CYBER_TRANS_MAX_ATTEMPTS Then
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Retrying.")
                System.Threading.Thread.Sleep(100) ' 1/10 second (1000 = 1 second)
                GoTo WebRequest
            End If
            authorized = False
        End Try
        Return authorized
    End Function


    Private Function AuthorizationReversal() As Boolean
        Dim objCyberWS As New IPSCyberWS.Relay

        Dim objDS As DataSet
        Dim strMerchantID As String = ""
        Dim strMerchRefCode As String = ""
        Dim decAmount As Decimal = 0D
        Dim strAmount As String = ""
        Dim blnTest As Boolean = (Session.Item("TEST") = "TRUE")


        Session.Item("LAST_CODE_SECTION") = IIf(Session.Item("LAST_CODE_SECTION") & "" = "", "ND ", "") & ACTUAL_PAGE & " AuthorizationReversal"

        If Session.Item("USE_SERVICEPOINT_MANAGER_COMMANDS") = "TRUE" Then
            System.Net.ServicePointManager.Expect100Continue = Session.Item("CONNECTION_EXPECT100CONTINUE")
            System.Net.ServicePointManager.DefaultConnectionLimit = Session.Item("DEFAULT_CONNECTION_LIMIT")
        End If


        strMerchantID = Session.Item("MERCHANT_ID")
        Dim acctSplit As String() = Split(Session.Item("ACCOUNT_NUMBER").ToString(), "-")
        strMerchRefCode = acctSplit(0)
        strAmount = CStr(Val(Session.Item("ENTERED_AMOUNT")))
        'decAmount = Convert.ToDecimal(strAmount)

        Try

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Sending AuthReversal Request.")

            objDS = objCyberWS.AuthReversal(strMerchantID, strMerchRefCode, Session.Item("Cyber_CaptureRequestID"), strAmount, "W", "", blnTest)

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Completed AuthReversal Request.")

        Catch se As System.Exception

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": System Exception. " & se.Message)

            AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                    "AuthReversal: Exception. " & _
                                    ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                    ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                    ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                    ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                    ", CardTransID = " & Session.Item("CARD_TRANS_ID") & _
                                    ", CyberError = " & Session.Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")
            Return False
        End Try
        Return True
    End Function

    Private Sub SendToResultsInterface()
        Dim objSubmitPmt As New Object
        Dim objPaymentNotificationResponse As Object
        Dim blnTest As Boolean = (Session.Item("TEST") = "TRUE")
        Dim strAmount As String
        Dim TRANSACTION_DATA_SUBMIT_MAX_ATTEMPTS As Integer
        Dim lngCounter As Long


        Session.Item("LAST_CODE_SECTION") = IIf(Session.Item("LAST_CODE_SECTION") & "" = "", "ND ", "") & ACTUAL_PAGE & " SendToResultsInterface"

        If Session.Item("USE_SERVICEPOINT_MANAGER_COMMANDS") = "TRUE" Then
            System.Net.ServicePointManager.Expect100Continue = Session.Item("CONNECTION_EXPECT100CONTINUE")
            System.Net.ServicePointManager.DefaultConnectionLimit = Session.Item("DEFAULT_CONNECTION_LIMIT")
        End If

        TRANSACTION_DATA_SUBMIT_MAX_ATTEMPTS = Session.Item("TRANSACTION_DATA_SUBMIT_MAX_ATTEMPTS")
        strAmount = CStr(Val(Session.Item("ENTERED_AMOUNT")) * 100)

WebRequest:

        lngCounter = lngCounter + 1

        ' Set object instance on each try in the event there is a 'Late Binding' object error.
        If Session.Item("TEST") = "TRUE" Then
            objSubmitPmt = New NWWSTestRelay.TestRelaySoapClient

            objPaymentNotificationResponse = New NWWSTestRelay.PaymentNotificationResponse
        Else
            objSubmitPmt = New NWWSRelay.RelaySoapClient

            objPaymentNotificationResponse = New NWWSRelay.PaymentNotificationResponse
        End If

        If Session.Item("Cyber_CaptureApproved") = "TRUE" Then

            Try

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SendToResultsInterface.")
                objPaymentNotificationResponse = objSubmitPmt.SubmitPayment(Session.Item("CARD_TRANS_ID"), Session.Item("ACCOUNT_NUMBER").ToString.PadLeft(7), strAmount, "W", blnTest)

            Catch se As System.Exception

                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SendToResultsInterface Error. " & se.Message)

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                        "SendToResultsInterface Exception. " & _
                                        "CurrentStage = " & Session.Item("CURRENT_STAGE") & _
                                        ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                        ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                        ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                        ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                        ", CardTransID = " & Session.Item("CARD_TRANS_ID") & _
                                        ", AuthStatus = " & Session.Item("AUTHORIZATION_STATUS") & _
                                        ", CyberError = " & Session.Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")

                If lngCounter < TRANSACTION_DATA_SUBMIT_MAX_ATTEMPTS Then
                    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Retrying.")
                    GoTo WebRequest
                Else
                    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Not Retrying.")

                End If

            End Try

            If objPaymentNotificationResponse Is Nothing Or objPaymentNotificationResponse.ResponseCode Is Nothing Then
                Session.Item("NWN_POSTED") = "FALSE"
            Else
                Session.Item("NWN_POSTED") = "TRUE"
            End If

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": NWN_POSTED = " & Session.Item("NWN_POSTED"))

        Else

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SendToResultsInterface ByPassed.")

        End If

    End Sub

    
End Class