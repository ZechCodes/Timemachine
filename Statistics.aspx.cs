using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Statistics : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack)
		{
			ddlTeachers.DataBind();
			ddlCourses.DataBind();
        	gdvStats.DataSource = getStats();
			gdvStats.DataBind();
		}
    }
	
	protected void ddlTeachers_OnChanged(Object sender, EventArgs e)
	{
        ddlCourses.DataBind();
		gdvStats.DataSource = getStats();
		gdvStats.DataBind();
	}
	
	protected void ddlCourses_OnChanged(Object sender, EventArgs e)
	{
        gdvStats.DataSource = getStats();
		gdvStats.DataBind();
 	}
	
	private DataTable getStats()
	{
        String strID = String.Empty;
        Int32 intWeeks = 0;
        Object objTemp;
        DataColumn dcTemp;
        DataRow drTemp = null;
        BoundField bfTemp;

        SqlConnection scstat = new SqlConnection(Master.Conn);
        scstat.Open();
        
        gdvStats.Columns.Clear();
        DataTable dtStats = new DataTable();
		dcTemp = new DataColumn("Name");
		dtStats.Columns.Add (dcTemp);
        bfTemp = new BoundField();
        bfTemp.DataField = dcTemp.ColumnName;
        bfTemp.HeaderText = dcTemp.ColumnName;
        gdvStats.Columns.Add(bfTemp);
        dcTemp = new DataColumn("Sem Total");
		dtStats.Columns.Add (dcTemp);
        bfTemp = new BoundField();
        bfTemp.DataField = dcTemp.ColumnName;
        bfTemp.HeaderText = dcTemp.ColumnName;
        gdvStats.Columns.Add(bfTemp);

        SqlCommand cmdGetWeeks = new SqlCommand("tm_CalculateCourseWeeks", scstat);
        cmdGetWeeks.CommandType = CommandType.StoredProcedure;
        cmdGetWeeks.Parameters.AddWithValue("@courseID", ddlCourses.SelectedValue);
        objTemp = cmdGetWeeks.ExecuteScalar();
        intWeeks = (objTemp == null ? 0 : (Int32)objTemp);
        cmdGetWeeks.Dispose();
        int i=1;
        for (i = 1; i <= intWeeks; i++)
        {
            dcTemp = new DataColumn(i.ToString());
            dtStats.Columns.Add(dcTemp);
            bfTemp = new BoundField();
            bfTemp.DataField = dcTemp.ColumnName;
            bfTemp.HeaderText = dcTemp.ColumnName;
            gdvStats.Columns.Add(bfTemp);
        }

        SqlCommand cmdGetStats = new SqlCommand("tm_GetCourseWeeklyStats", scstat);
        cmdGetStats.CommandType = CommandType.StoredProcedure;
        cmdGetStats.Parameters.AddWithValue("@courseID", ddlCourses.SelectedValue);
        SqlDataReader rdstats = cmdGetStats.ExecuteReader();

        if (rdstats.HasRows)
        {
            while (rdstats.Read())
            {
                if (strID != rdstats["UserID"].ToString())
                {
                    if (strID != String.Empty)
                        dtStats.Rows.Add(drTemp);

                    strID = rdstats["UserID"].ToString();
                    drTemp = dtStats.NewRow();
                    drTemp["Name"] = strID;
                    drTemp["Sem Total"] = rdstats["Time"].ToString();
                    continue;
                }

                drTemp[rdstats["Week"].ToString()] = rdstats["Time"].ToString();
                
			}
            dtStats.Rows.Add(drTemp);
		}
		
		rdstats.Close();
		rdstats.Dispose();
		cmdGetStats.Dispose();
		scstat.Close();
		scstat.Dispose();
		
		return dtStats;
	}
}