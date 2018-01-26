<%@ Page Language="C#" MasterPageFile="~/FormFillier/index.Master" AutoEventWireup="true"
    CodeBehind="~/WorkflowEditor/WorkflowEditor.aspx.cs" Inherits="WebInterface.WorkflowEditor.WorkflowEditor"
    Trace="false" %>

<%@ Register Assembly="Fields" Namespace="Fields" TagPrefix="cc1" %>
<%--Page's Title--%>
<asp:Content ID="WFE_TitleContent" ContentPlaceHolderID="head" runat="server">
    <title>Editor </title>
    <%--Don't modofy link order--%>
    <%--CSS --%>
    <link href="../lib/css/smoothness/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/jquery.jgrowl.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/jquery.tooltip.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/WorkflowEditor/WFE_css/EditorInterface.css" rel="stylesheet"
        type="text/css" />
    <link href="../lib/css/FormFiller/contentHome.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/WorkflowEditor/WFF_widget_box_style.css" rel="stylesheet"
        type="text/css" />
    <link href="../lib/css/WorkflowEditor/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/WorkflowEditor/WFG/WFG_graph.css" rel="stylesheet" type="text/css" />
    <%--Lib --%>

    <script type="text/javascript" src="../lib/js/jquery.layout.min.js"></script>

    <script type="text/javascript" src="../lib/js/raphael.js"></script>

    <script type="text/javascript" src="../lib/js/WorkflowEditor/WFS_encoding.js"></script>

    <script type="text/javascript" src="../lib/js/jquery.treeview.min.js"></script>

    <script src="../lib/js/jquery.jgrowl_compressed.js" type="text/javascript"></script>

    <script src="../lib/js/jquery.tooltip.min.js" type="text/javascript"></script>

    <%--Custom script --%>

    <script src="../lib/js/WorkflowEditor/WFS_workflowStatus.js" type="text/javascript"></script>

    <script src="../lib/js/WorkflowEditor/WFG_serverComunicationUtility.js" type="text/javascript"></script>

    <script src="../lib/js/WorkflowEditor/WFG_graphEditor.js" type="text/javascript"></script>

    <script src="../lib/js/WorkflowEditor/WFF_widget_box.js" type="text/javascript"></script>

    <script src="../lib/js/WorkflowEditor/WFE_Editor.js" type="text/javascript"></script>

    <script src="../lib/js/WorkflowEditor/WFP_predicates.js" type="text/javascript"></script>

    <script src="../lib/js/WorkflowEditor/WFF_property_box.js" type="text/javascript"></script>

    <script src="../lib/js/WorkflowEditor/WFF_groups.js" type="text/javascript"></script>

    <script src="../lib/js/WorkflowEditor/WFB_buttons.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            /* Initializing the graph editor canvas */
            WFG_initialize();
            /* Initializing the editor */
            WFE_initialize();

            WFF_create_widget_list();
            WFF_enable_draggable_widgets();

            //Restoring existing wfs from cookie
            WFS_RestoreAllWorkflows();
        });
    </script>

