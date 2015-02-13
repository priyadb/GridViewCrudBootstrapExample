using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewCRUDBootstrapExample
{
    public partial class Default : System.Web.UI.Page
    {
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            BindGrid();                         
        }

        public void BindGrid()
        {
            try
            {
                //Fetch data from mysql database
                string connString = ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString;
                MySqlConnection conn = new MySqlConnection(connString);
                conn.Open();
                string cmd = "select * from tblCountry limit 10";
                MySqlDataAdapter dAdapter = new MySqlDataAdapter(cmd, conn);
                DataSet ds = new DataSet();
                dAdapter.Fill(ds);
                dt = ds.Tables[0];
                //Bind the fetched data to gridview
                GridView1.DataSource = dt;
                GridView1.DataBind();
                
            }
            catch (MySqlException ex)
            {
                System.Console.Error.Write(ex.Message);

            }  

        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName.Equals("detail"))
            {
                string code = GridView1.DataKeys[index].Value.ToString();
                IEnumerable<DataRow> query = from i in dt.AsEnumerable()
                                             where i.Field<String>("Code").Equals(code)
                                             select i;
                DataTable detailTable = query.CopyToDataTable<DataRow>();
                DetailsView1.DataSource = detailTable;
                DetailsView1.DataBind();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#detailModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DetailModalScript", sb.ToString(), false);
            }
            else if (e.CommandName.Equals("editRecord"))
            {                               
                GridViewRow gvrow = GridView1.Rows[index];                
                lblCountryCode.Text = HttpUtility.HtmlDecode(gvrow.Cells[3].Text).ToString();                
                txtPopulation.Text = HttpUtility.HtmlDecode(gvrow.Cells[7].Text);
                txtName.Text = HttpUtility.HtmlDecode(gvrow.Cells[4].Text);
                txtContinent1.Text = HttpUtility.HtmlDecode(gvrow.Cells[5].Text);
                lblResult.Visible = false;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#editModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditModalScript", sb.ToString(), false);
                
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                string code = GridView1.DataKeys[index].Value.ToString();
                hfCode.Value = code;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteModalScript", sb.ToString(), false);
            }

        }
     

        protected void btnSave_Click(object sender, EventArgs e)
        {
                string code=lblCountryCode.Text;
                int population=Convert.ToInt32(txtPopulation.Text);
                string countryname = txtName.Text;
                string continent=txtContinent1.Text;
                executeUpdate(code,population,countryname,continent);                  
                BindGrid();                
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("alert('Records Updated Successfully');");
                sb.Append("$('#editModal').modal('hide');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
                
        }

        private void executeUpdate(string code,int population,string countryname,string continent)
        {
            string connString = ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString;
            try
            {
                MySqlConnection conn = new MySqlConnection(connString);
                conn.Open();
                string updatecmd = "update tblCountry set Population=@population, Name=@countryname,Continent=@continent where Code=@code";
                MySqlCommand updateCmd = new MySqlCommand(updatecmd,conn);                
                updateCmd.Parameters.AddWithValue("@population", population);
                updateCmd.Parameters.AddWithValue("@countryname", countryname);
                updateCmd.Parameters.AddWithValue("@continent", continent);
                updateCmd.Parameters.AddWithValue("@code", code);
                updateCmd.ExecuteNonQuery();
                conn.Close();
                
            }
            catch (MySqlException me)
            {
                System.Console.Error.Write(me.InnerException.Data);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddShowModalScript", sb.ToString(), false);
            
        }

        protected void btnAddRecord_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text;
            string name = txtCountryName.Text;
            string region = txtRegion.Text;
            string continent = txtContinent.Text;
            int population = Convert.ToInt32(txtTotalPopulation.Text);
            int indyear = Convert.ToInt32(txtIndYear.Text);
            executeAdd(code, name, continent, region,population, indyear);
            BindGrid();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("alert('Record Added Successfully');");
            sb.Append("$('#addModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddHideModalScript", sb.ToString(), false);


        }

        private void executeAdd(string code, string name, string continent,string region, int population, int indyear)
        {
            string connString = ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString;
            try
            {
                MySqlConnection conn = new MySqlConnection(connString);
                conn.Open();
                string updatecmd = "insert into tblCountry (Code,Name,Continent,Region,Population,IndepYear) values (@code,@name,@continent,@region,@population,@indyear)";
                MySqlCommand addCmd = new MySqlCommand(updatecmd, conn);
                addCmd.Parameters.AddWithValue("@code", code);
                addCmd.Parameters.AddWithValue("@name", name);
                addCmd.Parameters.AddWithValue("@continent", continent);
                addCmd.Parameters.AddWithValue("@region", region);
                addCmd.Parameters.AddWithValue("@population", population);
                addCmd.Parameters.AddWithValue("@indyear", indyear);
                addCmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (MySqlException me)
            {                
                System.Console.Write(me.Message);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string code=hfCode.Value;
            executeDelete(code);
            BindGrid();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("alert('Record deleted Successfully');");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "delHideModalScript", sb.ToString(), false);


        }

        private void executeDelete(string code)
        {
            string connString = ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString;
            try
            {
                MySqlConnection conn = new MySqlConnection(connString);
                conn.Open();
                string updatecmd = "delete from tblCountry where Code=@code";
                MySqlCommand addCmd = new MySqlCommand(updatecmd, conn);
                addCmd.Parameters.AddWithValue("@code", code);               
                addCmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (MySqlException me)
            {
                System.Console.Write(me.Message);
            }

        }

    }
}