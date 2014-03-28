Public Class NWNMobile
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub LinkButtonClick(sender As Object, e As EventArgs)

        Dim linkButton As LinkButton
        linkButton = sender
        Session.Clear()
        Session.Abandon()
        Response.Redirect(linkButton.CommandArgument)


    End Sub

End Class