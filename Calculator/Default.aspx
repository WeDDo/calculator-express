<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Calculator._Default" EnableViewState="true" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="ErrorLabel" runat="server" Text=""></asp:Label>
            <asp:Label ID="Alert0Label" runat="server" Text=""></asp:Label>
            <asp:Label ID="Alert1Label" runat="server" Text=""></asp:Label>
            <asp:Label ID="DebugLabel" runat="server" Text=""></asp:Label>
            <div class="calculator">
                <asp:TextBox ID="DisplayTextBox" CssClass="display text" runat="server"></asp:TextBox>

                <asp:Button ID="Button1" runat="server" Text="1" CssClass="calculatorButton one" OnClick="Number_Click" />
                <asp:Button ID="Button2" runat="server" Text="2" CssClass="calculatorButton two" OnClick="Number_Click" />
                <asp:Button ID="Button3" runat="server" Text="3" CssClass="calculatorButton three" OnClick="Number_Click" />

                <asp:Button ID="Button4" runat="server" Text="4" CssClass="calculatorButton four" OnClick="Number_Click" />
                <asp:Button ID="Button5" runat="server" Text="5" CssClass="calculatorButton five" OnClick="Number_Click" />
                <asp:Button ID="Button6" runat="server" Text="6" CssClass="calculatorButton six" OnClick="Number_Click" />

                <asp:Button ID="Button7" runat="server" Text="7" CssClass="calculatorButton seven" OnClick="Number_Click" />
                <asp:Button ID="Button8" runat="server" Text="8" CssClass="calculatorButton eight" OnClick="Number_Click" />
                <asp:Button ID="Button9" runat="server" Text="9" CssClass="calculatorButton nine" OnClick="Number_Click" />

                <asp:Button ID="Button0" runat="server" Text="0" CssClass="calculatorButton zero" OnClick="Number_Click" />
                <asp:Button ID="ButtonDot" runat="server" Text="." CssClass="calculatorButton dot" OnClick="ButtonDot_Click" />
                <asp:Button ID="ButtonResult" runat="server" Text="=" CssClass="calculatorButton result" OnClick="ButtonResult_Click" />

                <asp:Button ID="ButtonDelete" runat="server" Text="DEL" CssClass="calculatorButton del" OnClick="ButtonDelete_Click" />
                <asp:Button ID="ButtonClear" runat="server" Text="CLR" CssClass="calculatorButton clear" OnClick="ButtonClear_Click" />
                <asp:Button ID="ButtonRoot" runat="server" Text="√" CssClass="calculatorButton root" OnClick="SingleOperation_Click" />

                <asp:Button ID="ButtonLBracked" runat="server" Text="(" CssClass="calculatorButton lbracket" OnClick="Bracket_Click" />
                <asp:Button ID="ButtonRBracked" runat="server" Text=")" CssClass="calculatorButton rbracket" OnClick="Bracket_Click" />
                <asp:Button ID="ButtonX" runat="server" Text="x" CssClass="calculatorButton x" OnClick="X_Click" />

                <asp:Button ID="AdditionButton" runat="server" Text="+" CssClass="calculatorButton operation plus" OnClick="Operation_Click" />
                <asp:Button ID="SubtractionButton" runat="server" Text="-" CssClass="calculatorButton operation minus" OnClick="Operation_Click" />
                <asp:Button ID="DivisionButoon" runat="server" Text="/" CssClass="calculatorButton operation div" OnClick="Operation_Click" />
                <asp:Button ID="MultiplicationButton" runat="server" Text="*" CssClass="calculatorButton operation multi" OnClick="Operation_Click" />

                <asp:Button ID="ButtonLIntervalBracket" runat="server" Text="[" CssClass="calculatorButton operation libracket" OnClick="Bracket_Click" />
                <asp:Button ID="ButtonRLntervalBracket" runat="server" Text="]" CssClass="calculatorButton operation ribracket" OnClick="Bracket_Click" />
                <asp:Button ID="ButtonInterval" runat="server" Text=" interval" CssClass="calculatorButton operation interval" OnClick="Bracket_Click" />
                <asp:Button ID="IntervalSeperator" runat="server" Text=";" CssClass="calculatorButton operation intervalSeperator" OnClick="Bracket_Click" />
                <asp:Button ID="ButtonSolveRange" runat="server" Text=" solve" CssClass="calculatorButton operation solve" OnClick="Bracket_Click" /> 
            </div>
            <div>
                <asp:Chart ID="SolutionChart" CssClass="margin-mid" runat="server" ImageType="Png" ImageStorageMode="UseImageLocation">
                    <Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
