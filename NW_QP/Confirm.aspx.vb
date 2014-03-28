Public Class Confirm
    Inherits Page
    Protected creditCardRequestInfo As CreditCardRequestModel
    Protected AccountInfo As Object

    Private Const STAGE_PAGE As String = "Confirm"
    Private Const ACTUAL_PAGE As String = "Confirm"

    'Protected AccountInfo As LegacyNwnIvrWebService.AccountInformation

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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



        If SessionInvalid() Then Exit Sub

        If IsPostBack = False Then
            Session.Item("NAVIGATING_FROM") = STAGE_PAGE
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": PageLoad, non Postback.")
        Else
            Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": PageLoad, Postback.")

            If Session.Item("AUTHORIZATION_STATUS") = "COMPLETED" Then
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Authorization Status = " & Session.Item("AUTHORIZATION_STATUS"))
                Tools.AddWebHistoryItem(Session.Item("WEB_EVENTLOG"), ACTUAL_PAGE & ": Redirecting to ConfirmationInfo.")
                Session.Item("CURRENT_STAGE") = "Receipt"
                Response.Redirect(System.Web.VirtualPathUtility.ToAbsolute("~/Receipt.aspx"), False)
            End If
        End If
    End Sub

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        If Request.Browser.IsMobileDevice Or SharedModules.CheckUserAgentString(Request.UserAgent) Then
            MasterPageFile = "~/NWNMobile.Master"
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
        creditCardRequestInfo = Session("CreditCardRequestInfo")
        Dim testAccountInfo As NWWS_TestRelay.AccountInformation
        Dim prodAccountInfo As NWWS_Relay.AccountInformation
        If (TypeOf Session.Item("ACCOUNT_INFORMATION") Is NWWS_TestRelay.AccountInformation) Then
            testAccountInfo = Session.Item("ACCOUNT_INFORMATION")
            AccountInfo = testAccountInfo
        Else
            prodAccountInfo = Session.Item("ACCOUNT_INFORMATION")
            AccountInfo = prodAccountInfo
        End If
        If creditCardRequestInfo Is Nothing Or AccountInfo Is Nothing Then
            Response.Redirect("ExpiredForbidden.aspx")
        End If

    End Sub

 


    Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        Session.Item("CURRENT_STAGE") = "Receipt"
        'Note:  Took out Response.Redirect since it was causing issues with older versions of andriod
        'Response.Redirect("Receipt.aspx")
        Server.Transfer("Receipt.aspx")
    End Sub

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


End Class