</asp:Content>
<%--Page's body--%>
<asp:Content ID="WFE_MainContent" ContentPlaceHolderID="contentHome" runat="server">

    <script src="../lib/js/FormFiller/CommWFEGui.js" type="text/javascript"></script>

    <div id="content-container">
        <div id="content-border">
            <fieldset id="WFE-body" class="ui-layout-pane">
                <%-- WF-Editor Content --%>
                <%-- West Section --%>
                <div id="rightClickArc" class="WFG_menu">
                    <ul class="WFG_menu_ul">
                        <li class="WFG_menu_li"><a class="WFG_menu_item" href="#" onclick="WFG_rightClickMenuArcHide(); WFE_openDialog(WFG_workflow.elementSelected);">
                            Add precondition</a></li>
                        <li class="WFG_menu_li"><a class="WFG_menu_item" href="#" onclick="WFG_rightClickMenuArcHide(); WFP_addPredicateFromXml(WFG_workflow.elementSelected);">
                            Add precondition xml</a><br />
                        </li>
                        <li class="WFG_menu_li"><a class="WFG_menu_item" href="#" onclick="WFG_rightClickMenuArcHide(); WFG_workflow.elementSelected.remove();">
                            Remove Arc</a></li>
                    </ul>
                </div>
                <div id="rightClickNode" class="WFG_menu">
                    <ul class="WFG_menu_ul">
                        <li class="WFG_menu_li"><a class="WFG_menu_item" href="#" onclick="WFG_rightClickMenuNodeHide(); WFG_workflow.elementSelected.remove();">
                            Remove Node</a></li>
                    </ul>
                </div>
                <div class="ui-layout-west">
                    <div class="WFE_headers">
                        <%--WF Editor mode--%>
                        <div class="west-wf-editor ui-state-default ui-corner-all ui-helper-clearfix" style="padding: 3px;">
                            <span class="ui-icon ui-icon-pencil" style="float: left; margin: 2px 5px 0 20px;">
                            </span>Options
                        </div>
                        <%--Form Editor mode--%>
                        <div class="west-form-editor ui-state-default ui-corner-all ui-helper-clearfix hiddenpanel"
                            style="padding: 3px;">
                            <span class="ui-icon ui-icon-pencil" style="float: left; margin: 2px 5px 0 20px;">
                            </span>Widgets Box
                        </div>
                    </div>
                    <%--WF Editor mode--%>
                    <div class="WFE_content WFE_keepImageOverflow">
                        <div class="west-wf-editor">
                            <div id="WFE_WidgetBoxButtons">
                            </div>
                            <%-- DIALOG TO ADD PREDICATE USING XML SOURCE --%>
                            <div id="dialog_predicates_from_xml" title="Add preconditions using XML">
                            </div>
                            <%-- DIALOG TO ADD PRECONDITIONS --%>
                            <div id="dialog" title="Add preconditions for this edge" style="width: 0; height: 0;">
                                <span style="float: left; margin: 0 7px 20px 0;"></span>
                                <div id="precondition_webcontrols" class="hiddenpanel">
                                    <asp:UpdatePanel ID="preconditionup" runat="server">
                                        <ContentTemplate>
                                            Webcontrols for values
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <%-- DIALOG TO RECEIVE NODE's NAME --%>
                            <div id="dialog_node_name" title="Insert the step's name" style="width: 0; height: 0;">
                                <span style="float: left; margin: 0 7px 20px 0;"></span>
                            </div>
                            <%-- DIALOG TO RECEIVE IMAGE's SRC --%>
                            <div id="dialog_image_src" title="Insert the image's url" style="width: 0; height: 0;">
                                <span style="float: left; margin: 0 7px 20px 0;"></span>
                            </div>
                            <%-- DIALOG TO RECEIVE HTML's SRC --%>
                            <div id="dialog_html_src" title="Insert the html code to embed" style="width: 0;
                                height: 0;">
                                <span style="float: left; margin: 0 7px 20px 0;"></span>
                            </div>
                            <%-- DIALOG TO ADD CONSTRAINTS --%>
                            <div id="dialog_constraints" title="Add constraints to this field" style="width: 0;
                                height: 0;">
                                <p>
                                    <span style="float: left; margin: 0 7px 20px 0;"></span>
                                </p>
                            </div>
                            <%-- DIALOG TO SAVE THE WF --%>
                            <div id="dialog_saving" title="Form's description" style="width: 0; height: 0;">
                                <p>
                                    <span style="float: left; margin: 0 7px 20px 0;"></span>
                                </p>
                            </div>
                            <%-- DIALOG TO SHOW A SIMPLE TUTORIAL --%>
                            <div id="tutorial_dialog" class="hiddenpanel">
                                <object width="425" height="344">
                                    <param name="movie" value="http://www.youtube.com/v/7K3r4mj1i9c&hl=it&fs=1&"></param>
                                    <param name="allowFullScreen" value="true"></param>
                                    <param name="allowscriptaccess" value="always"></param>
                                    <embed src="http://www.youtube.com/v/7K3r4mj1i9c&hl=it&fs=1&" type="application/x-shockwave-flash"
                                        allowscriptaccess="always" allowfullscreen="true" width="425" height="344"></embed></object>
                            </div>
                            <%-- DIALOG TO CHOOSE TYPE OF WF AND PUBLISH IT --%>
                            <div id="dialog_publishing" title="Publish form" style="width: 0; height: 0; display: none;">
                                <p>
                                    <span style="float: left; margin: 0 7px 20px 0;"></span>
                                </p>
                                <div id="box">
                                    <div id="boxSx">
                                        <p class="p">
                                            Choose the form's type
                                            <select id="sele_saveWF" class="contentOption" onchange="WFE_onChangePublicationType();" title="if public the form will be available to all users, if private will be available only for the selected users ">
                                                <option id="public" value="public" selected="selected">Public</option>
                                                <option id="private" value="private">Private</option>
                                            </select>
                                        </p>
                                        <p class="p" id="form_type">
                                        </p>
                                        <p class="p">
                                            Expiration date <span class="p" id="expiration_date" title="the form will be compilable only for t he specified period"></span>
                                        </p>
                                        <p class="small_font">
                                            You will have 1 month after expiration date to collect the results. After that the
                                            publication will be completely deleted from the system
                                        </p>
                                        <p>
                                            <textarea id="wfDescription" rows="8" cols="28" class="contentOption"></textarea>
                                        </p>
                                    </div>
                                    <div id="boxDx">
                                        <p class="p">
                                            Choose a way to retrieve results</p>
                                        <select id="serviceComm" onchange="choosen(this.value)" class="contentOption">                                            
                                        </select>
                                        <br />
                                        <br />
                                        <div class="field" id="pEmail">
                                            <span>Email</span>
                                            <br />
                                            <asp:TextBox ID="email" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="wfeReq1" runat="server" ErrorMessage="Field required"
                                                Display="Dynamic" ControlToValidate="email" ValidationGroup="10"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="wfeReg1" runat="server"
                                                ErrorMessage="Email not valid" EnableClientScript="true" ValidationGroup="10"
                                                ControlToValidate="email" Display="Dynamic" ValidationExpression="[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"></asp:RegularExpressionValidator>
                                        </div>
                                        <div class="field" id="pUser">
                                            <span>Username</span>
                                            <br />
                                            <asp:TextBox ID="user" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="wfeReq2" runat="server" ErrorMessage="Field required"
                                                Display="Dynamic" ControlToValidate="user" ValidationGroup="10"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator EnableClientScript="true" ID="wfeReg2"
                                                runat="server" ErrorMessage="User not valid" ValidationGroup="10" ControlToValidate="user"
                                                Display="Dynamic" ValidationExpression="[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+)*(@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)*"></asp:RegularExpressionValidator>
                                        </div>
                                        <div class='field' id='pPass'>
                                            <span>Password</span>
                                            <br />
                                            <asp:TextBox ID="pass" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="wfeReq3" runat="server" ErrorMessage="Field required"
                                                ControlToValidate="pass" Display="Dynamic" ValidationGroup="10"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator EnableClientScript="true" Display="Dynamic" ID="wfeReg3"
                                                runat="server" ValidationExpression="[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+)*"
                                                ErrorMessage="Password not valid" ControlToValidate="pass"></asp:RegularExpressionValidator>
                                        </div>
                                        <div class='field' id='pHost'>
                                            <span>Host</span>
                                            <br />
                                            <asp:TextBox ID="host" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="wfeReq4" runat="server" ErrorMessage="Field required"
                                                ControlToValidate="host" Display="Dynamic" ValidationGroup="10"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="wfeReg4" runat="server"
                                                ErrorMessage="Host not valid" EnableClientScript="true" ControlToValidate="host"
                                                Display="Dynamic" ValidationGroup="10" ValidationExpression="[a-zA-Z0-9_.@]+"></asp:RegularExpressionValidator>
                                        </div>
                                        <div class='field' id='pDir'>
                                            <span>Directory</span>
                                            <br />
                                            <asp:TextBox ID="dir" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="wfeReq5" runat="server" ErrorMessage="Field required"
                                                ControlToValidate="dir" Display="Dynamic" ValidationGroup="10"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="wfeReg5" ValidationGroup="10"
                                                runat="server" EnableClientScript="true" ErrorMessage="Directory not valid" ControlToValidate="dir"
                                                Display="Dynamic" ValidationExpression="[a-zA-Z0-9\-\.\?\,\'\/\\\%\$#_]*"></asp:RegularExpressionValidator>
                                        </div>
                                        <br />
                                    </div>
                                </div>
                            </div>
                            <%-- DIALOG TO SET THE WF'S NAME --%>
                            <div id="dialog_to_set_WF_name" title="Set Form's Name" style="width: 0; height: 0;">
                            </div>
                        </div>
                        <%--Form Editor mode--%>
                        <div class="west-form-editor WFF_widget_box_menu hiddenpanel">
                            <div id="WFF_accordion_widgets">
                                <%-- ADDED WIDGETS (STATIC BUTTONS)--%>
                                <h2 class="WFF_accordion_widgets_header WFF_accordion_widgets_active" align="center">
                                    Controls</h2>
                                <ul class="WFF_accordion_widgets" id="WFF_widget_list">
                                </ul>
                                <h2 class="WFF_accordion_widgets_header" align="center">
                                    Special Controls</h2>
                                <ul class="WFF_accordion_widgets" id="WFF_widget_list_static">
                                </ul>
                            </div>
                            <div id="uppanel" class="hiddenpanel">
                                <asp:UpdatePanel ID="upcontrols" runat="server">
                                    <ContentTemplate>
                                        Standard Controls Previews
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div id="iconpanel" class="hiddenpanel">
                                <asp:UpdatePanel ID="upicons" runat="server">
                                    <ContentTemplate>
                                        Standard Controls Icons
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div id="renderingpanel" class="hiddenpanel">
                                <asp:UpdatePanel ID="uprendering" runat="server">
                                    <ContentTemplate>
                                        Standard Controls Tooltips
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div id="specialpanel" class="hiddenpanel">
                                <asp:UpdatePanel ID="upspecial" runat="server">
                                    <ContentTemplate>
                                        Special Controls Icons
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
                <%-- Center Section --%>
                <div class="ui-layout-center">
                    <div id="tabs" class="WFE_content">
                        <ul>
                        </ul>
                    </div>
                </div>
                <%-- East Section --%>
                <div class="ui-layout-east">
                    <div class="WFE_headers">
                        <div class="ui-state-default ui-corner-all ui-helper-clearfix" style="padding: 3px;">
                            <span class="ui-icon ui-icon-pencil" style="float: left; margin: 2px 5px 0 20px;">
                            </span>Property Box
                        </div>
                    </div>
                    <div class="WFE_content">
                        <ul id="WFF_property_box" class="treeview">
                        </ul>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
</asp:Content>
