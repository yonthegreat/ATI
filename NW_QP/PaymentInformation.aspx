<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/NWN.Master" CodeBehind="PaymentInformation.aspx.vb" Inherits="NW_QP.PaymentInformation" %>
<%@ Import Namespace="NW_QP" %>
<%@ Import Namespace = "Microsoft.Security.Application" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainBodyContent" runat="server">
<h1 >Payment Information</h1>
<div>
	<p>This payment method promptly reflects on your account and will automatically prevent a pending service interruption.</p>
	<p>Please verify your NW Natural account number then enter your payment information.</p>
	<p>NW Natural Account #: <strong><%= Microsoft.Security.Application.Encoder.HtmlEncode(AccountNumberWithCheckDigit.Value)%> </strong> </p>
	<% If Not String.IsNullOrEmpty(TotalBalanceDueLabelValue) Then%>
	<p><%= TotalBalanceDueLabelValue%>:<strong> $<%= TotalBalanceDueDisplayValue %> </strong></p>	
	<% End If%>	
</div>

<asp:HiddenField ID="IsInTestMode" runat="server" />
<asp:HiddenField ID="ClientNumber" runat="server" />
<asp:HiddenField ID="AccountNumber" runat="server" />
<asp:HiddenField ID="AccountNumberWithCheckDigit" runat="server" />
<asp:HiddenField ID="ServiceZip" runat="server" />
<asp:HiddenField ID="TotalBalanceDue" runat="server" />	
<div class="process-actions">
	 <asp:ValidationSummary 
		ID="ValidationSummary1"
		HeaderText="Please correct the errors and try again."
		DisplayMode="BulletList" 
		CssClass="validation-summary-errors"
		EnableClientScript="False"
		ShowSummary="True" 
		ShowMessageBox = "True"
		ForeColor="#812f17"
		runat="server"/>
	<div class="signup">
		<div class="row">
			<div class="label">
				<label><span class="required">*</span>Payment&nbsp;Amount:</label>
			</div>
			<div class="formField">
				<asp:TextBox ID="txtAmount"  MaxLength="8" type="number"   runat="server"></asp:TextBox>
			</div>
			<div class="Error">
				<asp:RequiredFieldValidator 
					ID="RequiredFieldValidator2" 
					runat="server" 
					ControlToValidate="txtAmount" 
					ErrorMessage="Must provide Payment Amount" 
					Display="Dynamic"
					EnableClientScript="False"  
					Text="<img src='Images/error.png' title='Must provide Payment Amount' />" /> 
				<asp:RegularExpressionValidator 
						ID="RegularExpressionValidator5" 
						runat="server" 
						EnableClientScript="False"
						ControlToValidate="txtAmount" 
						ValidationExpression="^(\d{1,3}[0-9]\.\d{2}|[1-9]\.\d{2}|[1][0][0][0][0]\.[0][0])"
						ErrorMessage="Payment Amount must be greater than $1.00 and less than $10,000" 
						Display="Dynamic"  
						Text="<img class='errorImg' src='Images/error.png' title='Payment Amount must be greater than $1.00 and less than $10,000' />"/>
			</div>
			<div class="example">
				<i>Example:&nbsp;0.00</i>
			</div>
		</div>
		<div class="row">
			<div class="label">
				<label><span class="required">*</span>Card&nbsp;Number:</label>
			</div>
			<div class="formField">
				<asp:TextBox ID="CardNumber"  MaxLength="16" type="number" runat="server"></asp:TextBox>
			</div>
			<div class="Error">
				<asp:RequiredFieldValidator 
					ID="RequiredCardNumber" 
					runat="server" 
					EnableClientScript="False"
					ControlToValidate="CardNumber" 
					ErrorMessage="Must provide Card Number" 
					Display="Dynamic"  
					Text="<img src='Images/error.png'  title='Must provide Card Number' />" />    
				 <asp:RegularExpressionValidator 
						ID="RegxCardNumber" 
						runat="server" 
						EnableClientScript="False"
						ControlToValidate="CardNumber" 
						ValidationExpression="^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$"
						ErrorMessage="Card Number must be 16 digits and contain no spaces or dashes" 
						Display="Dynamic"  
						Text="<img src='Images/error.png' title='Card Number must be 16 digits and contain no spaces or dashes' />"/>    
			</div>
			<div class="example"><i>Example:&nbsp;1111222233334444</i></div>
		</div>
		<div class="row">
			<div class="label">
				<label><span class="required">*</span>Expiration&nbsp;Date:</label>
			</div>
			<div class="formField">
				<asp:TextBox ID="ExpirationDate" MaxLength="4" type="number" runat="server"></asp:TextBox>
			</div>
			<div class="Error">
				<asp:RequiredFieldValidator 
					ID="RequiredFieldValidator4" 
					runat="server" 
					EnableClientScript="False"
					ControlToValidate="ExpirationDate" 
					ErrorMessage="Must provide Expiration Date" Display="Dynamic"  Text="<img src='Images/error.png' title='Must provide Expiration Date' />" />                  
				<asp:CustomValidator 
					runat="server" 
					id="CustomExpirationDate" 
					controltovalidate="ExpirationDate" 
					onservervalidate="CustomExpirationDate_ServerValidate" 
					Text="<img src='Images/error.png' title='Expiration Date must be 4 digits, formatted as MMYY' />"   
					Display="Dynamic"                  
					ErrorMessage="Expiration Date must be 4 digits, formatted as MMYY"  />
			</div>
			<div class="example">
				<i>Example:&nbsp;mmyy</i>
			</div>
		</div>
		<div class="row">
			<div class="label">
				<label><span class="required">*</span>Security&nbsp;Code:</label>
			</div>
			<div class="formField">
				<asp:TextBox ID="SecurityCode" MaxLength="3" type="number" runat="server"></asp:TextBox>
			</div>
			<div class="Error">
				<asp:RequiredFieldValidator 
					ID="RequiredFieldValidator5" 
					runat="server" 
					EnableClientScript="False"
					ControlToValidate="SecurityCode" 
					ErrorMessage="Must provide Security Code" Display="Dynamic"  Text="<img src='Images/error.png' title='Must provide Security Code' />" />    
					<asp:RegularExpressionValidator 
						ID="RegularExpressionValidator2" 
						runat="server" 
					EnableClientScript="False"
						ControlToValidate="SecurityCode" 
						ValidationExpression="^\d{3}"
						ErrorMessage="Security Code must be 3 digits, such as 123" Display="Dynamic"  Text="<img src='Images/error.png' title='Security Code must be 3 digits, such as 123' />"/>
	
			</div>
			<div class="example">
				<i><a class="example" target="_blank"  href=" https://ipsquickpay.net/SysPgs/SecCode.aspx">(3-digit&nbsp;number)</a></i>
			</div>
		</div>
		<div class="row">
			<div class="label">
				<label><span class="required">*</span>Billing&nbsp;ZIP&nbsp;Code:</label>
			</div>
			<div class="formField">
				<asp:TextBox ID="BillingZipCode" MaxLength="5" type="number"  runat="server"></asp:TextBox>
			</div>
			<div class="Error">
				<asp:RequiredFieldValidator 
					ID="RequiredFieldValidator3" 
					runat="server" 
					EnableClientScript="False"
					ControlToValidate="BillingZipCode" 
					ErrorMessage="Must provide Billing ZIP Code" Display="Dynamic"  Text="<img src='Images/error.png' title='Must provide Billing ZIP Code' />" />
				<asp:RegularExpressionValidator 
					ID="RegularExpressionValidator1" 
					runat="server" 
				EnableClientScript="False"
					ControlToValidate="BillingZipCode" 
					ValidationExpression="^\d{5}"
					ErrorMessage="Billing ZIP Code must be 5 digits, such as 97209" Display="Dynamic"  Text="<img src='Images/error.png' title='Billing ZIP Code must be 5 digits, such as 97209' />"/>
		
	
			</div>
			<div class="example">
				<i>Example:&nbsp;97209</i>
			</div>
		</div>	
	</div>
	<div class="row">
		<asp:Button id="btnNext" data-theme="b" data-role="button"   Text="NEXT STEP" CssClass="iebutton right" runat="server"  />
		<%If Not SharedModules.CheckUserAgentString((Request.UserAgent)) Then %>
			<a href="https://www.nwnatural.com/AccountDashboard">Cancel</a>
		<% Else%>
			<a data-theme="c" data-role="button" href="https://www.nwnatural.com/AccountDashboard">Cancel</a>
		<% End If%>
	</div>
</div>

 <script type="text/javascript">
	 document.getElementById("ctl00_MainBodyContent_txtAmount").focus();    
	</script>	
	
</asp:Content>