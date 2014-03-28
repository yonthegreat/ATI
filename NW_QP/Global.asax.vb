Imports System.Diagnostics
Imports System.Web.SessionState
Imports System.Web.UI
Imports System.Web.Util
Imports System.Web.Routing




Public Class Global_asax
    Inherits System.Web.HttpApplication

    '<%@ Application Language="VB" %>
    '<%@ Import Namespace="System.Diagnostics" %>
    '<%@ Import Namespace="System.Web.SessionState" %>


    'A key issue with taking advantage of the events is knowing the order in which they're triggered. The Application_Init 
    'and Application_Start events are fired once when the application is first started. Likewise, the Application_Disposed 
    'and Application_End are only fired once when the application terminates. In addition, the session-based events 
    '(Session_Start and Session_End) are only used when users enter and leave the site. The remaining events deal with 
    'application requests, and they're triggered as follows:


    'Order of Events:
    '============================================
    'Application_BeginRequest
    'Application_AuthenticateRequest
    'Application_AuthorizeRequest
    'Application_ResolveRequestCache
    'Application_AcquireRequestState
    'Application_PreRequestHandlerExecute
    'Application_PreSendRequestHeaders
    'Application_PreSendRequestContent
    ' <<other app code is executed>>
    'Application_PostRequestHandlerExecute
    'Application_ReleaseRequestState
    'Application_UpdateRequestCache
    'Application_EndRequest


    '    'Description Of Events:
    '============================================
    'Application_Init: Fired when an application initializes or is first called. It's invoked for all HttpApplication object instances.
    'Application_Disposed: Fired just before an application is destroyed. This is the ideal location for cleaning up previously used resources.
    'Application_Error: Fired when an unhandled exception is encountered within the application.
    'Application_Start: Fired when the first instance of the HttpApplication class is created. It allows you to create objects that are accessible by all HttpApplication instances.
    'Application_End: Fired when the last instance of an HttpApplication class is destroyed. It's fired only once during an application's lifetime.
    'Application_BeginRequest: Fired when an application request is received. It's the first event fired for a request, which is often a page request (URL) that a user enters.
    'Application_EndRequest: The last event fired for an application request.
    'Application_PreRequestHandlerExecute: Fired before the ASP.NET page framework begins executing an event handler like a page or Web service.
    'Application_PostRequestHandlerExecute: Fired when the ASP.NET page framework is finished executing an event handler.
    'Application_PreSendRequestHeaders: Fired before the ASP.NET page framework sends HTTP headers to a requesting client (browser).
    'Application_PreSendContent: Fired before the ASP.NET page framework sends content to a requesting client (browser).
    'Application_AcquireRequestState: Fired when the ASP.NET page framework gets the current state (Session state) related to the current request.
    'Application_ReleaseRequestState: Fired when the ASP.NET page framework completes execution of all event handlers. This results in all state modules to save their current state data.
    'Application_ResolveRequestCache: Fired when the ASP.NET page framework completes an authorization request. It allows caching modules to serve the request from the cache, thus bypassing handler execution.
    'Application_UpdateRequestCache: Fired when the ASP.NET page framework completes handler execution to allow caching modules to store responses to be used to handle subsequent requests.
    'Application_AuthenticateRequest: Fired when the security module has established the current user's identity as valid. At this point, the user's credentials have been validated.
    'Application_AuthorizeRequest: Fired when the security module has verified that a user can access resources.
    'Session_Start: Fired when a new user visits the application Web site.
    'Session_End: Fired when a user's session times out, ends, or they leave the application Web site.


    Protected Sub Application_BeginRequest(sender As Object, e As System.EventArgs)
    End Sub

    Protected Sub Application_AuthenticateRequest(sender As Object, e As System.EventArgs)
    End Sub

    Protected Sub Application_AuthorizeRequest(sender As Object, e As System.EventArgs)
    End Sub

    Protected Sub Application_Disposed(sender As Object, e As System.EventArgs)
    End Sub

    Protected Sub Application_EndRequest(sender As Object, e As System.EventArgs)
    End Sub

    Protected Sub Application_AcquireRequestState(sender As Object, e As System.EventArgs)
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        On Error Resume Next

        Response.Redirect(System.Web.VirtualPathUtility.ToAbsolute("~/SysPgs/Exception.aspx"))

    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        On Error Resume Next

        With Session

            .Item("WEB_EVENTLOG") = ""
            .Item("START_DATETIME") = Now.ToString
            .Item("LAST_ACTIVITY_DATETIME") = Now.ToString
            .Item("SESSION_ID") = Session.SessionID.ToString
            .Item("USER_HOST_ADDRESS") = Request.UserHostAddress.ToString
            If Request.UrlReferrer Is Nothing Then
                .Item("URL_REFERER") = ""
            Else
                .Item("URL_REFERER") = Request.UrlReferrer.ToString
            End If
            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), "IPSQuickPAY Session Start 3.1")
            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), "Platform: " & Request.Browser.Platform)
            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), "Browser: " & Request.Browser.Browser)
            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), "Type: " & Request.Browser.Type)
            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), "Version: " & Request.Browser.Version)
            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), "Cookies: " & Request.Browser.Cookies)
            Tools.AddWebHistoryItem(.Item("WEB_EVENTLOG"), "URLReferrer: " & .Item("URL_REFERER"))

        End With

    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)

    End Sub


End Class