<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Calculator._Default" EnableViewState="true" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="ErrorLabel" runat="server" Text=""></asp:Label>
            <asp:Label ID="AlertLabel" runat="server" Text=""></asp:Label>
            <asp:Label ID="DebugLabel" runat="server" Text=""></asp:Label>

            <!-- Calculator selection radio buttons -->
            <asp:RadioButtonList ID="CalculatorRadioButtonList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CalculatorRadioButtonList_SelectedIndexChanged">
                <asp:ListItem Text="Basic Calculator" runat="server" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Equation Calculator" runat="server"></asp:ListItem>
                <asp:ListItem Text="Function Calculator" runat="server"></asp:ListItem>
            </asp:RadioButtonList>
            <!-- End of calculator selection radio buttons -->

            <!-- Calculator display boxes -->
            <div id="SimpleCalculatorDiv" class="div-mid" runat="server">
                <asp:TextBox ID="SimpleCalculatorDisplayTextBox" CssClass="big-input" runat="server"></asp:TextBox>
            </div>

            <div id="EquationCalculatorDiv" class="div-mid" runat="server">
                <asp:TextBox ID="EquationCalculatorDisplay1TextBox" CssClass="big-input" runat="server"></asp:TextBox>
                =
                <asp:TextBox ID="EquationCalculatorDisplay2TextBox" CssClass="small-input" runat="server" OnTextChanged="EquationCalculatorDisplay2TextBox_TextChanged"></asp:TextBox>
            </div>

            <div id="FunctionCalculatorDiv" class="div-mid" runat="server">
                <asp:Label ID="FunctionLabel" runat="server" Text="f(x)="></asp:Label><asp:TextBox ID="FunctionCalculatorDisplayTextBox" CssClass="big-input" runat="server"></asp:TextBox>
                <asp:Label ID="IntervalFromLabel" runat="server" Text="Interval start ="></asp:Label><asp:TextBox ID="IntervalFromTextBox" CssClass="small-input" runat="server"></asp:TextBox>
                <asp:Label ID="IntervalToLabel" runat="server" Text="Interval end ="></asp:Label><asp:TextBox ID="IntervalToTextBox" CssClass="small-input"  runat="server"></asp:TextBox>
                <asp:Label ID="IntervalStepLabel" runat="server" Text="Interval step ="></asp:Label><asp:TextBox ID="IntervalStepTextBox" CssClass="small-input" runat="server"></asp:TextBox>
            </div>
            <!-- End of display boxes -->

            <!-- Calculator textbox controls -->
            <div id="TextBoxControls" runat="server">
                <asp:Button ID="ButtonPrevious" runat="server" Text="<-" OnClick="ButtonPrevious_Click"/>
                <asp:Button ID="ButtonNext" runat="server" Text="->" OnClick="ButtonNext_Click" />
            </div>
            <!-- End of calculator textbox controls -->
            <asp:Label ID="ResultLabel" runat="server" Text="Result:"></asp:Label>
            <div class="calculator">
                <asp:TextBox ID="DisplayTextBox" CssClass="big-input text" runat="server"></asp:TextBox>
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
                <asp:Button ID="ButtonPower" runat="server" Text="^" CssClass="calculatorButton pow" OnClick="Operation_Click" />

                <asp:Button ID="AdditionButton" runat="server" Text="+" CssClass="calculatorButton operation plus" OnClick="Operation_Click" />
                <asp:Button ID="SubtractionButton" runat="server" Text="-" CssClass="calculatorButton operation minus" OnClick="Operation_Click" />
                <asp:Button ID="DivisionButoon" runat="server" Text="/" CssClass="calculatorButton operation div" OnClick="Operation_Click" />
                <asp:Button ID="MultiplicationButton" runat="server" Text="*" CssClass="calculatorButton operation multi" OnClick="Operation_Click" />
            </div>
            <div style="margin-top: 150px" >
                <asp:Chart ID="SolutionChart" runat="server" ImageType="Png" ImageStorageMode="UseImageLocation">
                    <Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
