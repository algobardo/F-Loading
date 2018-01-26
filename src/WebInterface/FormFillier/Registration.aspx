<%@ Page Language="C#" MasterPageFile="~/FormFillier/index.Master" AutoEventWireup="true"
    CodeBehind="Registration.aspx.cs" Inherits="WebInterface.FormFillier.Registration"
    Title="Pagina senza titolo" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="../lib/css/smoothness/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/FormFiller/contentRegistration.css" type="text/css" rel="Stylesheet"
        media="screen" />

    <script src="../lib/js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../lib/js/jquery-ui-1.7.1.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../lib/js/AnimeJ.js"></script>

    <title>Registration</title>
</asp:Content>
<asp:Content ID="Regitration" ContentPlaceHolderID="contentHome" runat="server">
    <div id="content-container">
        <div id="content-border">
            <div id="textContainer" class="boxContainerGeneral">
                <h1 class="h1">
                    Welcome to registration page</h1>
                <h3 class="h3" runat="server" id="descriptionPage">
                </h3>
            </div>
            <div id="PanelNick" class="boxContainerGeneral boxContainerNick">
                <fieldset>
                    <legend>Personal Information</legend>
                    <h4 class="h4">
                        Please insert a nickname</h4>
                    <asp:UpdatePanel ID="PanelServices" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="PanelNickUpdated" runat="server" EnableViewState="true" Enabled="true">
                                <label title="Insert your nickname" id="labelNick" class="firstBox">
                                    Nickname *
                                </label>
                                <asp:TextBox ID="TextBoxNick" CssClass="secondBox" runat="server" ToolTip="Insert your nickname"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBoxNick"
                                    ErrorMessage="You must insert your nickname" Display="Dynamic" Font-Bold="True"
                                    Font-Size="Large"></asp:RequiredFieldValidator>
                                <div>
                                    <label title="Insert your email" id="labelMail" class="firstBox">
                                        E-Mail *
                                    </label>
                                    <asp:TextBox ID="TextBoxMail" CssClass="secondBox mail" runat="server" ToolTip="Insert your email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBoxMail"
                                        ErrorMessage="You must insert your email" Display="Dynamic" Font-Bold="True"
                                        Font-Size="Large"></asp:RequiredFieldValidator>
                                </div>
                                <h4 class="h4">
                                    Select your additional services</h4>
                                <div id="PanelServReg" runat="server">
                                </div>
                                <asp:DropDownList ID="DropDownList1" runat="server" CssClass="firstBox dropDown">
                                </asp:DropDownList>
                                <asp:Button ID="register" runat="server" CausesValidation="false" Text="Add a New Service"
                                    CssClass="secondBox specialBox" OnClick="registerClick" />
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
            </div>
            <div id="contGen" class="boxContainerGeneral boxContainerContract">
                <asp:Panel ID="P2" runat="server" EnableViewState="true">
                    <fieldset>
                        <legend>Terms & Conditions</legend>
                        <asp:TextBox ID="textCondition" TextMode="MultiLine" runat="server" CssClass="contract"
                            Text="" Enabled="false"></asp:TextBox>
                        <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="upCheck">
                            <ContentTemplate>
                                <asp:CheckBox ID="chkTandCs" Text="I have read and understand the Terms & Conditions of this agreemen"
                                    AutoPostBack="false" CssClass="checkContract" TextAlign="Left" runat="server"
                                    Enabled="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%--  </div>--%>
                    </fieldset>
                </asp:Panel>
            </div>
            <div id="bottomButton">
                <asp:ImageButton ID="Home" CssClass="buttonSxReg" runat="server" ImageUrl="~/lib/image/Home.png"
                    CausesValidation="false" ToolTip="HomePage" PostBackUrl="~/FormFillier/index.aspx" />
                <asp:ImageButton ID="okButt" runat="server" ImageUrl="~/lib/image/ok.png" CausesValidation="false"
                    ToolTip="Accept" OnClick="okButt_Click" CssClass="buttonDxReg" />
            </div>
        </div>
    </div>
</asp:Content>
