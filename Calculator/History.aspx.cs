using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Calculator
{
    public partial class About : Page
    {
        const string connectionString = "Data Source=NET163\\SQLEXPRESS; Initial Catalog = calculatordb; User ID = admin; Password=123456; Trusted_Connection=False;";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            GetOperationsFromDatabase().ToString();
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            if(FromDateTimePicker.Text != string.Empty && ToDateTimePicker.Text != string.Empty)
            {
                errorLabel.Text = string.Empty;

                DateTime fromDateTime = Convert.ToDateTime(FromDateTimePicker.Text);
                DateTime toDateTime = Convert.ToDateTime(ToDateTimePicker.Text);

                if(DateTime.Compare(fromDateTime, toDateTime) < 0)
                {
                    for (int i = CalculationHistoryTable.Rows.Count - 1; i >= 1; i--)
                    {
                        DateTime calculationRecordDateTime = Convert.ToDateTime(CalculationHistoryTable.Rows[i].Cells[1].Text);

                        if (DateTime.Compare(calculationRecordDateTime, fromDateTime) > 0 && DateTime.Compare(calculationRecordDateTime, toDateTime) < 0)
                        {
                            //Telpa i filtravimo rezius
                        }
                        else
                        {
                            CalculationHistoryTable.Rows.Remove(CalculationHistoryTable.Rows[i]);
                        }
                    }
                }
                else
                {
                    errorLabel.Text = "From date needs to be before to Date!";
                }
            }
            else
            {
                errorLabel.Text = "Pick from and to dates!";
            }
        }

        protected void ClearSearchButton_Click(object sender, EventArgs e)
        {
            errorLabel.Text = string.Empty;

            while (CalculationHistoryTable.Rows.Count > 1)
                CalculationHistoryTable.Rows.RemoveAt(1);

            GetOperationsFromDatabase();
        }

        int GetOperationsFromDatabase()
        {
            SqlConnection cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(string.Format("SELECT id, date, line FROM Calculation"), cnn);

            try
            {
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader["id"].ToString();
                    string date = reader["date"].ToString();
                    string line = reader["line"].ToString();

                    TableRow row = new TableRow();
                    TableCell cell1 = new TableCell();
                    TableCell cell2 = new TableCell();
                    TableCell cell3 = new TableCell();

                    cell1.Text = id;
                    cell2.Text = date;
                    cell3.Text = line;

                    row.Cells.AddRange(new TableCell[] { cell1, cell2, cell3 });
                    CalculationHistoryTable.Rows.Add(row);
                }
                cnn.Close();

                return 1; //Success
            }
            catch (Exception ex)
            {
                errorLabel.Text = "Couldn't connect to the database!";
                return 0; //Error accessing database
            }
        }
    }
}