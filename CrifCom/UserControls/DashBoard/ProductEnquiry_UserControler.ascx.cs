using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.NodeFactory;
using Umbraco.Core;

namespace CrifCom.UserControls.DashBoard
{
    public partial class ProductEnquiry : System.Web.UI.UserControl
    {
        internal string connectionString = umbraco.GlobalSettings.DbDSN;
        protected void Page_Load(object sender, EventArgs e)
        {

            SqlDataSourceMain.ConnectionString = umbraco.GlobalSettings.DbDSN;
            SqlDataSourceDetails.ConnectionString = umbraco.GlobalSettings.DbDSN;

        }
        protected void DetailsView_DataBound(object sender, EventArgs e)
        {
            DetailsView dv = (DetailsView)sender;
        }
        protected void select_form_SelectedIndexChanged(object sender, EventArgs e)
        {
            // SqlDataSourceMain.SelectParameters[0].DefaultValue =select_form.SelectedValue.ToString();
            DetailsView dv = (DetailsView)sender;
            //GridView1.Columns[1].Visible = select_form.SelectedValue != "1";
        }


        protected void ibDownload_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Clear();
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            string sqlSelectQuery = "";
            sqlConnection.Open();
            sqlSelectQuery = String.Format("SELECT [Id],[FirstName],[LastName],[Company],[UserRole] AS Role,[Email],[Telephone],[contentUrl],[ClientIP],[Device],[Browser], " +
                           " [UserAgent],[DevicePlatform],[FormId] as [Page Url],[CreatedDate] FROM[dbo].[DownloadedContents] WHERE [IsProduct]=1 ORDER BY CreatedDate DESC", sqlConnection);
            SqlDataReader knowledgeFormReader = SqlHelper.ExecuteReader(connectionString, CommandType.Text, sqlSelectQuery);
            dt.Load(knowledgeFormReader);
            var dataRows = dt.Rows;
            string json = "";
            int columnCounter = 0;

            if (dt != null)
            {
                DataView dv = new DataView(dt);
                string[] columnhead = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    columnhead[i] = dt.Columns[i].ToString();
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", string.Format("{0}_{1}.csv", DateTime.Now.ToString("ddMMMyyyy"), "Contact Form")));
                HttpContext.Current.Response.ContentType = "application/CSV";
                HttpContext.Current.Response.Charset = "utf-8";
                HttpContext.Current.Response.OutputStream.Write(new byte[] { 0xef, 0xbb, 0xbf }, 0, 3);
                HttpContext.Current.Response.Write(DataViewToCSV(dv, columnhead));
                HttpContext.Current.Response.End();
            }

        }

        public static string DataViewToCSV(DataView table, string[] columns)
        {
            var result = new StringBuilder();
            for (int i = 0; i < columns.Length; i++)
            {
                result.Append("\"").Append((columns[i]).Replace("\"", "\"\"").Replace("\n", " | ")).Append("\"");
                result.Append(i == columns.Length - 1 ? "\n" : ",");
            }

            foreach (DataRowView row in table)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    result.Append("\"").Append(row[columns[i]].ToString().Replace("\"", "\"\"").Replace("\n", " | ")).Append("\"");

                    result.Append(i == columns.Length - 1 ? "\n" : ",");
                }
            }

            return result.ToString();
        }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView1.Rows.Count == 0)
            {
                ibDownload.Visible = false;
                Label1.Visible = false;
            }
            else
            {
                ibDownload.Visible = true;
                Label1.Visible = true;
            }
        }       
    }
}