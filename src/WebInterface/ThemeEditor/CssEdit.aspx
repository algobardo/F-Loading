<%@ Page Language="C#" MasterPageFile="~/FormFillier/index.Master" AutoEventWireup="true" CodeBehind="CssEdit.aspx.cs" Inherits="WebInterface.cssEditPage" %>

<asp:Content ID="CSSEDIT_TitleContent" ContentPlaceHolderID="head" runat="server">
    <title>CSS Editor </title>
    <%--Don't modofy link order--%>
    <%--CSS --%>
    <link href="../lib/css/smoothness/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/FormFiller/contentHome.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" media="all" type="text/css" href="css/themeEditor.css" />
    <link href="css/cssEdit.css" media="all" rel="Stylesheet" type="text/css" />
    <%--Lib --%>
       
    <script type="text/javascript" charset="utf-8" src="../../lib/js/jquery-1.3.2.min.js" ></script>
    <script type="text/javascript" charset="utf-8" src="../../lib/js/jQuery.tooltips.js" ></script>
    <script type="text/javascript" charset="utf-8" src="lib/jQuery.uploader.js" ></script>
    <script language="javascript" type="text/javascript" src="edit_area/edit_area_full.js"></script>
    <script type="text/javascript">

    function submitForm()
    {
    document.getElementById('ctl00_contentHome_cssArea').value = editAreaLoader.getValue('ctl00_contentHome_cssArea');
    document.getElementById("aspnetForm").submit();
    }
    function resetForm()
    {
    $('#aspnetForm').reset();
    }

    </script>
    
    <script language="javascript" type="text/javascript">
    editAreaLoader.init({
	    id : "ctl00_contentHome_cssArea"		// textarea id
	    ,syntax: "css"			// syntax to be uses for highgliting
	    ,start_highlight: true		// to display with highlight mode on start-up
	    ,word_wrap: true
    });
    </script>

    
    
</asp:Content>

<asp:Content ID="THEME_MainContent" ContentPlaceHolderID="contentHome" runat="server">
    <div id="content-container">
    <div id="content-border" >

 <div id="themeEditor">
        <div id="generalCustomizations">
            <h2>Editing Theme CSS</h2>
   
    <p><a id="serverMsg" runat="server"></a></p>
    
    <div style="padding:0px 0px 20px 20px;">
    <div class="floaters">
        <form id="cssEditForm" method="post" action="CssEdit.aspx" enctype="multipart/form-data">
            <p>
            <% WebInterface.FloadingButton.createFloadingButton(Page, "submitCss", "Save", "submitForm()"); %>
            <% WebInterface.FloadingButton.createFloadingButton(Page, "resetCss", "Reset", "resetForm()"); %>
            <% WebInterface.FloadingButton.createFloadingButton(Page, "goback", "Back", "document.location='ThemeEditor.aspx';"); %>
            <br />
            <br />

            <textarea rows="30" cols="40" id="cssArea" style="border:dotted 2px #666;" name="S2" runat="server"></textarea>
            </p>
        </form>
        </div>
        <div class="floaters">  
        <h2>Syntax:</h2>
        <table class="section"> 
        <tr> 
          <th colspan="2">Positioning</th> 
        </tr> 
        <tr> 
          <td>clear</td> 
          <td>Any floating elements around the element?
              <div class="values">both, left, right, none</div></td> 
        </tr> 
        <tr class="evenrow"> 
          <td>float</td> 
          <td>Floats to a specified side
          <div class="values">left, right, none</div></td> 
        </tr> 
        <tr> 
          <td>left</td> 
          <td>The left position of an element
            <div class="values">auto, length values (pt, in, cm, px)</div></td> 
        </tr> 
        <tr class="evenrow"> 
          <td>top</td> 
          <td>The top position of an element
            <div class="values">auto, length values (pt, in, cm, px)</div></td> 
        </tr> 
        <tr> 
          <td> position</td> 
          <td><div class="values">static, relative, absolute</div></td> 
        </tr> 
        <tr class="evenrow"> 
          <td>z-index </td> 
          <td>Element above or below overlapping elements?
              <div class="values">auto, integer (higher numbers on top) </div></td> 
        </tr> 
      </table> 

<table class="section" border="1"> 
        <tr> 
          <th colspan="2">Borders</th> 
        </tr> 
		<tr> 
		  <td>border-width</td> 
	      <td>Width of the border</td> 
		</tr> 
		<tr class="evenrow"> 
          <td>border-style</td> 
          <td><div class="values">dashed; dotted; double; groove; inset; outset;
              ridge; solid; none</div></td> 
	    </tr> 
		<tr> 
		  <td>border-color</td> 
	      <td>Color of the border</td> 
		</tr> 
      </table> 
       <table class="section" border="1"> 
        <tr> 
          <th colspan="2">Font</th> 
        </tr> 
        <tr> 
          <td>font-style</td> 
          <td><div class="values">Italic, normal </div></td> 
        </tr> 
        <tr class="evenrow"> 
          <td>font-variant</td> 
          <td><div class="values">normal, small-caps</div></td> 
        </tr> 
        <tr> 
          <td>font-weight</td> 
          <td><div class="values">bold, normal, lighter, bolder, integer (100-900)</div></td> 
        </tr> 
        <tr class="evenrow"> 
          <td>font-size</td> 
          <td>Size of the font</td> 
        </tr> 
        <tr> 
          <td>font-family</td> 
          <td>Specific font(s) to be used</td> 
        </tr>
        <tr>
        <td>...</td>
        <td><a target="_blank" href="http://lesliefranke.com/files/reference/csscheatsheet.html">....read more(opens a new page)</a></th> 
        </td>
        </tr> 
      </table> 
      

        </div>
        </div> 
    </div>
   </div>
   
       </div>
   </div>


</asp:Content>

