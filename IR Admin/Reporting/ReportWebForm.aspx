<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportWebForm.aspx.cs" Inherits="IR_Admin.Reporting.ReportWebForm" %>
<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<form id="form1" runat="server">  
    <div>  
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>  
        <rsweb:ReportViewer ID="ReportViewer1" ZoomMode="Percent" runat="server"  Width="100%" Height="685px"></rsweb:ReportViewer>  
    </div>  
</form>  
</body>
</html>
