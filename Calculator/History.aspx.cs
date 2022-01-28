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
        protected DataTable searchDataTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["searchActive"] = false;
                fullDataTable = new DataTable();
                BindOperationHistoryData(fullDataTable);
                ViewState["fullDataTable"] = fullDataTable;
                ViewState["searchActive"] = false;
                CalculationHistoryGridView.DataSource = fullDataTable;
                CalculationHistoryGridView.DataBind();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            ViewState["searchActive"] = true;
            fullDataTable = (DataTable)ViewState["fullDataTable"];
            searchDataTable = fullDataTable.Copy();

            if (FromDateTimePicker.Text != string.Empty && ToDateTimePicker.Text != string.Empty)
            {
                errorLabel.Text = string.Empty;

                DateTime fromDateTime = Convert.ToDateTime(FromDateTimePicker.Text);
                DateTime toDateTime = Convert.ToDateTime(ToDateTimePicker.Text);

                if(DateTime.Compare(fromDateTime, toDateTime) < 0)
                {
                    for (int i = searchDataTable.Rows.Count - 1; i >= 0; i--)
                    {
                        DateTime calculationRecordDateTime = Convert.ToDateTime(searchDataTable.Rows[i].ItemArray[1].ToString());
                        
                        if (DateTime.Compare(calculationRecordDateTime, fromDateTime) > 0 && DateTime.Compare(calculationRecordDateTime, toDateTime) < 0)
                        {
                            //Telpa i filtravimo rezius
                        }
                        else
                        {
                            searchDataTable.Rows.RemoveAt(i);
                        }
                    }
                    searchDataTable.AcceptChanges();
                    ViewState["searchDataTable"] = searchDataTable;
                    CalculationHistoryGridView.DataSource = (DataTable)ViewState["searchDataTable"];
                    CalculationHistoryGridView.DataBind();
                    errorLabel.Text = searchDataTable.Rows.Count.ToString();
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

            fullDataTable = new DataTable();
            BindOperationHistoryData(fullDataTable);
            CalculationHistoryGridView.DataSource = fullDataTable;
            CalculationHistoryGridView.DataBind();
        }

        int BindOperationHistoryData(DataTable dt)
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

                return 1; //Success
            }
            catch (Exception ex)
            {
                errorLabel.Text = "Couldn't connect to the database!";
                return 0; //Error accessing database
            }
        }

        protected void CalculationHistoryGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if ((bool)ViewState["searchActive"])
            {
                errorLabel.Text += "SEARCH";
                //SearchButton_Click(null, null);
                CalculationHistoryGridView.PageIndex = e.NewPageIndex;
                CalculationHistoryGridView.DataSource = (DataTable)ViewState["searchDataTable"];
                CalculationHistoryGridView.DataBind();
            }
            else
            {
                errorLabel.Text += "FULL";
                CalculationHistoryGridView.PageIndex = e.NewPageIndex;
                CalculationHistoryGridView.DataSource = (DataTable)ViewState["fullDataTable"];
                CalculationHistoryGridView.DataBind();
            }

            /*
            if (searchActive)
            {
                SearchButton_Click(null, null);
                CalculationHistoryGridView.DataSource = searchDataTable;
            }
            else
            {
                BindOperationHistoryData(fullDataTable);
                CalculationHistoryGridView.DataSource = fullDataTable;
            }
            CalculationHistoryGridView.PageIndex = e.NewPageIndex;
            CalculationHistoryGridView.DataBind();
            */

        }
    }
}