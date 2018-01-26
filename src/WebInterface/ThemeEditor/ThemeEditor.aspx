<%@ Page Language="C#" MasterPageFile="~/FormFillier/index.Master" AutoEventWireup="true" CodeBehind="ThemeEditor.aspx.cs" 
    Inherits="WebInterface.ThemeEditor.ThemeEditor" %>
    
<%--Page's Title--%>
<asp:Content ID="THEME_TitleContent" ContentPlaceHolderID="head" runat="server">
    <title>Theme Editor </title>
    <%--Don't modofy link order--%>
    <%--CSS --%>
    <link href="../lib/css/smoothness/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/FormFiller/contentHome.css" rel="stylesheet" type="text/css" />
    <%--Lib --%>
    
    <link rel="stylesheet" media="all" type="text/css" href="css/toolbox.css" />
    <link rel="stylesheet" media="all" type="text/css" href="css/tooltip.css" />
    <link rel="stylesheet" media="all" type="text/css" href="css/gccolor.css" />
    <link href="../lib/css/FormFiller/contentPresentation.css" media="screen" type="text/css" rel="Stylesheet" /> 
    <link rel="stylesheet" media="all" type="text/css" href="css/themeEditor.css" />
    <link href="../lib/css/jquery.jgrowl.css" rel="stylesheet" type="text/css" />

    
    <script type="text/javascript" charset="utf-8" src="../../lib/js/jquery-1.3.2.min.js" ></script>
    
    <script type="text/javascript" charset="utf-8" src="../../lib/js/jquery-ui-te.min.js" ></script>

    <script src="../lib/js/jquery.jgrowl_compressed.js" type="text/javascript"></script>
    <script src="lib/jquery.tools.min.js" type="text/javascript"></script>
    <script type="text/javascript" charset="utf-8" src="lib/jquery.gccolor.js" ></script>
    <script type="text/javascript" charset="utf-8" src="../../lib/js/jQuery.tooltips.js" ></script>
    
    
    <script type="text/javascript" charset="utf-8" src="lib/themeeditor.js" ></script>
    <script type="text/javascript" charset="utf-8" src="lib/toolbox.js" ></script>
</asp:Content>


