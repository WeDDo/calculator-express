<%@ Page Title="Calculator history" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="Calculator.About" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h2><%: Title %></h2>
            <h3>History of your calculations:</h3>
            <asp:Label ID="errorLabel" ForeColor="Red" runat="server" Text=""></asp:Label>
            <asp:Label ID="Label1" ForeColor="Red" runat="server" Text=""></asp:Label>
            <div style="width: 100%; margin: 0 auto;">
                <asp:Label ID="FromDateTime" runat="server" Text="From: "></asp:Label><asp:TextBox ID="FromDateTimePicker" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
                <asp:Label ID="ToDateTime" runat="server" Text="To: "></asp:Label><asp:TextBox ID="ToDateTimePicker" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
                <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
                <asp:Button ID="ClearSearchButton" runat="server" Text="Show Current Month (Default)" OnClick="ClearSearchButton_Click" />
                <hr />

                <asp:GridView ID="CalculationHistoryGridView" CssClass="historyTable" Width="100%" OnPageIndexChanging="CalculationHistoryGridView_PageIndexChanging" AllowPaging="true" PageSize="10" runat="server">
                </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
