<%@ Page Title="Contact" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.vb" Inherits="AseelahWebApps.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">

        <div class="container" style="margin-top">
            <div class="row">
                <div class="col">

                    <h2 id="title" style="margin-top:20px;"><%: Title %></h2>
                    <hr />
                    <p><strong>IT Department</strong></p>


                    <%--<address>
                        <abbr title="Phone">P:</abbr>
                    </address>--%>

                    <address>
                        Email: <a href="mailto:idabu@aseelah.com">idabu@aseelah.com</a><br />
                        <a href="mailto:akhan@alaseel.com" style="margin-left:46px;">akhan@alaseel.com</a><br />
                    </address>

                </div>
            </div>


        </div>
    </main>
</asp:Content>
