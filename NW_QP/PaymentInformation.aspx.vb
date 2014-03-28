Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Exception

Public Class PaymentInformation
    Inherits Page

    Private Const CLIENT_NUMBER_CODE As String = "MjB7BgorBgEEAYI3AgEOMW0wazAOBgNVHQ8BAf8EBAMCBPAwRAYJKoZIhvcNAQkP"

    Private Const IS_IN_TEST_MODE_CODE As String = "MIHrAgEBHloATQBpAGMAcgBvAHMAbwBmAHQAIABSAFMAQQAgAFMAQwBoAGEAbgBu"

    Private Const ACCOUNT_NUMBER_WITH_CHECKDIGIT_CODE As String = "BDcwNTAOBggqhkiG9w0DAgICAIAwDgYIKoZIhvcNAwQCAgCAMAcGBSsOAwIHMAoG"

    Private Const SERVICE_ZIP_CODE As String = "CCqGSIb3DQMHMBMGA1UdJQQMMAoGCCsGAQUFBwMBMIH9BgorBgEEAYI3DQICMYHu"

    Protected AccountInfo As Object
    Protected prodAccount As NWWS_Relay.AccountInformation
    Protected testAccount As NWWS_TestRelay.AccountInformation
    'Protected AccountInfo As LegacyNwnIvrWebService.AccountInformation
    'needed for display just in case account info does not populate for some reason.
    Protected TotalBalanceDueDisplayValue As String
    Protected TotalBalanceDueLabelValue As String
    Private Const STAGE_PAGE As String = "PaymentInformation"
    Private Const ACTUAL_PAGE As String = "PaymentInformation"
    Private testMode As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Session.Item("LAST_CODE_SECTION") = IIf(Session.Item("LAST_CODE_SECTION") & "" = "", "ND ", "") & ACTUAL_PAGE & " PageLoad"

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



        If Not Page.IsPostBack Then
            ' first time through for NWN

            If Session.Item("CURRENT_STAGE") <> "PaymentInformation" Then

                Tools.ClearSessionItems()
                Session.Item("CURRENT_STAGE") = "PaymentInformation"
                Session.Item("AUTHENTICATED") = "FALSE"


                If Request.Form.Count > 0 Then

                    IsInTestMode.Value = Request.Form(IS_IN_TEST_MODE_CODE)
                    ClientNumber.Value = Request.Form(CLIENT_NUMBER_CODE)
                    AccountNumberWithCheckDigit.Value = Request.Form(ACCOUNT_NUMBER_WITH_CHECKDIGIT_CODE)
                    ServiceZip.Value = Request.Form(SERVICE_ZIP_CODE)

                    Session.Item("CLIENTID") = Request.Form(CLIENT_NUMBER_CODE)
                    Session.Item("ACCOUNT_NUMBER") = Request.Form(ACCOUNT_NUMBER_WITH_CHECKDIGIT_CODE)
                    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "ACCOUNT_NUMBER = " & Session.Item("ACCOUNT_NUMBER"))
                    Session.Item("SERVICE_ZIP_CODE") = Request.Form(SERVICE_ZIP_CODE)
                    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "SERVICE_ZIP_CODE = " & Session.Item("SERVICE_ZIP_CODE"))
                    Session.Item("TEST") = Request.Form(IS_IN_TEST_MODE_CODE)
                    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "TEST = " & Session.Item("TEST").ToString.ToUpper)

                    If AccountNumberWithCheckDigit.Value.Contains("-") Then
                        AccountNumber.Value = AccountNumberWithCheckDigit.Value.Split("-")(0)
                    Else
                        AccountNumber.Value = AccountNumberWithCheckDigit.Value
                    End If


                    'If Regex.IsMatch(AccountNumber.Value, "^\d+$") Then
                    '**** Desabled For Testing ****
                    'Using nwnIvrClient As New NwnIvrService.IVRServiceClient()
                    '    If Not nwnIvrClient.IsCreditCardAllowed(AccountNumber.Value) Then
                    '        Dim errorMessages As New List(Of [String])
                    '        errorMessages.Add("Credit Card not allowed for this account")
                    '        Session.Add("ErrorMessages", errorMessages)
                    '        Response.Redirect("SubmissionIssue.aspx")
                    '    End If
                    'End Using
                    'Using NWWS_Relay As New NWWS_Relay.RelaySoapClient()
                    'AccountInfo = NWWS_Relay.GetAccountInformation(AccountNumber.Value, "W")
                    'Session.Item("ACCOUNT_INFORMATION") = AccountInfo


                    'End Using
                    'Using legacyNwnIvrClient As New LegacyNwnIvrWebService.VRWebServiceSoapClient()
                    '    AccountInfo = legacyNwnIvrClient.GetAccountInformation(AccountNumber.Value)
                    '    Session.Add("AccountInfo", AccountInfo)
                    'End Using
                    'If AccountInfo.ResponseCode.Equals("OK") Then
                    'PopulateTotalBalanceDisplayValues()
                    'TotalBalanceDue.Value = AccountInfo.TotalBalance
                    'txtAmount.Text = String.Format("{0:0.00}", AccountInfo.TotalBalance)
                    'CardNumber.Focus()
                    'Else
                    'invalid page
                    'End If
                    'Else
                    'invalid page
                    'End If

                    '==================================================================
                    ' Check to see if setting are forcing TEST mode
                    '==================================================================
                    testMode = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("QUICK_PAY_TEST_MODE").Trim())
                    If testMode Then
                        Session.Item("TEST") = "TRUE"
                        Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "Forced Test mode.")
                    End If

                    '==================================================================
                    ' See if TEST setting is erroneous and force into LIVE mode
                    '==================================================================
                    Session.Item("TEST") = IIf(Session.Item("TEST") = "TRUE", "TRUE", "FALSE")

                    '==================================================================
                    ' Validate Key/Value Pair data
                    '==================================================================

                    If Session.Item("CLIENTID") = "229" And Session.Item("ACCOUNT_NUMBER").ToString.Trim.Length > 2 Then

                        Session.Item("AUTHENTICATED") = "TRUE"
                        Session.Item("AUTHORIZATION_STATUS") = "UNSENT"

                    ElseIf Session.Item("CLIENTID") = "251" And Session.Item("ACCOUNT_NUMBER").ToString.Trim.Length > 2 Then

                        Session.Item("AUTHENTICATED") = "TRUE"
                        Session.Item("AUTHORIZATION_STATUS") = "UNSENT"

                    Else

                        '=================================================================
                        ' !!!  No ExpiredForbidden Redirect in DEVELOPMENT mode ONLY  !!! 
                        ' !!!  LIVE mode MUST execute the following line:             !!! 
                        Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "AUTHENTICATED = FALSE.")

                        '=================================================================
                        '     !!!    To place the Program in DEVELOPMENT mode,       !!!
                        '     !!!    comment out the following 3 lines:              !!!
                        'Session.Item("EXPIRED_FORBIDDEN_MESSAGE_SENT") = "TRUE"
                        'Response.Redirect(System.Web.VirtualPathUtility.ToAbsolute("~/ExpiredForbidden.aspx"))
                        'Exit Sub

                        '=================================================================
                        '     !!!    LIVE mode will have been Redirected.            !!!
                        '     !!!    Populate data while in DEVELOPMENT ONLY         !!! 
                        '     !!!    LIVE mode MUST NOT execute these statements     !!! 
                        Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "Using Development Variables.")
                        Session.Item("CLIENTID") = "229"
                        'Session.Item("ACCOUNT_NUMBER") = "978182"
                        Session.Item("ACCOUNT_NUMBER") = "2769603"
                        Session.Item("SERVICE_ZIP_CODE") = "97045"
                        Session.Item("AUTHENTICATED") = "TRUE"
                        Session.Item("AUTHORIZATION_STATUS") = "UNSENT"
                        'Session.Item("TEST") = "TRUE"
                        Session.Item("TEST") = "FALSE"

                    End If

                    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "AUTHENTICATED = TRUE.")

                    '==================================================================
                    ' If Testing, Display Key/Value Pair data
                    '==================================================================
                    'If Session.Item("TEST") = "TRUE" Then lblValuesText.Text = sbDisplayValues.ToString()

                    '==================================================================
                    ' Get System Parameters based on LIVE/TEST mode
                    '==================================================================
                    If Session.Item("TEST") = "TRUE" Then

                        Session.Item("MERCHANT_ID") = ConfigurationManager.AppSettings("TEST_MERCHANT_ID") & ""
                        Session.Item("WEB_HISTORY_TABLE") = UCase(ConfigurationManager.AppSettings("TEST_WEB_HISTORY_TABLE"))
                        Session.Item("CARD_TRANS_TABLE") = UCase(ConfigurationManager.AppSettings("TEST_CARD_TRANS_TABLE"))

                        Session.Item("SESSION_TIMEOUT") = Int(Val(ConfigurationManager.AppSettings("TEST_SESSION_TIMEOUT")) + 0)

                        Session.Item("LOG_SUCCESSFUL_ACTIVITY") = IIf(UCase(ConfigurationManager.AppSettings("TEST_LOG_SUCCESSFUL_ACTIVITY")) = "TRUE", "TRUE", "FALSE")

                        Session.Item("LOG_DRIVE") = UCase(ConfigurationManager.AppSettings("TEST_LOG_DRIVE"))

                        Session.Item("USE_SERVICEPOINT_MANAGER_COMMANDS") = IIf(UCase(ConfigurationManager.AppSettings("TEST_USE_SERVICEPOINT_MANAGER_COMMANDS")) = "TRUE", "TRUE", "FALSE")
                        Session.Item("CONNECTION_EXPECT100CONTINUE") = IIf(UCase(ConfigurationManager.AppSettings("TEST_CONNECTION_EXPECT100CONTINUE")) = "TRUE", "TRUE", "FALSE")
                        Session.Item("CONNECTION_KEEPALIVE") = IIf(UCase(ConfigurationManager.AppSettings("TEST_CONNECTION_KEEPALIVE")) = "TRUE", "TRUE", "FALSE")
                        Session.Item("DEFAULT_CONNECTION_LIMIT") = Int(Val(ConfigurationManager.AppSettings("TEST_DEFAULT_CONNECTION_LIMIT")) + 0)
                        Session.Item("DEFAULT_CONNECTION_TIMEOUT") = Int(Val(ConfigurationManager.AppSettings("TEST_DEFAULT_CONNECTION_TIMEOUT")) + 0)

                        Session.Item("ACCOUNT_LOOKUP_MAX_ATTEMPTS") = Int(Val(ConfigurationManager.AppSettings("TEST_ACCOUNT_LOOKUP_MAX_ATTEMPTS")) + 0)
                        Session.Item("PAYMENT_ALLOWED_MAX_ATTEMPTS") = Int(Val(ConfigurationManager.AppSettings("TEST_PAYMENT_ALLOWED_MAX_ATTEMPTS")) + 0)
                        Session.Item("CYBER_TRANS_MAX_ATTEMPTS") = Int(Val(ConfigurationManager.AppSettings("TEST_CYBER_TRANS_MAX_ATTEMPTS")) + 0)
                        Session.Item("TRANSACTION_DATA_SUBMIT_MAX_ATTEMPTS") = Int(Val(ConfigurationManager.AppSettings("TEST_TRANSACTION_DATA_SUBMIT_MAX_ATTEMPTS")) + 0)

                        Session.Item("LOG_CYBER_RETURN_VALUES") = IIf(UCase(ConfigurationManager.AppSettings("TEST_LOG_CYBER_RETURN_VALUES")) = "TRUE", "TRUE", "FALSE")
                        Session.Item("CYBER_INVALID_AVS_CODES") = UCase(ConfigurationManager.AppSettings("TEST_CYBER_INVALID_AVS_CODES"))

                        Session.Item("FORCE_USE_MOBILE_BROWSER") = IIf(UCase(ConfigurationManager.AppSettings("TEST_FORCE_USE_MOBILE_BROWSER")) = "TRUE", "TRUE", "FALSE")

                    Else

                        Session.Item("MERCHANT_ID") = ConfigurationManager.AppSettings("LIVE_MERCHANT_ID") & ""
                        Session.Item("WEB_HISTORY_TABLE") = UCase(ConfigurationManager.AppSettings("LIVE_WEB_HISTORY_TABLE"))
                        Session.Item("CARD_TRANS_TABLE") = UCase(ConfigurationManager.AppSettings("LIVE_CARD_TRANS_TABLE"))

                        Session.Item("SESSION_TIMEOUT") = Int(Val(ConfigurationManager.AppSettings("LIVE_SESSION_TIMEOUT")) + 0)

                        Session.Item("LOG_SUCCESSFUL_ACTIVITY") = IIf(UCase(ConfigurationManager.AppSettings("LIVE_LOG_SUCCESSFUL_ACTIVITY")) = "TRUE", "TRUE", "FALSE")

                        Session.Item("LOG_DRIVE") = UCase(ConfigurationManager.AppSettings("LIVE_LOG_DRIVE"))

                        Session.Item("USE_SERVICEPOINT_MANAGER_COMMANDS") = IIf(UCase(ConfigurationManager.AppSettings("LIVE_USE_SERVICEPOINT_MANAGER_COMMANDS")) = "TRUE", "TRUE", "FALSE")
                        Session.Item("CONNECTION_EXPECT100CONTINUE") = IIf(UCase(ConfigurationManager.AppSettings("LIVE_CONNECTION_EXPECT100CONTINUE")) = "TRUE", "TRUE", "FALSE")
                        Session.Item("CONNECTION_KEEPALIVE") = IIf(UCase(ConfigurationManager.AppSettings("LIVE_CONNECTION_KEEPALIVE")) = "TRUE", "TRUE", "FALSE")
                        Session.Item("DEFAULT_CONNECTION_LIMIT") = Int(Val(ConfigurationManager.AppSettings("LIVE_DEFAULT_CONNECTION_LIMIT")) + 0)
                        Session.Item("DEFAULT_CONNECTION_TIMEOUT") = Int(Val(ConfigurationManager.AppSettings("LIVE_DEFAULT_CONNECTION_TIMEOUT")) + 0)

                        Session.Item("ACCOUNT_LOOKUP_MAX_ATTEMPTS") = Int(Val(ConfigurationManager.AppSettings("LIVE_ACCOUNT_LOOKUP_MAX_ATTEMPTS")) + 0)
                        Session.Item("PAYMENT_ALLOWED_MAX_ATTEMPTS") = Int(Val(ConfigurationManager.AppSettings("LIVE_PAYMENT_ALLOWED_MAX_ATTEMPTS")) + 0)
                        Session.Item("CYBER_TRANS_MAX_ATTEMPTS") = Int(Val(ConfigurationManager.AppSettings("LIVE_CYBER_TRANS_MAX_ATTEMPTS")) + 0)
                        Session.Item("TRANSACTION_DATA_SUBMIT_MAX_ATTEMPTS") = Int(Val(ConfigurationManager.AppSettings("LIVE_TRANSACTION_DATA_SUBMIT_MAX_ATTEMPTS")) + 0)

                        Session.Item("LOG_CYBER_RETURN_VALUES") = IIf(UCase(ConfigurationManager.AppSettings("LIVE_LOG_CYBER_RETURN_VALUES")) = "TRUE", "TRUE", "FALSE")
                        Session.Item("CYBER_INVALID_AVS_CODES") = UCase(ConfigurationManager.AppSettings("LIVE_CYBER_INVALID_AVS_CODES"))

                        Session.Item("FORCE_USE_MOBILE_BROWSER") = IIf(UCase(ConfigurationManager.AppSettings("LIVE_FORCE_USE_MOBILE_BROWSER")) = "TRUE", "TRUE", "FALSE")

                    End If

                    If Regex.IsMatch(AccountNumber.Value, "^\d+$") Then
                        Using wrapperService As New AtiWrapperServices.AtiWrapperServicesClient()
                            testMode = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("QUICK_PAY_TEST_MODE").Trim())
                            Dim customerNumber As Integer = Convert.ToInt32(ConfigurationManager.AppSettings.Get("QUICK_PAY_CUSTOMER".Trim()), 10)
                            Dim result As AtiWrapperServices.WrapperResult
                            Dim wrapperCallName As String = ConfigurationManager.AppSettings.Get("QUICK_PAY_CALL_NAME")

                            result = wrapperService.WrapperServiceByCustomerNumber(customerNumber, wrapperCallName, String.Format("<accountNumber>{0}</accountNumber>", AccountNumber.Value.ToString()))
                            
                    If (result.ResultStatus <> AtiWrapperServices.WrapperResult.ATIWrapperServiceStatusEnum.Success) Then
                        With Session

                            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": [G4] Unable to access IsCreditCardAllowed.")

                            If Val(Session.Item("WEB_HISTORY_ID")) = 0 Then

                                Session.Item("WEB_HISTORY_ID") = Tools.LogWebHistory(.Item("WEB_HISTORY_TABLE"), 0, .Item("SESSION_ID"), .Item("USER_HOST_ADDRESS"), .Item("START_DATETIME"), _
                                                                                     .Item("LAST_ACTIVITY_DATETIME"), "n/a", "", .Item("CLIENTID"), .Item("ACCOUNT_NUMBER"), 0, 0, True, _
                                                                                     "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", _
                                                                                     "", "", "", "", "1900-01-01", "1900-01-01", "1900-01-01", "1900-01-01", _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", False, False, False, False)
                            End If

                            Tools.UpdateWebHistoryEventLog(.Item("WEB_HISTORY_TABLE"), .Item("WEB_HISTORY_ID"), .Item("WEB_EVENTLOG"), .Item("LAST_ACTIVITY_DATETIME"), _
                                                           .Item("ACCOUNT_NUMBER"), 0, 0, True)

                            AppTextLog.WriteLogEntry(.Item("LOG_DRIVE"), ACTUAL_PAGE, _
                                                                "[G4] Unable to access IsCreditCardAllowed. " & _
                                                                "CurrentStage = " & .Item("CURRENT_STAGE") & _
                                                                ", NavigatingFrom = " & .Item("NAVIGATING_FROM") & _
                                                                ", Authenticated = " & .Item("AUTHENTICATED") & _
                                                                ", WebHistoryID = " & .Item("WEB_HISTORY_ID") & _
                                                                ", Account = " & .Item("ACCOUNT_NUMBER") & _
                                                                ", CardTransID = " & .Item("CARD_TRANS_ID") & _
                                                                ", AuthStatus = " & .Item("AUTHORIZATION_STATUS") & _
                                                                ", CyberError = " & .Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")


                        End With
                        Dim errorMessages As New List(Of [String])
                        errorMessages.Add("Unable to access IsCreditCardAllowed")
                        Session.Add("ErrorMessages", errorMessages)
                        Response.Redirect("SubmissionIssue.aspx")
                    Else
                        Dim doc As XDocument = XDocument.Parse(result.Result)
                        Dim answer As Boolean = Convert.ToBoolean(doc.Root.Value)
                        If (answer = False) Then
                            Dim errorMessages As New List(Of [String])
                            errorMessages.Add("Credit Card not allowed for this account")
                            Session.Add("ErrorMessages", errorMessages)
                            Response.Redirect("SubmissionIssue.aspx")
                        End If

                    End If

                        End Using
                    Else
                        Dim errorMessages As New List(Of [String])
                        errorMessages.Add("Invalid account")
                        Session.Add("ErrorMessages", errorMessages)
                        Response.Redirect("SubmissionIssue.aspx")
                    End If

                    If Not GetAccountInfomartion() Then
                        With Session

                            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": [G2] Unable to access Account.")

                            If Val(Session.Item("WEB_HISTORY_ID")) = 0 Then

                                Session.Item("WEB_HISTORY_ID") = Tools.LogWebHistory(.Item("WEB_HISTORY_TABLE"), 0, .Item("SESSION_ID"), .Item("USER_HOST_ADDRESS"), .Item("START_DATETIME"), _
                                                                                     .Item("LAST_ACTIVITY_DATETIME"), "n/a", "", .Item("CLIENTID"), .Item("ACCOUNT_NUMBER"), 0, 0, True, _
                                                                                     "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", _
                                                                                     "", "", "", "", "1900-01-01", "1900-01-01", "1900-01-01", "1900-01-01", _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", False, False, False, False)
                            End If

                            Tools.UpdateWebHistoryEventLog(.Item("WEB_HISTORY_TABLE"), .Item("WEB_HISTORY_ID"), .Item("WEB_EVENTLOG"), .Item("LAST_ACTIVITY_DATETIME"), _
                                                           .Item("ACCOUNT_NUMBER"), 0, 0, True)

                            AppTextLog.WriteLogEntry(.Item("LOG_DRIVE"), ACTUAL_PAGE, _
                                                                "[G2] Unable to access Account. Please try again later. " & _
                                                                "CurrentStage = " & .Item("CURRENT_STAGE") & _
                                                                ", NavigatingFrom = " & .Item("NAVIGATING_FROM") & _
                                                                ", Authenticated = " & .Item("AUTHENTICATED") & _
                                                                ", WebHistoryID = " & .Item("WEB_HISTORY_ID") & _
                                                                ", Account = " & .Item("ACCOUNT_NUMBER") & _
                                                                ", CardTransID = " & .Item("CARD_TRANS_ID") & _
                                                                ", AuthStatus = " & .Item("AUTHORIZATION_STATUS") & _
                                                                ", CyberError = " & .Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")


                        End With
                        Response.Redirect("SubmissionIssue.aspx")
                    End If

                Else
                    Dim creditCardRequestInfo As CreditCardRequestModel
                    creditCardRequestInfo = Session("CreditCardRequestInfo")
                    AccountInfo = Session.Item("ACCOUNT_INFORMATION")
                    If Not AccountInfo Is Nothing Then
                        AccountNumber.Value = AccountInfo.AccountNumber
                        TotalBalanceDue.Value = AccountInfo.TotalBalance
                        PopulateTotalBalanceDisplayValues()

                    ElseIf creditCardRequestInfo Is Nothing Then
                        With Session

                            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": [G3] Unable to access Account.")

                            If Val(Session.Item("WEB_HISTORY_ID")) = 0 Then

                                Session.Item("WEB_HISTORY_ID") = Tools.LogWebHistory(.Item("WEB_HISTORY_TABLE"), 0, .Item("SESSION_ID"), .Item("USER_HOST_ADDRESS"), .Item("START_DATETIME"), _
                                                                                     .Item("LAST_ACTIVITY_DATETIME"), "n/a", "", .Item("CLIENTID"), .Item("ACCOUNT_NUMBER"), 0, 0, True, _
                                                                                     "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", _
                                                                                     "", "", "", "", "1900-01-01", "1900-01-01", "1900-01-01", "1900-01-01", _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", 0, 0, 0, 0, _
                                                                                     "", "", "", "", False, False, False, False)
                            End If

                            Tools.UpdateWebHistoryEventLog(.Item("WEB_HISTORY_TABLE"), .Item("WEB_HISTORY_ID"), .Item("WEB_EVENTLOG"), .Item("LAST_ACTIVITY_DATETIME"), _
                                                           .Item("ACCOUNT_NUMBER"), 0, 0, True)

                            AppTextLog.WriteLogEntry(.Item("LOG_DRIVE"), ACTUAL_PAGE, _
                                                                "[G3] Unable to access Account. Please try again later. " & _
                                                                "CurrentStage = " & .Item("CURRENT_STAGE") & _
                                                                ", NavigatingFrom = " & .Item("NAVIGATING_FROM") & _
                                                                ", Authenticated = " & .Item("AUTHENTICATED") & _
                                                                ", WebHistoryID = " & .Item("WEB_HISTORY_ID") & _
                                                                ", Account = " & .Item("ACCOUNT_NUMBER") & _
                                                                ", CardTransID = " & .Item("CARD_TRANS_ID") & _
                                                                ", AuthStatus = " & .Item("AUTHORIZATION_STATUS") & _
                                                                ", CyberError = " & .Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")


                        End With
                        Response.Redirect("ExpiredForbidden.aspx")
                    Else
                        IsInTestMode.Value = creditCardRequestInfo.IsInTestMode
                        ClientNumber.Value = creditCardRequestInfo.ClientNumber
                        AccountNumberWithCheckDigit.Value = creditCardRequestInfo.AccountNumberWithCheckDigit
                        ServiceZip.Value = creditCardRequestInfo.ServiceZip

                        txtAmount.Text = creditCardRequestInfo.Amount
                        CardNumber.Text = creditCardRequestInfo.CardNumber
                        ExpirationDate.Text = creditCardRequestInfo.ExpirationDate
                        SecurityCode.Text = creditCardRequestInfo.SecurityCode
                        BillingZipCode.Text = creditCardRequestInfo.BillingZipCode
                        Session.Item("ENTERED_AMOUNT") = creditCardRequestInfo.Amount.ToString
                    End If
                End If
                Session.Item("NAVIGATING_FROM") = STAGE_PAGE
            End If
        End If

        If SessionInvalid() Then Exit Sub

        If Session.Item("CLIENTID") = "229" Then

            Session.Item("MAIL_ALERTS_FROM") = ConfigurationManager.AppSettings("MAIL_ALERTS_FROM_229")
            Session.Item("MAIL_ALERTS_TO") = ConfigurationManager.AppSettings("MAIL_ALERTS_TO_229")

            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": NW Natural Redirect.")
            Session.Item("CURRENT_STAGE") = "PaymentInformation"
        End If

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRenderComplete

        If AccountInfo Is Nothing Then
            Response.Redirect("ExpiredForbidden.aspx")
        End If

    End Sub
    Private Function GetAccountInfomartion() As Boolean
        Dim accountValid As Boolean = False
        Dim ACCOUNT_LOOKUP_MAX_ATTEMPTS As Integer
        Dim lngCounter As Long

        ACCOUNT_LOOKUP_MAX_ATTEMPTS = Session.Item("ACCOUNT_LOOKUP_MAX_ATTEMPTS")

WebRequest:

        lngCounter = lngCounter + 1

        If Regex.IsMatch(AccountNumber.Value, "^\d+$") Then
            Try
                ' Set object instance on each try in the event there is a 'Late Binding' object error.
                If Session.Item("TEST") = "TRUE" Then
                    Using NWWS_TestRelay As New NWWS_TestRelay.TestRelaySoapClient()
                        AccountInfo = New NWWS_TestRelay.AccountInformation()
                        AccountInfo = NWWS_TestRelay.GetAccountInformation(AccountNumber.Value, "W")
                        Session.Item("ACCOUNT_INFORMATION") = AccountInfo
                    End Using
                Else
                    Using NWWS_Relay As New NWWS_Relay.RelaySoapClient()
                        AccountInfo = New NWWS_Relay.AccountInformation()
                        AccountInfo = NWWS_Relay.GetAccountInformation(AccountNumber.Value, "W")
                        Session.Item("ACCOUNT_INFORMATION") = AccountInfo
                    End Using
                End If

                If AccountInfo.ResponseCode.Equals("OK") Then
                    PopulateTotalBalanceDisplayValues()
                    TotalBalanceDue.Value = AccountInfo.TotalBalance
                    txtAmount.Text = String.Format("{0:0.00}", AccountInfo.TotalBalance)
                    CardNumber.Focus()
                    accountValid = True
                Else
                    Dim errorMessages As New List(Of [String])
                    errorMessages.Add("Account Information is not available. Please try again later.")
                    Session.Add("ErrorMessages", errorMessages)

                End If
            Catch se As System.Exception

                '======================================================================
                ' Error generated from Web Service
                '======================================================================
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Exception.  Unable To Access Account Information.")
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Exception: " & se.Message)

                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", _
                                            "GetAccountInformation Exception. " & _
                                            "CurrentStage = " & Session.Item("CURRENT_STAGE") & _
                                            ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & _
                                            ", Authenticated = " & Session.Item("AUTHENTICATED") & _
                                            ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & _
                                            ", Account = " & Session.Item("ACCOUNT_NUMBER") & _
                                            ", CardTransID = " & Session.Item("CARD_TRANS_ID") & _
                                            ", AuthStatus = " & Session.Item("AUTHORIZATION_STATUS") & _
                                            ", CyberError = " & Session.Item("CYBER_ERROR"), True, Session.Item("TEST") & "" = "TEST")

                If lngCounter < ACCOUNT_LOOKUP_MAX_ATTEMPTS Then

                    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Retrying.")
                    GoTo WebRequest

                Else

                    Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Not Retrying.")


                    With Session

                        .Item("ACCOUNT_INFORMATION") = Nothing

                        If Val(Session.Item("WEB_HISTORY_ID")) = 0 Then

                            Session.Item("WEB_HISTORY_ID") = Tools.LogWebHistory(.Item("WEB_HISTORY_TABLE"), 0, .Item("SESSION_ID"), .Item("USER_HOST_ADDRESS"), .Item("START_DATETIME"), _
                                             .Item("LAST_ACTIVITY_DATETIME"), "n/a", "", .Item("CLIENTID"), .Item("ACCOUNT_NUMBER"), 0, 0, True, _
                                             "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", _
                                             "", "", "", "", "1900-01-01", "1900-01-01", "1900-01-01", "1900-01-01", _
                                             "", "", "", "", 0, 0, 0, 0, _
                                             "", "", "", "", 0, 0, 0, 0, _
                                             "", "", "", "", 0, 0, 0, 0, _
                                             "", "", "", "", False, False, False, False)
                        End If

                        Tools.UpdateWebHistoryEventLog(.Item("WEB_HISTORY_TABLE"), .Item("WEB_HISTORY_ID"), .Item("WEB_EVENTLOG"), .Item("LAST_ACTIVITY_DATETIME"), _
                                                       .Item("ACCOUNT_NUMBER"), 0, 0, True)

                    End With
                End If
            End Try
        Else
            Dim errorMessages As New List(Of [String])
            errorMessages.Add("Account Number was not found. Please try again later.")
            Session.Add("ErrorMessages", errorMessages)

        End If

        Return accountValid
    End Function

    Private Sub PopulateTotalBalanceDisplayValues()
        If (TypeOf AccountInfo Is NWWS_Relay.AccountInformation) Then
            prodAccount = AccountInfo
            If prodAccount.IsOnPaymentPlan.Equals("N") Then
                TotalBalanceDueLabelValue = "Total Balance Due"
            Else
                TotalBalanceDueLabelValue = "Plan Balance Due"
            End If
            TotalBalanceDueDisplayValue = prodAccount.TotalBalance.ToString("0.00", CultureInfo.InvariantCulture)
        Else
            testAccount = AccountInfo
            If testAccount.IsOnPaymentPlan.Equals("N") Then
                TotalBalanceDueLabelValue = "Total Balance Due"
            Else
                TotalBalanceDueLabelValue = "Plan Balance Due"
            End If
            TotalBalanceDueDisplayValue = testAccount.TotalBalance.ToString("0.00", CultureInfo.InvariantCulture)
        End If


    End Sub

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

        If Request.Browser.IsMobileDevice Or SharedModules.CheckUserAgentString(Request.UserAgent) Then
            MasterPageFile = "~/NWNMobile.Master"
        End If
    End Sub


    Protected Sub CustomExpirationDate_ServerValidate(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Dim reg As New Regex("^(0[1-9]|1[0-2])[0-9][0-9]$")
        Dim year As Integer
        Dim month As Integer

        e.IsValid = False


        Dim date1 As Date
        Dim date2 As Date
        Dim relationship As String

        If reg.IsMatch(e.Value) Then

            If Integer.TryParse(e.Value.Substring(2, 2), year) And Integer.TryParse(e.Value.Substring(0, 2), month) Then

                Dim expireDate = New Date((year + 2000), month, 1)
                expireDate = expireDate.AddMonths(1).AddDays(-1)

                Dim result As Integer = Date.Compare(expireDate, Date.Now.Date)

                If result >= 0 Then
                    e.IsValid = True
                End If
            End If
        End If









    End Sub

    
    
    Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        Dim creditCardRequestInfo As New CreditCardRequestModel

        If Page.IsValid Then
            creditCardRequestInfo.AccountNumberWithCheckDigit = AccountNumberWithCheckDigit.Value
            REM We should not have a problem with the ammount but just in case
            If Not Decimal.TryParse(txtAmount.Text, creditCardRequestInfo.Amount) Then
                Page.Validate()
            End If
            creditCardRequestInfo.BillingZipCode = BillingZipCode.Text
            creditCardRequestInfo.CardNumber = CardNumber.Text
            creditCardRequestInfo.ClientNumber = ClientNumber.Value
            creditCardRequestInfo.ExpirationDate = ExpirationDate.Text
            creditCardRequestInfo.IsInTestMode = IsInTestMode.Value = "TRUE"
            creditCardRequestInfo.SecurityCode = SecurityCode.Text
            creditCardRequestInfo.ServiceZip = ServiceZip.Value
            creditCardRequestInfo.AccountNumber = AccountNumber.Value
            creditCardRequestInfo.TotalAmountDue = TotalBalanceDue.Value
            Session.Add("CreditCardRequestInfo", creditCardRequestInfo)
            Session.Item("ENTERED_CARD_NUMBER") = creditCardRequestInfo.CardNumber
            Session.Item("ENTERED_EXPIRATION") = creditCardRequestInfo.ExpirationDate
            Session.Item("ENTERED_SECURITY_CODE") = creditCardRequestInfo.SecurityCode
            Session.Item("ENTERED_BILLING_ZIP_CODE") = creditCardRequestInfo.BillingZipCode
            Session.Item("ENTERED_AMOUNT") = creditCardRequestInfo.Amount.ToString
            Session.Item("CURRENT_STAGE") = "Confirm"
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Next.")
            'Note:  Took out Response.Redirect since it was causing issues with older versions of andriod
            'Response.Redirect("Confirm.aspx")
            Server.Transfer("Confirm.aspx")

        End If
    End Sub

    Private Function SessionInvalid() As Boolean

        SessionInvalid = False

        If Session.Item("AUTHENTICATED") <> "TRUE" Then

            SessionInvalid = True
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid. Authenticated = False")
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), "SessionInValid.  Authenticated = False")
            'AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. Authenticated = FALSE. " & _
            '                "CurrentStage = " & Session.Item("CURRENT_STAGE") & ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"))

            AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. Authenticated = FALSE. " & _
                            "CurrentStage = " & Session.Item("CURRENT_STAGE") & ", NavigatingFrom = " & Session.Item("NAVIGATING_FROM") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"), True, Session.Item("TEST") = "TRUE")


        End If

        If STAGE_PAGE <> "" And Session.Item("CURRENT_STAGE") <> STAGE_PAGE Then

            If Session.Item("NAVIGATING_FROM") <> STAGE_PAGE Then

                SessionInvalid = True
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  NavigatingFrom = " & Session.Item("NAVIGATING_FROM"))
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  Should be NavigatingFrom = " & STAGE_PAGE)
                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. Navigating from " & Session.Item("NAVIGATING_FROM") & ", should be " & STAGE_PAGE & "." & _
                                " CurrentStage = " & Session.Item("CURRENT_STAGE") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"), True, Session.Item("TEST") & "" = "TEST")
            Else

                SessionInvalid = True
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  Stage = " & Session.Item("CURRENT_STAGE"))
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  NavigatingFrom = " & Session.Item("NAVIGATING_FROM"))
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  Should be Stage = " & STAGE_PAGE)
                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. Navigating from " & Session.Item("NAVIGATING_FROM") & ", should be " & STAGE_PAGE & "." & _
                                " CurrentStage = " & Session.Item("CURRENT_STAGE") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"), True, Session.Item("TEST") & "" = "TEST")

            End If

        Else

            If STAGE_PAGE = "" Then

                SessionInvalid = True
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": SessionInValid.  STAGE_PAGE = ''")
                AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", "ExpForbid", STAGE_PAGE & " SessionInvalid. STAGE_PAGE = ''." & _
                                " CurrentStage = " & Session.Item("CURRENT_STAGE") & ", WebHistoryID = " & Session.Item("WEB_HISTORY_ID") & ", Account = " & Session.Item("ACCOUNT_NUMBER"), True, Session.Item("TEST") & "" = "TEST")

            End If

        End If

        If SessionInvalid Then

            Session.Item("EXPIRED_FORBIDDEN_MESSAGE_SENT") = "TRUE"
            Response.Redirect(System.Web.VirtualPathUtility.ToAbsolute("~/ExpiredForbidden.aspx"))

        End If

    End Function
End Class