<%@ Page Title="" Language="C#" MasterPageFile="~/FormFillier/index.Master" AutoEventWireup="true"
    CodeBehind="httpsLogin.aspx.cs" Inherits="WebInterface.Auth.httpsLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="../lib/css/smoothness/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/FormFiller/contentHome.css" type="text/css" rel="Stylesheet"
        media="screen" />

    
    <script src="../lib/js/FormFiller/AuthLogin.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentHome" runat="server">
    <div id="content-container">
        <div id="content-border">
            <fieldset >
                <div id="callLogin" runat="server"  style="min-height:400px;">
                </div>
                <%--<div>
                    <h2>
                        Please insert your username/password for the service
                        <%=Session["HTTPSServiceName"]%></h2>
                    <asp:TextBox ID="d" runat="server">username</asp:TextBox>
                    <asp:TextBox ID="d2" runat="server" TextMode="Password">password</asp:TextBox>
                    <asp:Button ID="Submit" runat="server" OnClick="Button1_Click" Text="Button" />
                </div>--%>
                <div id="httpsService" title="Authenticate for the service">
                    <p id="labelUserHttps" class="contentLabel">
                        Username</p>
                    <asp:TextBox ID="Username" runat="server" TextMode="SingleLine" CssClass="contentOption"></asp:TextBox>
                    <p id="labelPassHttps" class="contentLabel">
                        Password</p>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="contentOption"></asp:TextBox>
                </div>
            </fieldset>
        </div>
    </div>
</asp:Content>
