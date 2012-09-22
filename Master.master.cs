using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Master : System.Web.UI.MasterPage
{
    #region Variables
        private String str_conn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SEI_TMConnString"].ConnectionString;
		
		private Boolean bool_teacher = false;
		private Boolean bool_admin = false;
	
		private Int32 intUserType;
    #endregion

    #region Properties
        public String Conn
        {
            get { return str_conn; }
        }
	
		public Boolean isTeacher
        {
            get { return bool_teacher; }
        }
	
		public Boolean isAdmin
        {
            get { return bool_admin; }
        }
	
		public Int32 UserType
        {
            get { return intUserType; }
        }

        public String UserID
        {
            get { return Session["s_user"] != null ? Session["s_user"].ToString ( ) : null; }
        }
    #endregion
	
	#region Protected Functions
	    protected void Page_Init(object sender, EventArgs e)
	    {
            // Redirect to login if there is no user
            if (Session["s_user"] == null)
                Response.Redirect("Default.aspx");

            // Open DB connection
	        SqlConnection TM_DB = new SqlConnection(Conn);
	        TM_DB.Open();

			// Initial load functions
            if (Session["s_user"] != null)
                Load_user(TM_DB);
		
			// Show proper quicklinks
            tcClassesProjects.Visible = isTeacher;
			tcAdmin.Visible = isAdmin;
		
			// Close DB connection
			TM_DB.Close();
			TM_DB.Dispose();
	    }

        protected void lnk_Logout_Click(object sender, EventArgs e)
        {
            Session.Remove("s_user");
            Response.Redirect("Default.aspx");
        }
	#endregion
	
	#region Private Functions
	    // Pulling the DB to load user info
	    private void Load_user(SqlConnection user_db)
	    {
			intUserType = 0;
			
	        //Set up the getuser procedure
	        SqlCommand command_GetUser = new SqlCommand("tm_GetUser", user_db);
	        command_GetUser.CommandType = CommandType.StoredProcedure;
	        command_GetUser.Parameters.AddWithValue("@UserID", Session["s_user"].ToString());

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
			user_reader.Close();
			user_reader.Dispose();
			command_GetUser.Dispose();
			
			bool_teacher = (intUserType >= 1);
			bool_admin = (intUserType >= 2);
            lbl_UserName.Text = Session["s_user"].ToString();
	    }
	#endregion
}
