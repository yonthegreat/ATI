<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/NWN.Master" CodeBehind="Confirm.aspx.vb" Inherits="NW_QP.Confirm" %>
<%@ Import Namespace="NW_QP" %>
<%@ Import Namespace = "Microsoft.Security.Application" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainBodyContent" runat="server">

	<h1 >Confirm</h1>
	<div>
		<p>Please confirm that the information shown below is correct before authorizing your payment.</p>
		
	</div>
		
 		<div class="signup">
 		<div class="row" > 			
 			<div class="longlabel" >
 				<strong>NW Natural Account #: </strong>
 			</div >
 			<div class="verbiage" >
                <%= Microsoft.Security.Application.Encoder.HtmlEncode(creditCardRequestInfo.AccountNumberWithCheckDigit)%>
 			</div >
 		</div>
 		
 		
 		<div class="row" > 			
 			<div class="longlabel" >
 				<strong>Payment Amount: </strong>
 			</div >
 			<div class="verbiage" >
 				$<%= creditCardRequestInfo.Amount%>
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
 		
 		<div class="row" > 			
 			<div class="longlabel" >
 				<strong>Expiration Date:  </strong>
 			</div >
 			<div class="verbiage" >
 				<%= Microsoft.Security.Application.Encoder.HtmlEncode(creditCardRequestInfo.ExpirationDate.Substring(0, 2))%>/<%= Microsoft.Security.Application.Encoder.HtmlEncode(creditCardRequestInfo.ExpirationDate.Substring(2, 2))%>
 			</div >
 		</div>

        
        <div class="row" > 			
 			<div class="longlabel" >
 				<strong>Security Code:</strong>
 			</div >
 			<div class="verbiage" >
 				****
 			</div >
 		</div>

        <div class="row" > 			
 			<div class="longlabel" >
 				<strong>Billing ZIP Code:  </strong>
 			</div >
 			<div class="verbiage" >
                <%= Microsoft.Security.Application.Encoder.HtmlEncode(creditCardRequestInfo.BillingZipCode)%>
 			</div >
 		</div>
 		</div>


 		



 		
 		<div class="process-actions">
 		<p>
 		    <asp:Button id="btnNext" data-theme="b" data-role="button"     Text="SUBMIT" CssClass="iebutton right" runat="server"  />
            <%If Not SharedModules.CheckUserAgentString((Request.UserAgent)) Then %>
 			<a href="PaymentInformation.aspx">Back</a>
            <% Else%>
            <a data-theme="a" data-role="button" href="PaymentInformation.aspx">Back</a>
            <% End If%>
		</p>
		</div>
	
    <script type="text/javascript">
        document.getElementById("ctl00_MainBodyContent_btnNext").focus();    
    </script>
	



</asp:Content>

