Public Class SubmissionIssue
    Inherits System.Web.UI.Page
    Public errorMessages As List(Of [String])
    Private Const ACTUAL_PAGE As String = "SubmissionIssue"
    Private Const STAGE_PAGE As String = "SubmissionIssue"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        errorMessages = Session("ErrorMessages")
        If errorMessages Is Nothing Then
            errorMessages = New List(Of String)()
            errorMessages.Add("Unknown issue")
        End If

        Session.Item("NAVIGATING_FROM") = STAGE_PAGE

        AppTextLog.WriteLogEntry(Session.Item("LOG_DRIVE") & "", ACTUAL_PAGE & "", errorMessages.Item(0) & " Account = " & Session.Item("ACCOUNT_NUMBER"), False, Session.Item("TEST") & "" = "TEST")

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
        Session.Clear()
        Session.Abandon()
    End Sub

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        If Request.Browser.IsMobileDevice Or SharedModules.CheckUserAgentString(Request.UserAgent) Then
            MasterPageFile = "~/NWNMobile.Master"
        End If
    End Sub

End Class