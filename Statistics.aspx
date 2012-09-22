<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="Statistics.aspx.cs" Inherits="Statistics" %>
<%@ MasterType VirtualPath="~/Master.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_Head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_Body" Runat="Server">
    <asp:SqlDataSource ID="sqlTeachers" runat="server" ConnectionString='<%$ connectionStrings:SEI_TMConnString %>'
                       SelectCommandType="StoredProcedure"
                       SelectCommand="tm_GetTeachers">
    </asp:SqlDataSource>
	
	<asp:SqlDataSource ID="sqlCourses" runat="server" ConnectionString='<%$ connectionStrings:SEI_TMConnString %>'
                       SelectCommandType="StoredProcedure"
                       SelectCommand="tm_GetTeachersCourses">
        <SelectParameters>
            <asp:ControlParameter Name="TeacherID" ControlID="ddlTeachers" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="sqlStats" runat="server" ConnectionString='<%$ connectionStrings:SEI_TMConnString %>'
                       SelectCommandType="StoredProcedure"
                       SelectCommand="tm_GetCourseWeeklyStats">
		<SelectParameters>
            <asp:ControlParameter Name="courseID" ControlID="ddlCourses" />
        </SelectParameters>
    </asp:SqlDataSource>

    
    <asp:DropDownList ID="ddlTeachers" DataSourceID="sqlTeachers" DataTextField="TeacherID" DataValueField="TeacherID" AutoPostBack="true" OnSelectedIndexChanged="ddlTeachers_OnChanged" runat="server" />&nbsp;&nbsp;
	<asp:DropDownList ID="ddlCourses" DataSourceID="sqlCourses" DataTextField="cName" DataValueField="ID" AutoPostBack="true" OnSelectedIndexChanged="ddlCourses_OnChanged" runat="server" />
    <br /><br />
    <asp:GridView ID="gdvStats" AutoGenerateColumns="false" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_Foot" Runat="Server">
</asp:Content>

