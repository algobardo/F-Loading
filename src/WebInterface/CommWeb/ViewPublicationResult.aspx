<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPublicationResult.aspx.cs" Inherits="WebInterface.CommWeb.ViewPublicationResult" %>
<%@ Register Assembly="Storage" Namespace="Storage" TagPrefix="st1" %>
<%@ Register Assembly="System" Namespace="System" TagPrefix="sys1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
	<title>Floading Rss Channel</title>
	<link href="/CommWeb/css/rss.css" rel="stylesheet" type="text/css" />
</head>
	<body>
		<div>
			<div class="header">  </div>
			<div class="content">
		        <%= InnerText %>
		    </div>
		</div>
	</body>
</html>