using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using org.mariuszgromada.math.mxparser;
using System.Web.UI.DataVisualization.Charting;

namespace Calculator
{
    public partial class _Default : Page
    {
        const string rootSymbol = "√";
        const string rootSymbolBasic = "^(1/2)";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DisplayTextBox.Text = string.Empty;
                SetCalculatorVisibility(CalculatorRadioButtonList.SelectedItem.Text);
                SimpleCalculatorHelpLabel.Text = "Calculate simple numerical expressions like 2+9*(5.5+6)-6^2+(6+2)/5.";
                EquationCalculatorHelpLabel.Text = "Calculate equations with x. First field should contain x and in the second field there should only be numbers. For example 5*x+5=25.";
                FunctionCalculatorHelpLabel.Text = "Calculate function results when given an interval.";
                /*
                string toSolve = string.Format("solve( 9*x - 80 , x, -999, 999 )");
                Expression expression = new Expression(toSolve);
                DebugLabel.Text = expression.calculate().ToString();
                */
            }
        }

        protected void Number_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            AddSymbol(clickedButton.Text);
        }

        protected void Operation_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            AddSymbol(clickedButton.Text);
        }

        protected void ButtonResult_Click(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
            textBox.Text = FormatExpression(textBox.Text);

            if (SimpleCalculatorDiv.Visible)
            {
                Expression expression = new Expression(SimpleCalculatorDisplayTextBox.Text);
                DisplayTextBox.Text = FormatExpression(expression.calculate().ToString()).Replace(",", ".");
                string insertLine = string.Format("{0}={1}", expression.getExpressionString(), expression.calculate());
                InsertOperationToDatabase(insertLine);
                return;
            }
            if (EquationCalculatorDiv.Visible)
            {
                Expression expression = new Expression(EquationCalculatorDisplay2TextBox.Text);
                string equation = EquationCalculatorDisplay1TextBox.Text + (expression.calculate() * -1).ToString();
                string toSolve = string.Format("solve( {0}, x, {1}, {2} )", equation, int.MinValue, int.MaxValue);

                expression = new Expression(toSolve);
                double result = expression.calculate();
                if(result.ToString() == "NaN")
                {
                    expression = new Expression(EquationCalculatorDisplay2TextBox.Text);
                    equation = EquationCalculatorDisplay1TextBox.Text + (expression.calculate() * -1).ToString();
                    toSolve = string.Format("solve( {0}, x, {1}, {2} )", equation, 0, int.MaxValue);

                    expression = new Expression(toSolve);
                    result = expression.calculate();
                    if (result.ToString() == "NaN")
                    {
                        expression = new Expression(EquationCalculatorDisplay2TextBox.Text);
                        equation = EquationCalculatorDisplay1TextBox.Text + (expression.calculate() * -1).ToString();
                        toSolve = string.Format("solve( {0}, x, {1}, {2} )", equation, int.MinValue, 0);

                        expression = new Expression(toSolve);
                        result = expression.calculate();
                        if (result.ToString() == "NaN")
                            DebugLabel.Text += "CAN'T BE CALCULATED!";
                    }
                }
                DisplayTextBox.Text = result.ToString().Replace(",", ".");
                string insertLine = string.Format("{0}, x={1}", equation + "=0", expression.calculate());
                InsertOperationToDatabase(insertLine);

                return;
            }
            if (FunctionCalculatorDiv.Visible)
            {
                string equation = FunctionCalculatorDisplayTextBox.Text;
                double intervalStart = Convert.ToDouble(IntervalFromTextBox.Text);
                double intervalEnd = Convert.ToDouble(IntervalToTextBox.Text);
                double intervalStep = Convert.ToDouble(IntervalStepTextBox.Text);
                List<double> xValues = new List<double>();

                for (double i = intervalStart; i <= intervalEnd; i += intervalStep)
                {
                    xValues.Add(i);
                }
                SolveEquationWithInterval(equation, xValues);
                return;
            }
            DisplayTextBox.Text = DisplayTextBox.Text;
        }

        protected void ButtonDot_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            AddSymbol(clickedButton.Text);
        }

        protected void SingleOperation_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            AddSymbol(clickedButton.Text);
        }

        protected void ButtonClear_Click(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
            textBox.Text = string.Empty;
            AlertLabel.Text = "";
        }

        protected void ButtonDelete_Click(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
            if (textBox.Text.Length > 0)
            if (textBox.Text.Length > 0)
            {
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1, 1);
                if (!textBox.Text.Contains("x"))
                    AlertLabel.Text = "";
            }
        }

        protected void Bracket_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            AddSymbol(clickedButton.Text);
        }

        protected void X_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            AddSymbol(clickedButton.Text);
        }

        /// <summary>
        /// Skaičiavimų patalpinimas į duomenų bazę
        /// </summary>
        /// <param name="line">Rezultatų/Veiksmų eilutė, kuri atvaizduojama duomenų bazėje</param>
        /// <returns>1 jei pavyksta pasiekti duomenų bazę, 0 jei ne</returns>
        int InsertOperationToDatabase(string line)
        {
            SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString);
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("Calculation_Insert", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Line", SqlDbType.VarChar).Value = line;
                cmd.ExecuteNonQuery();
                cnn.Close();

                return 1; //Success
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.ToString();
                return 0; //Error accessing database
            }
        }

        /// <summary>
        /// Metodas skirtas sutvarkyti uzdavini i isprendziama. Dabar tik saknies sutvarkymo algoritmas
        /// </summary>
        /// <param name="expression">Salyga</param>
        /// <returns>Sutvarkyta salyga</returns>
        string FormatExpression(string expression)
        {
            int rootIndex = -1;
            for (int i = 0; i < expression.Length; i++)
            {
                if (rootIndex != -1)
                {
                    if (char.IsDigit(expression[i]) || expression[i] == 'x')
                    {
                        if (expression.Length == i + 1 || !char.IsDigit(expression[i + 1]))
                        {
                            expression = expression.Remove(rootIndex, 1);
                            expression = expression.Insert(i, rootSymbolBasic);
                            rootIndex = -1;
                        }
                    }
                }

                if (expression[i] == Convert.ToChar(rootSymbol))
                {
                    rootIndex = i;
                }
            }

            return expression;
        }

        void SolveEquationWithInterval(string equation, List<double> xValues)
        {
            List<double> yValues = new List<double>();
            Argument x = new Argument("x");
            Argument y = new Argument(string.Format("y = {0}", equation), x);

            SolutionChart.Titles.Add(string.Format("Solution graph for {0}", equation));
            SolutionChart.ChartAreas[0].AxisX.Title = "X";
            SolutionChart.ChartAreas[0].AxisY.Title = "Y";
            SolutionChart.ChartAreas[0].AxisX.LineWidth = 3;
            SolutionChart.ChartAreas[0].AxisY.LineWidth = 3;
            SolutionChart.ChartAreas[0].AxisX.IsMarginVisible = false;
            SolutionChart.ChartAreas[0].AxisY.IsMarginVisible = false;
            SolutionChart.ChartAreas[0].AxisX.Crossing = 0;
            SolutionChart.ChartAreas[0].AxisY.Crossing = 0;

            Series series = new Series("EquationSolve");
            series.MarkerStyle = MarkerStyle.Circle;
            series.ChartType = SeriesChartType.Spline;

            foreach (var xValue in xValues)
            {
                x.setArgumentValue(xValue);
                yValues.Add(y.getArgumentValue());
            }

            series.Points.DataBindXY(xValues, yValues);
            SolutionChart.Series.Add(series);

            string insertLine = string.Format("{0}=[{1}] when x [{2}]", y.getArgumentExpressionString(), string.Join(", ", yValues), string.Join(", ", xValues));
            InsertOperationToDatabase(insertLine);
        }

        protected void CalculatorRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var calculatorList = (RadioButtonList)sender;
            string selectedItem = calculatorList.SelectedItem.Text;
            SetCalculatorVisibility(selectedItem);
        }

        void SetCalculatorVisibility(string selectedItemText)
        {
            if (selectedItemText == "Basic Calculator")
            {
                SimpleCalculatorDiv.Visible = true;
                EquationCalculatorDiv.Visible = false;
                FunctionCalculatorDiv.Visible = false;
                ViewState["mode"] = 0;
                ViewState["currentTextBox"] = "SimpleCalculatorDisplayTextBox";
                TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                textBox.Focus();
            }

            if (selectedItemText == "Equation Calculator")
            {
                SimpleCalculatorDiv.Visible = false;
                EquationCalculatorDiv.Visible = true;
                FunctionCalculatorDiv.Visible = false;
                ViewState["mode"] = 1;
                ViewState["currentTextBox"] = "EquationCalculatorDisplay1TextBox";
                TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                textBox.Focus();
            }

            if (selectedItemText == "Function Calculator")
            {
                SimpleCalculatorDiv.Visible = false;
                EquationCalculatorDiv.Visible = false;
                FunctionCalculatorDiv.Visible = true;

                ViewState["mode"] = 2;
                ViewState["currentTextBox"] = "FunctionCalculatorDisplayTextBox";
                TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                textBox.Focus();
            }
        }

        protected void EquationCalculatorDisplay2TextBox_TextChanged(object sender, EventArgs e)
        {
            if (EquationCalculatorDisplay2TextBox.Text.Contains('x'))
            {
                ErrorLabel.Text = "Second input field should only have numbers!";
            }
        }

        void AddSymbol(string symbol)
        {
            TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
            textBox.Text += symbol;
        }

        protected void ButtonPrevious_Click(object sender, EventArgs e)
        {
            if ((int)ViewState["mode"] == 0)
            {
                if (ViewState["currentTextBox"].ToString() == "SimpleCalculatorDisplayTextBox")
                {
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }
                return;
            }

            if ((int)ViewState["mode"] == 1)
            {
                if (ViewState["currentTextBox"].ToString() == "EquationCalculatorDisplay2TextBox")
                {
                    ViewState["currentTextBox"] = "EquationCalculatorDisplay1TextBox";
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }
                return;
            }

            if ((int)ViewState["mode"] == 2)
            {
                if (ViewState["currentTextBox"].ToString() == "IntervalStepTextBox")
                {
                    ViewState["currentTextBox"] = "IntervalToTextBox";
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }

                if (ViewState["currentTextBox"].ToString() == "IntervalToTextBox")
                {
                    ViewState["currentTextBox"] = "IntervalFromTextBox";
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }

                if (ViewState["currentTextBox"].ToString() == "IntervalFromTextBox")
                {
                    ViewState["currentTextBox"] = "FunctionCalculatorDisplayTextBox";
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }
                return;
            }
        }

        protected void ButtonNext_Click(object sender, EventArgs e)
        {
            if ((int)ViewState["mode"] == 0)
            {
                if (ViewState["currentTextBox"].ToString() == "SimpleCalculatorDisplayTextBox")
                {
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }
                return;
            }

            if ((int)ViewState["mode"] == 1)
            {
                if (ViewState["currentTextBox"].ToString() == "EquationCalculatorDisplay1TextBox")
                {
                    ViewState["currentTextBox"] = "EquationCalculatorDisplay2TextBox";
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }
                return;
            }

            if ((int)ViewState["mode"] == 2)
            {
                if (ViewState["currentTextBox"].ToString() == "FunctionCalculatorDisplayTextBox")
                {
                    ViewState["currentTextBox"] = "IntervalFromTextBox";
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }

                if (ViewState["currentTextBox"].ToString() == "IntervalFromTextBox")
                {
                    ViewState["currentTextBox"] = "IntervalToTextBox";
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }

                if (ViewState["currentTextBox"].ToString() == "IntervalToTextBox")
                {
                    ViewState["currentTextBox"] = "IntervalStepTextBox";
                    TextBox textBox = (TextBox)TextBoxControls.FindControl(ViewState["currentTextBox"].ToString());
                    textBox.Focus();
                    return;
                }
                return;
            }
        }
    }
}