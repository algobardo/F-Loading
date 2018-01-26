<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/FormFillier/index.Master"
    CodeBehind="index.aspx.cs" Inherits="Floading._Default" %>

<%@ MasterType VirtualPath="~/FormFillier/index.Master" %>
<asp:Content runat="server" ContentPlaceHolderID="head" ID="head">
    <link href="../lib/css/smoothness/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="../lib/css/FormFiller/contentHome.css" type="text/css" rel="Stylesheet"
        media="screen" />
    <link rel="shortcut icon" href="../lib/image/favicon.ico" />

    <script src="../lib/js/jquery-1.3.2.min.js" type="text/javascript"></script>

    <script src="../lib/js/jquery-ui-1.7.1.custom.min.js" type="text/javascript"></script>

    <script type="text/javascript" src="../lib/js/FormFiller/homeJs.js"></script>

    <script type="text/javascript" src="../lib/js/AnimeJ.js"></script>

    <title>F-Loading</title>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="contentHome" ID="home">
    <div id="content-container">
        <div id="content-border">
            <fieldset>
                <script type="text/javascript">
                    if (navigator.userAgent.indexOf("Firefox") < 0)
                        document.write("\
					<center>\
                        <small>This website is best viewed in:</small><br />\
                        <a href='http://www.mozilla.com/?from=sfx&amp;uid=0&amp;t=309'>\
                            <img src='http://sfx-images.mozilla.org/affiliates/Buttons/firefox3/468x60.png' alt='Spread Firefox Affiliate Button' border='0' />\
                        </a>\
                    </center>\
                    ");
                </script>                
                <div id="helpAlone">
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/lib/image/helpOK.png"
                        Title="Help" />
                    <br />
                    <br />
                    <div id="playPause" style="height: 40px; margin-top: -58px; float: right">
                        <img id="Img1" title="Play/Pause" alt="pause" src="../lib/image/pause2.png" onclick="pausa()" />
                    </div>
                    <div id="tutorial_dialog" style="border: 1px solid #A7A7A7">
                        <object height="450" width="624">
                            <param name="movie" value="http://www.youtube.com/v/zn8NCqBx7h0&hl=it&fs=1&"></param>
                            <param name="wmode" value="transparent" />
                            <param name="allowFullScreen" value="true"></param>
                            <param name="allowscriptaccess" value="always"></param>
                            <embed src="http://www.youtube.com/v/zn8NCqBx7h0&hl=it&fs=1&" wmode="transparent"
                                type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true"
                                width="624" height="450"></embed></object>
                    </div>
                    <div id="contentHelp" style="position: relative; height: 60px; margin-top: 5px">
                        <asp:Panel ID="HelpContent" Height="60" CssClass="content" ForeColor="Black" runat="server">
                            To have more information see the video above that explain you how to use the interface
                            and will show you an example of form.<br />
                            If you want more information contact Loa-Group 2009 at floading2009@gmail.com.
                        </asp:Panel>
                    </div>
                </div>
                <%--<div id="stat">
                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/lib/image/statsOK.png"
                        Title="Statistics" />
                    <asp:Panel ID="StatContent" runat="server">
                        <br />
                        <br />
                        <select name="seleziona">
                            <option onclick="costruisci(1)">Share per il 21-4-09</option>
                            <option onclick="costruisci(2)">Quanti andranno a votare</option>
                        </select>
                        <br />
                        <div id="StatC">
                        </div>
                    </asp:Panel>
                </div>--%>
            </fieldset>
        </div>

        <script type="text/javascript">
        </script>

    </div>
</asp:Content>
