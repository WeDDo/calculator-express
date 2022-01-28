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

namespace Calculator
{
    public partial class _Default : Page
    {
        const string rootSymbol = "√";
        const string rootSymbolBasic = "^(1/2)";

        protected void Page_Load(object sender, EventArgs e)
        {
            string toSolve = "root(2, 4)";
            Expression e1 = new Expression(toSolve);
            DebugLabel.Text = e1.calculate().ToString();
            
            if (!IsPostBack)
            {
                DisplayTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Number_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            DisplayTextBox.Text += clickedButton.Text;
        }

        protected void Operation_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            string text = DisplayTextBox.Text;

            if (text.Length > 0)
            {
                //Jeigu pirmas simbolis yra minusas ir daugiau simboliu nera nieko nedaryti
                if (Convert.ToChar(clickedButton.Text) == '-' && text[text.Length - 1] == '-' && text.Length == 1)
                {
                    return;
                }

                //Jei randamas --, simboliai pakeiciami i +
                if (Convert.ToChar(clickedButton.Text) == '-' && text[text.Length - 1] == '-')
                {
                    DisplayTextBox.Text = text.Remove(text.Length - 1) + '+';
                    return;
                }

                //Simboliu pakeitimas
                if (text[text.Length - 1] == '+' || text[text.Length - 1] == '-' || text[text.Length - 1] == '*' || text[text.Length - 1] == '/')
                {
                    DisplayTextBox.Text = text.Remove(text.Length - 1) + clickedButton.Text;
                }
                else
                {
                    DisplayTextBox.Text += clickedButton.Text;
                }
            }
            else
            {
                //Pirmo minuso pridejimas
                if (Convert.ToChar(clickedButton.Text) == '-')
                {
                    DisplayTextBox.Text += clickedButton.Text;
                }
            }
        }
        protected void ButtonResult_Click(object sender, EventArgs e)
        {

            DisplayTextBox.Text = FormatExpression(DisplayTextBox.Text);

            //Paprastos aritmetinės operacijos
            if (!DisplayTextBox.Text.Contains("x"))
            {
                Expression expression = new Expression(DisplayTextBox.Text);
                DisplayTextBox.Text = expression.calculate().ToString();
            }
            else
            {
                //Paprastos lygties pvz. [2x=2] sprendimas
                string toSolve = string.Format("solve( {0}, x, {1}, {2} )", DisplayTextBox.Text, int.MinValue, int.MaxValue);
                Expression expression = new Expression(toSolve);
                DisplayTextBox.Text = expression.calculate().ToString();

                //Su aibe sprendimas !!(reikia sugalvoti uzrasymo formata) [1. 2. 4. 6] formatas
            }
            AlertLabel.Text = "";
        }

        protected void ButtonDot_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            string text = DisplayTextBox.Text;

            if (text[text.Length - 1] != '.')
            {
                DisplayTextBox.Text += clickedButton.Text; ;
            }
        }

        protected void SingleOperation_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            if(clickedButton.Text == rootSymbol)
            {
                DisplayTextBox.Text += rootSymbol;
            }
        }

        protected void ButtonClear_Click(object sender, EventArgs e)
        {
            DisplayTextBox.Text = string.Empty;
            AlertLabel.Text = "";
        }

        protected void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (DisplayTextBox.Text.Length > 0)
            {
                DisplayTextBox.Text = DisplayTextBox.Text.Remove(DisplayTextBox.Text.Length - 1, 1);
                if (!DisplayTextBox.Text.Contains("x"))
                    AlertLabel.Text = "";
            }
        }

        protected void Bracket_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            DisplayTextBox.Text += clickedButton.Text;
        }

        protected void X_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            AlertLabel.Text = "Equation format should be in format f(x) = 0. For example: 2x=4-2*x should be 2*x-4+2*x";

            DisplayTextBox.Text += clickedButton.Text;
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
            for (int i = 0; i < expression.Length ; i++)
            {
                if(rootIndex != -1)
                {
                    if (char.IsDigit(expression[i]))
                    {
                        if (expression.Length == i + 1 || !char.IsDigit(expression[i + 1]))
                        {
                            expression = expression.Remove(rootIndex, 1);
                            expression = expression.Insert(i, rootSymbolBasic);
                            rootIndex = -1;
                        }
                    }
                }

                if(expression[i] == Convert.ToChar(rootSymbol))
                {
                    rootIndex = i;
                }
            }

            return expression;
        }
    }
}