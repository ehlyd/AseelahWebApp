<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ExportItemMaster.aspx.vb" Inherits="AseelahWebApps.ExportItemMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <br />
    <div class="container" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke; width: 50%;  margin-left: 10%;">
        <h6 style="margin-top: 20px; margin-bottom: 20px;">Export Item Master</h6>
        <hr />

        <div class="row" style="margin-bottom: 20px; width: 100%;">
            <div class="col-2">
                <asp:Label ID="Label2" class="col-form-label-sm" runat="server" Text="File Format"></asp:Label>
            </div>
            <div class="col-10">
                <asp:RadioButton ID="rdoWindows" class="form-control-sm"  runat="server" GroupName="FileFormat" Text="Windows Device" />
                <asp:RadioButton ID="rdoAndroid" class="form-control-sm" Style="margin-left: 20px;" runat="server" GroupName="FileFormat" Text="Android Device" Checked="True" />
            </div>

        </div>

        <div class="row" style="margin-bottom: 20px; width: 100%;">
            <div class="col-2">
                <asp:Label ID="Label1" class="col-form-label-sm" runat="server" Text="Organization"></asp:Label>

            </div>
            <div class="col-10">
                <asp:DropDownList ID="ddListOrganization" class="form-control-sm" Style="width: 100%; max-width: 100%;" runat="server" AutoPostBack="True"></asp:DropDownList>
            </div>
        </div>

        <div class="row" style="margin-bottom: 10px; width: 100%;">
            <div class="col-2"></div>
            <div class="col-10" style="display:flex; align-items:end; justify-content:end;">
                <asp:Button ID="btnExport" class="btn btn-secondary btn-sm" style="width:120px;" runat="server" Text="Export" onclientclick="showWaitCursor()"/>
                <asp:Button ID="btnDownload" class="btn btn-secondary btn-sm" Style="margin-left: 10px; width:120px;" runat="server" Text="Download File" />
            </div>
        </div>

    </div>
</asp:Content>
