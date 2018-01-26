<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetSession.aspx.cs" Inherits="WebInterface.ResetSession" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
            Text="Reset Session" />
        <br />
        <br />
        <asp:Literal ID="Literal2" runat="server"></asp:Literal>
        <br />
    
    </div>
    <asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
        Text="Create Many Publication" />
    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
    <br />
    <br />
    <asp:Button ID="Button3" runat="server" onclick="Button3_Click" 
        Text="Delete All Publication" />
    <br />
    <br />
    <asp:Button ID="Button4" runat="server" onclick="Button4_Click" 
        Text="ReloadTypes" />
    </form>
</body>
</html>
