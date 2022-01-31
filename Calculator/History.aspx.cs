using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Calculator
{
    public partial class About : Page
    {
        protected DataTable fullDataTable;
        const string databaseDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["searchActive"] = false;
                fullDataTable = new DataTable();
                ViewState["fullDataTable"] = fullDataTable;
                SelectCalculations(fullDataTable);
                //GetInitialCalculationsList();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            ViewState["searchActive"] = true;

            if (FromDateTimePicker.Text != string.Empty && ToDateTimePicker.Text != string.Empty)
            {
                errorLabel.Text = string.Empty;

                DateTime fromDateTime = Convert.ToDateTime(FromDateTimePicker.Text);
                DateTime toDateTime = Convert.ToDateTime(ToDateTimePicker.Text);

                if (DateTime.Compare(fromDateTime, toDateTime) < 0)
                {
                    DataTable searchDataTable = GetSearchDataTable(fromDateTime, toDateTime);
                    ViewState["searchDataTable"] = searchDataTable;
                    CalculationHistoryGridView.DataSource = searchDataTable;
                    CalculationHistoryGridView.DataBind();
                    CalculationHistoryGridView.PageIndex = 1;
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
            ViewState["searchActive"] = false;
            errorLabel.Text = string.Empty;

            CalculationHistoryGridView.DataSource = (DataTable)ViewState["fullDataTable"];
            CalculationHistoryGridView.DataBind();
            CalculationHistoryGridView.PageIndex = 1;
        }

        bool SelectCalculations(DataTable dt)
        {
            SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString);

            DataRow dr = null;
            dt.Columns.Add("Id");
            dt.Columns.Add("Date");
            dt.Columns.Add("Expression");

            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("Calculation_Select", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader["id"].ToString();
                    string date = reader["date"].ToString();
                    string line = reader["line"].ToString();

                    dr = dt.NewRow();
                    dr["Id"] = id;
                    dr["Date"] = date;
                    dr["Expression"] = line;
                    dt.Rows.Add(dr);
                }
                cnn.Close();
                dt.AcceptChanges();

                ViewState["fullDataTable"] = fullDataTable;
                CalculationHistoryGridView.DataSource = fullDataTable;
                CalculationHistoryGridView.DataBind();

                return true; //Success
            }
            catch (Exception ex)
            {
                errorLabel.Text = "Couldn't connect to the database!";
                return false; //Error accessing database
            }
        }

        bool SelectCalculationByDateTime(DateTime fromDateTime, DateTime toDateTime, DataTable dataTable)
        {
            SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString);

            DataRow dr = null;
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Expression");

            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("Calculation_Search_ByDateTime", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@From_DateTime", SqlDbType.DateTime).Value = fromDateTime.ToString(databaseDateTimeFormat);
                cmd.Parameters.Add("@To_DateTime", SqlDbType.DateTime).Value = toDateTime.ToString(databaseDateTimeFormat);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader["id"].ToString();
                    string date = reader["date"].ToString();
                    string line = reader["line"].ToString();

                    dr = dataTable.NewRow();
                    dr["Id"] = id;
                    dr["Date"] = date;
                    dr["Expression"] = line;
                    dataTable.Rows.Add(dr);
                }
                cnn.Close();
                dataTable.AcceptChanges();

                return true; //Success
            }
            catch (Exception ex)
            {
                //Label1.Text += ex.ToString();
                return false; //Error accessing database
            }
        }

        DataTable GetSearchDataTable(DateTime fromDateTime, DateTime toDateTime)
        {
            DataTable searchDataTable = new DataTable();
            SelectCalculationByDateTime(fromDateTime, toDateTime, searchDataTable);
            return searchDataTable;
        }

        void GetInitialCalculationsList()
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);
            DataTable dataTable = new DataTable();
            SelectCalculationByDateTime(firstDayOfMonth, lastDayOfMonth, dataTable);
            CalculationHistoryGridView.DataSource = dataTable;
            CalculationHistoryGridView.DataBind();
            CalculationHistoryGridView.PageIndex = 1;
        }

        protected void CalculationHistoryGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if ((bool)ViewState["searchActive"])
            {
                CalculationHistoryGridView.PageIndex = e.NewPageIndex;
                DataTable searchDataTable = (DataTable)ViewState["searchDataTable"];
                CalculationHistoryGridView.DataSource = searchDataTable;
                CalculationHistoryGridView.DataBind();
            }
            else
            {
                CalculationHistoryGridView.PageIndex = e.NewPageIndex;
                CalculationHistoryGridView.DataSource = (DataTable)ViewState["fullDataTable"];
                CalculationHistoryGridView.DataBind();
            }
        }
    }
}