<%@ Page Language="C#" MasterPageFile="~/FormFillier/index.Master" AutoEventWireup="true" CodeBehind="managecontacts.aspx.cs" Inherits="WebInterface.FormFillier.WebForm1" Title="Pagina senza titolo"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <link href="../lib/css/smoothness/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
     <link href="../lib/css/FormFiller/contentContacts.css" type="text/css" rel="Stylesheet" media="screen" />
     <script src="../lib/js/FormFiller/jshashtable.js" type="text/javascript"></script>
     <script src="../lib/js/spinner.js" type="text/javascript"></script>
     <script src="../lib/js/FormFiller/ContactsManager/ContactsManager.js" type="text/javascript"></script>
    <link href="../lib/css/WorkflowEditor/WFE_css/EditorInterface.css" rel="stylesheet" type="text/css" />
    <title>Contacts</title>
   
</asp:Content>
<asp:Content ID="manageContacts" ContentPlaceHolderID="contentHome" runat="server">
    <div id="content-container" align="left">
        <div id="call" runat="server">
        </div>
        <div id="content-border">
            <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                    </fieldset>
                    <div id="mainaccordion">
                        <div id="accordion"></div>
                        <div id="slider"></div>
                    </div>
                    <div id="buttons" align="center">
                        <input type="button" id="download_contacts" class="button_import" title="Import contacts" />Import contacts<br />
                        <input type="button" id="create_group" class="button_addgroup" title="Create group" />Create a group<br />                        
                        <input type="button" id="return_back" class="button_returnback" title="Return cack" onclick="returnBack()" />Back<br />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
        <%--***************************GROUPS OF DIALOGS ********************************--%>
        <div id="dialog" title="Choose the service" align="center">
            <div id="tuttiiservizi">
                <img id="imageLoading" alt="image loading" src="../lib/image/loading4.gif" />
                <br />
                <select id="selserv" style="visibility: hidden;">
                </select>
            </div>
        </div>
        <div id="contactfirst" title="Choose contacts to download">
            <div id="tuttiicontatti">
                <img id="imageLoading1" alt="image loading" src="../lib/image/loading4.gif" />
            </div>
        </div>
        <div id="selectGroups" title="Choose name and contacts" align="center">
            <div id="bord">
                <div id="allconts" align="left">
                </div>
            </div>
            <br />
            Group name:&nbsp;<input id="newNameGroup" type="text" />
        </div>
        <div id="choiseGroup" title="Remove this group?" align="center">
            <div id="ngroup">
                <br />
                <label id="remObject" style="font-weight: bold;">
                </label>
                <br />
                <br />
                <input id="rem_perm" type="checkbox" />
                <label>
                    Remove permanently contacts</label>
            </div>
        </div>
        <div id="renameGroup" title="Rename this group?" align="center">
            <div id="rGroup">
                <br />
                Old name:&nbsp;<label style="font-weight: bold;" id="renGroup"></label>
                <br />
                <br />
                New name:&nbsp;<input id="newnametext" type="text" />
                <br />
            </div>
        </div>
        <div id="removeContacts" title="Remove these contacts?" align="center">
            <label id="hidegrouprem" visible="false">
            </label>
            <div id="remConts" align="left">
            </div>
        </div>
        <div id="moveContacts" title="Move these contacts?" align="center">
            <label id="hidegroupmov" visible="false">
            </label>
            <div id="movConts" align="left">
            </div>
            <br />
            Group name:&nbsp;<select id="seltomove"></select>
        </div>
        <div id="error" title="Error" align="center">
        </div>
        <%--********************************* END ********************************--%>
</asp:Content>
