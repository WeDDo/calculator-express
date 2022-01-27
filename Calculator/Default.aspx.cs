using System;
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
        const string connectionString = "Data Source=NET163\\SQLEXPRESS; Initial Catalog = calculatordb; User ID = admin; Password=123456; Trusted_Connection=False;";
        const string rootSymbol = "√";
        const string dotSymbol = ".";
        const string minusSymbol = "-";
        const string rootSymbolDatabase = "^(1/2)";
        const int calculatorAccuracy = 9;

        protected void Page_Load(object sender, EventArgs e)
        {
            Expression e1 = new Expression("2x=4");
            Label1.Text = e1.calculate().ToString();
            if (!IsPostBack)
            {
                DisplayTextBox.Text = string.Empty;
                ViewState["number1"] = null;
                ViewState["operation"] = null;
                ViewState["clearOnNextNumber"] = false;
                ViewState["number2Started"] = false;
                ViewState["dotUsed"] = false;
                ViewState["negUsed"] = false;
            }
        }
 
        protected void Number_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            if ((bool)ViewState["clearOnNextNumber"])
            {
                DisplayTextBox.Text = string.Empty;
                ViewState["clearOnNextNumber"] = false;
                ViewState["number2Started"] = true;
                ViewState["dotUsed"] = false;
                ViewState["negUsed"] = false;
            }
            DisplayTextBox.Text += clickedButton.Text;
        }

        protected void Operation_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            if(ViewState["operation"] == null)
            {
                ViewState["number1"] = DisplayTextBox.Text;
                ViewState["operation"] = clickedButton.Text;
                ViewState["clearOnNextNumber"] = true;
            }
            else
            {
                if (!(bool)ViewState["number2Started"])
                {
                    //Galima pakeisti operacija
                    ViewState["operation"] = clickedButton.Text;
                }
                else
                {
                    //Operacijos pakeisti nebegalima pradejus rasyti antra skaiciu
                }
            }
        }
        protected void ButtonResult_Click(object sender, EventArgs e)
        {
            double number1 = 0;
            double number2 = 0;
            char operation = ' ';
            double result = 0;
            string resultLine = string.Empty;

            if(ViewState["number1"] != null && (bool)ViewState["number2Started"])
            {
                number1 = Convert.ToDouble(ViewState["number1"]);
                number2 = Convert.ToDouble(DisplayTextBox.Text);
                operation = Convert.ToChar(ViewState["operation"]);

                result = Calculate(number1, number2, operation);
                resultLine = string.Format("{0}{1}{2}={3}", number1, operation, number2, result);

                if (result.ToString().Contains(dotSymbol))
                {
                    ViewState["dotUsed"] = true;
                }
                if (result.ToString().Contains(minusSymbol))
                {
                    ViewState["minusUsed"] = true;
                }

                ViewState["operation"] = null;
                ViewState["number2Started"] = false;
                DisplayTextBox.Text = result.ToString();
                InsertOperationToDatabase(resultLine);
            }
        }

        protected void ButtonDot_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            if (DisplayTextBox.Text.Length != 0)
            {
                if (!(bool)ViewState["dotUsed"])
                {
                    DisplayTextBox.Text += clickedButton.Text;
                    ViewState["dotUsed"] = true;
                }
                else
                {
                    //Negalima deti kablelio, niekas nevyksta
                }
            }
        }

        protected void SingleOperation_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            double result = 0;
            string resultLine = string.Empty;
            if(clickedButton.Text == rootSymbol)
            {
                //Suskaiciuoti sakni DABARTINIO skaiciaus
                result = Math.Sqrt(Convert.ToDouble(DisplayTextBox.Text));
                resultLine = string.Format("{0}{1}={2}", DisplayTextBox.Text, rootSymbolDatabase, result);

                if (result.ToString().Contains(dotSymbol))
                {
                    ViewState["dotUsed"] = true;
                }
                if (result.ToString().Contains(minusSymbol))
                {
                    ViewState["minusUsed"] = true;
                }
                DisplayTextBox.Text = result.ToString();
                InsertOperationToDatabase(resultLine);
            }
        }

        protected void ButtonClear_Click(object sender, EventArgs e)
        {
            DisplayTextBox.Text = string.Empty;
            ViewState["number1"] = null;
            ViewState["operation"] = null;
            ViewState["clearOnNextNumber"] = false;
            ViewState["number2Started"] = false;
            ViewState["dotUsed"] = false;
            ViewState["negUsed"] = false;
        }

        protected void ButtonDelete_Click(object sender, EventArgs e)
        {
            if(DisplayTextBox.Text.Length > 0)
            {
                if(DisplayTextBox.Text[DisplayTextBox.Text.Length - 1] == Convert.ToChar(dotSymbol))
                {
                    ViewState["dotUsed"] = false;
                }
                else if (DisplayTextBox.Text[DisplayTextBox.Text.Length - 1] == Convert.ToChar(minusSymbol))
                {
                    ViewState["negUsed"] = false;
                }
                else
                {
                    //Digit is removed
                }
                DisplayTextBox.Text = DisplayTextBox.Text.Remove(DisplayTextBox.Text.Length - 1, 1);
            }
        }

        protected void ButtonNegative_Click(object sender, EventArgs e)
        {
            if (!(bool)ViewState["negUsed"])
            {
                DisplayTextBox.Text = DisplayTextBox.Text.Insert(0, minusSymbol);
                ViewState["negUsed"] = true;
            }
            else
            {
                DisplayTextBox.Text = DisplayTextBox.Text.Remove(0, 1);
                ViewState["negUsed"] = false;
            }
        }

        /// <summary>
        /// Atliekama viena iš keturių paprastų skaičiuotuvo operacijų
        /// </summary>
        /// <param name="number1">Pirmas skaičius</param>
        /// <param name="number2">Antras skaičius</param>
        /// <param name="operation">Operacija atlikimui</param>
        /// <returns>Gautas skaičius atlikus veiksmą su skaičiais</returns>
        double Calculate(double number1, double number2, char operation)
        {
            double result = 0;
            if (operation == '+')
            {
                result = number1 + number2;
            }
            else if (operation == '-')
            {
                result = number1 - number2;
            }
            else if (operation == '/')
            {
                result = number1 / number2;
            }
            else if (operation == '*')
            {
                result = number1 * number2;
            }

            ViewState["result"] = result;
            return result;
        }

        /// <summary>
        /// Skaičiavimų patalpinimas į duomenų bazę
        /// </summary>
        /// <param name="line">Rezultatų/Veiksmų eilutė, kuri atvaizduojama duomenų bazėje</param>
        /// <returns>1 jei pavyksta pasiekti duomenų bazę, 0 jei ne</returns>
        int InsertOperationToDatabase(string line)
        {
            SqlConnection cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(string.Format("INSERT Calculation (date, line) VALUES (CURRENT_TIMESTAMP, '{0}')", line), cnn);

            try
            {
                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();

                return 1; //Success
            }
            catch (Exception ex)
            {
                return 0; //Error accessing database
            }
        }
    }
}