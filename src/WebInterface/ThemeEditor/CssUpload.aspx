<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CssUpload.aspx.cs" Inherits="WebInterface._CssUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Css Upload Test</title>
    <style type="text/css">
        #TextArea2
        {
            height: 356px;
            width: 532px;
        }
    </style>
</head>
<body>
    
    <h1>Upload Css File:</h1>
    <div id="uploadDiv" >
    <form id="UploadForm" method="post" enctype="multipart/form-data" runat="server">
        <input type="file" id="cssFile" name="cssFile" runat="server" />
        <input type="submit" id="cssSubmit" value="Upload" runat="server" name="cssSubmit" />
    </form>
    </div>
    
    <div style="margin-bottom:60px;">
        <div id="result" runat="server" visible="false" style="float:left;">
        </div>
        <div id="img" style="float:left;">
            <img id="resultIMG" alt="" src="ThemeEditor/resources/102.png" visible="false" runat="server" />
        </div>
    </div>
    
    <h1>Css Files:</h1>
    <div id="cssFiles" runat="server"></div>

    

</body>
</html>