<%--Page's body--%>
<asp:Content ID="THEME_MainContent" ContentPlaceHolderID="contentHome" runat="server">
    <div id="content-container">
    <div id="content-border">
    
    <!-- Needed by asp... //-->
    <form id="ASPXForm"></form>
    
    <div id="themeEditor">
        <div id="generalCustomizations">
            <h2>General Page Customization</h2>
            <form onsubmit="alert('asd');">
                <div class="floaters">
                <h3>Page Title and Logo</h3>
                <ul>
                <li><label for="titleEditBox">Form Title:</label><input type="text" id="titleEditBox" value="Default Title" onchange="changeTitle( this.value )" onblur="changeTitle( this.value )"/></li>
                <li><label for="color_formTitle">Title Color:</label><input class="gccolor" onblur="toolbox.applyChanges('ctl00_contentHome_formTitle','color', this.value, 'color' );" title="Color of the Form Title. Example: #ff00ff" id="color_formTitle" type="text"/></form></li>
                <li><form id="logo_upload_form" method="post" enctype="multipart/form-data" target="img_upload_target" onsubmit="startUpload();" action="SaveLogo.aspx"><label>Upload logo</label><input id="imgfile" name="imgfile" type="file" size="15" /><input type="submit" name="submitBtn" value="Upload" /><iframe id="img_upload_target" name="img_upload_target" src="#" style="width:0;height:0;border:0px solid #fff;"></iframe><a id="logo-delete-response" style="display:none"></a> <img id="logo-delete" src="resources/delete.gif" onclick="logoDelete();" style="display:none" alt="delete current logo"/></form></li>
                <li><label for="titleEditBox">Title Size:</label><input type="text" id="titleSize" onblur="toolbox.applyChanges('ctl00_contentHome_formTitle','font-size', this.value, 'size' );"/></li>
                </ul>
                </div><div class="floaters">
                <h3>Form style</h3>
                <ul>
                <li><label for="background-color_presenPanel">Form Background:</label><input class="gccolor" onblur="toolbox.applyChanges('ctl00_contentHome_contentAll','background-color', this.value, 'color' );" title="Color of the Form background. Example: #ff00ff" id="background-color_presenPanel" type="text"/></li>
                <li><label for="color_inputLabel">Input Label Color:</label><input rel="label" class="gccolor" onblur="toolbox.applyChanges('label','color', this.value, 'color' );" title="Color of the Input Label. Example: #ff00ff" id="color_inputLabel" type="text"/><div style="display:none;"><input id="labelApplyToID" type="checkbox" /></div></li>
                <li><div id="editStaticLabel" runat="server"></div></li>
                </ul>
                </div>
            </form>
                       

        </div>
        <div id="fieldCustomization">
        <h2>Field customization and Preview</h2>
        <div id="ctl00_contentHome_contentAll">
                <div id="formHeaderContainer">
                    <div id="imageContainer">
                        <img id="logo_upload_img" src="resources/ajax-loader.gif" alt="loading" style="display:none;" />
                        <div id="logo-div"></div>
                     </div>
                     <div id="titleContainer" style="float:none">
                        <p id="ctl00_contentHome_formTitle">Default Title</p>
                     </div>
                </div>
       

        <form name="form2" id='form2' action="SaveTheme.aspx" method="post" enctype="multipart/form-data">
            <div id="presenPanel" runat="server">
            </div>
            
            <br />
            <% WebInterface.FloadingButton.createFloadingButton(Page, "showCss", "Show CSS", "showCSS()"); %>
            <% WebInterface.FloadingButton.createFloadingButton(Page, "editCss", "Edit CSS", "document.location='CssEdit.aspx'"); %>
            <% WebInterface.FloadingButton.createFloadingButton(Page, "submitFormData", "Submit", "submitFormData()"); %>
            <% WebInterface.FloadingButton.createFloadingButton(Page, "skipThemeEditor", "Skip", "skipThemeEditor()"); %>
            <textarea cols="0" rows="0" name="generatedCss" id="generatedCss" style="visibility:hidden"></textarea>
            <input type="hidden" name="generatedTitle" id="generatedTitle" />
        </form>
        

        <script type="text/javascript">

            
            

            $("input[value='Add']").hide();
            $("input[value='Remove']").hide();
            $("input[type='radio']").each(function() {

                var radio = document.getElementById($(this).attr('id'));
                radio.setAttribute('onClick', '');


            });

           /* $('.ratingStar > a').each(function() {
                
                var cnt = $(this).contents();
                $(this).replaceWith(cnt);


            });*/
            $(function() {
                // setup ul.tabs to work as tabs for each div directly under div.panes 
                $("ul.tabs").tabs("div.panes > div");
            });
            
            
            $('#background-color_presenPanel').gccolor({
                onChange: function(target, color) { 
                            target.val('#' + color); 
                            $("#ctl00_contentHome_contentAll").css('background','color');
                            $(target).trigger('onblur'); 
                         }
             });
             $('#color_formTitle').gccolor({
                 onChange: function(target, color) {
                     target.val('#' + color);
                     $(target).trigger('onblur');
                 }
             });
             $('#color_inputLabel').gccolor({
                 onChange: function(target, color) {
                     target.val('#' + color);
                     $(target).trigger('onblur');
                 }
             });
             $('#color_staticLabel').gccolor({
                 onChange: function(target, color) {
                     target.val('#' + color);
                     $(target).trigger('onblur');
                 }
             });
        </script>
        </div>
        </div>
    </div>
    
    </div>
    </div>
    
</asp:Content>



