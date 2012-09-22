using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    string tm_EmptyString = "<br />";
    protected void Page_Load(object sender, EventArgs e)
    {
        lbl_error_message.Text = tm_EmptyString;
    }
    
    protected void LoginButtonClick(object sender, EventArgs e)
    {
        Int32 intUserType = 0;
        using (DirectoryEntry entry = new DirectoryEntry())
        {
            entry.Username = txt_username.Text;
            entry.Password = txt_password.Text;

            DirectorySearcher searcher = new DirectorySearcher(entry);

            searcher.Filter = "(objectclass=user)";

            try
            {
                searcher.FindOne();
            }
            catch (DirectoryServicesCOMException ex)
            {
                if (ex.ErrorCode == -2147023570) // Login or password is incorrect
                {
                    lbl_error_message.Text = "Username or password was invalid.<br />";
                }
                return;
            }
        }

        // Open DB connection
        SqlConnection TM_DB = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SEI_TMConnString"].ConnectionString);
        TM_DB.Open();

        SqlCommand command_GetUser = new SqlCommand("tm_GetUser", TM_DB);
        command_GetUser.CommandType = CommandType.StoredProcedure;
        command_GetUser.Parameters.AddWithValue("@UserID", txt_username.Text);

        SqlDataReader user_reader = command_GetUser.ExecuteReader();

        //Get the first user returned
        if (user_reader.HasRows)
        {
            while (user_reader.Read())
            {
                intUserType = (Int32)user_reader["TypeID"];
                break;
            }
        }
        else
        {
            lbl_error_message.Text = "You are not a current user.  Please contact the administrator of Time Machine.<br />";
        }
        user_reader.Close();
        user_reader.Dispose();
        command_GetUser.Dispose();

        // Close DB connection
        TM_DB.Close();
        TM_DB.Dispose();

        if (lbl_error_message.Text == tm_EmptyString)
        {
            Session["s_user"] = txt_username.Text;
            if (intUserType >= 1)
                Response.Redirect("Statistics.aspx");
            else
                Response.Redirect("Timelog.aspx");
        }
    }
}