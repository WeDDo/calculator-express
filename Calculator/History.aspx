<%@ Page Title="Calculator history" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="Calculator.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <h3>History of your calculations:</h3>
    <asp:Label ID="errorLabel" ForeColor="Red" runat="server" Text=""></asp:Label>
    <div>
        <asp:Label ID="FromDateTime" runat="server" Text="From: "></asp:Label><asp:TextBox ID="FromDateTimePicker" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
        <asp:Label ID="ToDateTime" runat="server" Text="To: "></asp:Label><asp:TextBox ID="ToDateTimePicker" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
        <asp:Button ID="ClearSearchButton" runat="server" Text="Show all" OnClick="ClearSearchButton_Click" />
        <hr />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Table ID="CalculationHistoryTable" runat="server">
                    <asp:TableRow>
                        <asp:TableHeaderCell Width="5em">Id</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="15em">Date</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="30em">Calculation</asp:TableHeaderCell>
                    </asp:TableRow>
                </asp:Table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
