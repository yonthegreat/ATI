<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/NWN.Master" CodeBehind="SubmissionIssue.aspx.vb" Inherits="NW_QP.SubmissionIssue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainBodyContent" runat="server">
    
    <div  class="validation-summary-errors" style="color:#812F17;">
	There was an issue with your submission.
<ul>

<% For Each o As String In errorMessages %>
<li><%= Microsoft.Security.Application.Encoder.HtmlEncode(o)%></li>
 <%Next%>
</ul>
</div>
</asp:Content>
