<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/NWN.Master" CodeBehind="Receipt.aspx.vb" Inherits="NW_QP.Receipt" %>
<%@ Import Namespace="NW_QP" %>
<%@ Import Namespace = "Microsoft.Security.Application" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainBodyContent" runat="server">

 
	

	<h1 >Receipt</h1>
	<div>
		<p>Your authorization to make a one-time payment to NW Natural was successfully sent.  Print this page as your receipt.</p>
		<p>If you have questions regarding your transaction or account, please contact NW Natural at 800-422-4012.</p>
		
	</div>
	
			
		<div class="signup">
		<div class="row" > 			
			<div class="longlabel" >
				<strong>Confirmation&nbsp;Number:</strong>
			</div >
			<div class="verbiage" >
				<%= Microsoft.Security.Application.Encoder.HtmlEncode(creditCardRequestInfo.ConfirmationNumber)%>
			</div >
		</div>
		
		
		<div class="row" > 			
			<div class="longlabel" >
				<strong>NW&nbsp;Natural&nbsp;Account&nbsp;#:</strong>
			</div >
			<div class="verbiage" >
				
				<%= Microsoft.Security.Application.Encoder.HtmlEncode(creditCardRequestInfo.AccountNumberWithCheckDigit)%>
			</div >
		</div>
		
		<div class="row" > 			
			<div class="longlabel" >
				<strong>Payment&nbsp;Amount:</strong>
			</div >
			<div class="verbiage" >
				$<%= creditCardRequestInfo.Amount %>
			</div >
		</div>	
		<div class="row" > 			
			<div class="longlabel" >
				<strong>Card Number: </strong>
			</div >
			<div class="verbiage" >
				****<%= Microsoft.Security.Application.Encoder.HtmlEncode(creditCardRequestInfo.CardNumber.Substring(creditCardRequestInfo.CardNumber.Length - 4, 4))%>
			</div >
		</div>
		</div>
		<div class="process-actions">
		<p>	
			<asp:Button id="btnNext"  data-theme="b" data-role="button"   Text="Back to Home" CssClass="iebutton right" runat="server"  />
			 <%If Not SharedModules.CheckUserAgentString((Request.UserAgent)) Then%>
			<input type="button" value="PRINT THIS PAGE"  onclick="window.print();" class="iebutton right" />
			<% End If%>
		</p>
		</div>


  
</asp:Content>
