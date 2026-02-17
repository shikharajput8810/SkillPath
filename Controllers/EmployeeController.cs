using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        SqlConnection con = new SqlConnection("Data Source=LAPTOP-CI7LLE4G\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;");
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Profile()
        {
            string email = "";
            if (Session["emp"] != null)
            {
                email = Session["emp"].ToString();
            }
            else
            {
                return Content("<script>alert('Login First');location.href='/home/emplogin'</script>");
            }
            string command = $"select * from tbl_application where emailid='{email}' and ishired=1";
            SqlDataAdapter adapter = new SqlDataAdapter(command,con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            ViewBag.user = dt;
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string opass,string npass, string cpass)
        {
            if (npass.Equals(cpass))
            {
                if (npass.Equals(opass))
                {
                    return Content("<script>alert('New Password and old password should be different');location.href='/employee/ChangePassword'</script>");
                }
                else
                {
                    //change the password
                    string email = Session["emp"].ToString();
                    string command = $"update tbl_emplogin set password='{npass}' where email='{email}' and password='{opass}'";
                    SqlCommand cmd = new SqlCommand(command,con);
                    con.Open();
                    int result=cmd.ExecuteNonQuery();
                    con.Close();
                    if (result > 0)
                    {
                        Session.RemoveAll();
                        return Content("<script>alert('Password Updated...');location.href='/home/emplogin'</script>");
                    }
                    else
                    {
                        return Content("<script>alert('Password could not changed.Old password is incorrect');location.href='/employee/changepassword'</script>");
                    }
                }
            }
            else
            {
                return Content("<script>alert('Password and Confirm password should match!');location.href='/employee/Changepassword'</script>");
            }
               
        }
        public ActionResult LeaveApplication()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LeaveApplication(string subject,string detail,DateTime fromdate,DateTime todate,HttpPostedFileBase file)
        {
            string filename = "";
            if (file != null)
            {
                filename = file.FileName;
                file.SaveAs(Server.MapPath("/Content/leavefile/" + file.FileName));
            }
            int totaldays = 0;
            TimeSpan time= todate - fromdate;
            totaldays = time.Days;
            string userid = Session["emp"].ToString();
            string command = $"insert into tbl_leave values('{userid}','{subject}','{detail}','{fromdate.ToString("yyyy-MM-dd")}','{todate.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("yyyy-MM-dd")}',{totaldays},0,'{filename}')";
            SqlCommand cmd = new SqlCommand(command,con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('Application sent successfully.Please wait for admin approval.');location.href='/employee/leaveapplicationstatus'</script>");
        }
        public ActionResult Attendance()
        {
            string email = "";
            if (Session["emp"] != null)
            {
                email = Session["emp"].ToString();
            }
            else
            {
                return Content("<script>alert('Login First');location.href='/home/emplogin'</script>");
            }
            string command = $"select * from tbl_attendance where empid='{email}' and adate='{DateTime.Now.ToString("yyyy-MM-dd")}'";
            SqlDataAdapter adapter = new SqlDataAdapter(command,con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            ViewBag.data = dt;
            return View();
        }
        [HttpPost]
        public ActionResult Attendance(int? id)
        {
            string command = $"insert into tbl_attendance values('{Session["emp"]}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToShortTimeString()}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}')";
            SqlCommand cmd = new SqlCommand(command,con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('Your Attendance is marked for today');location.href='/employee/Attendance'</script>");

           
        }
        public ActionResult LeaveApplicationStatus()
        {

            string email = "";
            if (Session["emp"] != null)
            {
                email = Session["emp"].ToString();
            }
            else
            {
                return Content("<script>alert('Login First');location.href='/home/emplogin'</script>");
            }
                string command = $"select * from tbl_leave where empid='{email}'order by id desc";
            SqlDataAdapter adapter = new SqlDataAdapter(command,con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.leave = data;
            return View();
        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Emplogin","home");
        }
    }
}