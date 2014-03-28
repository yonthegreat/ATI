Public Class ExpiredForbidden
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        If Request.Browser.IsMobileDevice Or SharedModules.CheckUserAgentString(Request.UserAgent) Then
            MasterPageFile = "~/NWNMobile.Master"
        End If
    End Sub

End Class