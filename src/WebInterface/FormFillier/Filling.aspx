<%@ Page Language="C#" MasterPageFile="~/FormFillier/index.Master" AutoEventWireup="true"
    CodeBehind="Filling.aspx.cs" Inherits="WebInterface.FormFillier.Filling" Trace="false" %>

<%@ Register Assembly="Fields" Namespace="Fields" TagPrefix="cc1" %>
<asp:Content runat="server" ContentPlaceHolderID="head" ID="head">

    <script type="text/javascript" src="../lib/js/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../lib/js/jquery-ui-1.7.1.custom.min.js"></script>

    <script src="../lib/js/AnimeJ.js" type="text/javascript"></script>

    <script src="../lib/js/jquery.popup.js" type="text/javascript"></script>        

    <link href="../lib/css/smoothness/jquery-ui-1.7.1.custom.css" media="screen" type="text/css"
        rel="Stylesheet" />
    <link href="../lib/css/FormFiller/contentPresentation.css" media="screen" type="text/css"
        rel="Stylesheet" />
    <link href="../lib/css/FormFiller/dynamicContent.css" media="screen" type="text/css"
        rel="Stylesheet" />
    <title>Form Filling</title>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="contentHome" ID="Content1">

    <script src="../lib/js/FormFiller/fillingHandler.js" type="text/javascript"></script>

    <div id="fill">
    <div id="callFill" runat="server"></div>
        <div id="dialog" title="Format error">
            <p id="dialogP">
            </p>
        </div>
        <div id="content-container">
            <div id="content-border">
                <div id="contentAll" runat="server">
                    <fieldset>
                        <div id="panelFill">
                            <div id="dialogModal" title="Generic error">
                                <p id="dialogModalP">
                                    You haven't permisson to compile this form.<br />
                                    <br />
                                    You will be redirect at home page;
                                </p>
                            </div>
                            <div id="dialogLog" title="Need authorization" style="display: none;">
                                <p id="dialogLogP">
                                    To compile this form you need to authenticate.<br />Choose a service to login
                                </p>
                                <select id="selLog">
                                       
                                    </select>
                            </div>
                            <div id="saveDialog" style="display: none">
                                <p>
                                    Are you sure???
                                </p>
                            </div>
                            <asp:Panel ID="presenPanel3" runat="server">
                            </asp:Panel>
                            <div id="formHeaderContainer">
                                <div id="imageContainer">
                                    <%if (Session["wf"] != null)
                                      { %>
                                    <asp:Image ID="Logo" runat="server" ImageUrl="~/FormFillier/logoBmp.aspx" />
                                    <%} %>
                                </div>
                                <div id="titleContainer">
                                    <p id="formTitle" runat="server">
                                    </p>
                                </div>
                            </div>
                            <asp:UpdatePanel ID="corpo" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="presenPanel" runat="server">
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:Panel ID="presenPanel2" runat="server" CssClass="Panel2" Visible="false">
                            </asp:Panel>
                        </div>
                        <div id="footerControl">
                            <div id="backDiv">
                                <asp:Button ID="back" ToolTip="Back" CausesValidation="false" runat="server" CssClass="buttonSx"
                                    Text="Back" OnClick="back_Click" />
                            </div>
                            <div id="forwardDiv">
                                <asp:Button ID="forward" runat="server" ToolTip="Forward" CssClass="buttonDx" Text="Forward"
                                    OnClick="forward_Click" ValidationGroup="1" />
                                <input type="button" class=" buttonDx" runat="server" title="Save" id="save" value="save"
                                    onclick="save_Click();" visible="false" />
                            </div>
                            <div id="controlDiv">
                                <asp:ImageButton ID="restart" CausesValidation="false" ImageUrl="~/lib/image/Restart.png"
                                    runat="server" CssClass="buttonRestart" ToolTip="Restart" OnClick="restart_Click" />
                                <asp:ImageButton ID="home" CausesValidation="false" ImageUrl="~/lib/image/Home.png"
                                    runat="server" CssClass="buttonMid" ToolTip="Home" PostBackUrl="~/FormFillier/index.aspx" />
                                <asp:ImageButton ID="clear" ImageUrl="~/lib/image/Clear.png" CausesValidation="false"
                                    runat="server" CssClass="buttonClear" ToolTip="Clear" OnClick="clear_Click" />
                            </div>
                        </div>
                    </fieldset>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
