using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;


namespace FinalProject.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        SqlConnection con = new SqlConnection("Data Source=LAPTOP-CI7LLE4G\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;");
        public ActionResult DashedBoard()
        {
            return View();
        }
        public ActionResult addopening()
        {
            return View();
        }
        public ActionResult applicationdetails()
        {
            SqlDataAdapter sda=new SqlDataAdapter("select * from tbl_application order by id asc",con);
            DataTable data=new DataTable();
            sda.Fill(data);
            ViewBag.application = data;
            return View();
        }
        public ActionResult leaveapplication()
        {
            string command = $"select * from tbl_leave order by id desc";
            SqlDataAdapter adapter = new SqlDataAdapter(command, con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.leave = data;
            return View();
        }
        public ActionResult emplist()
        {
            SqlDataAdapter adapter = new SqlDataAdapter("select * from tbl_application where ishired=1",con);
            DataTable data=new DataTable();
            adapter.Fill(data);
            ViewBag.employee = data;
            return View();
        }
        public ActionResult salaryslip(string email,DateTime? fromdate,DateTime? todate)//HasValue only work with Nullable Values
        {
            //select all hired employee
            SqlDataAdapter adapter = new SqlDataAdapter("select * from tbl_application where ishired=1", con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.employee = data;
            if(email!=null && fromdate.HasValue && todate.HasValue)
            {
                string command = $"select * from tbl_attendance where empid='{email}' and adate between '{fromdate.Value.ToString("yyyy-MM-dd")}' and '{todate.Value.ToString("yyyy-MM-dd")}'";
                SqlDataAdapter sda = new SqlDataAdapter(command,con);
                DataTable attend = new DataTable();
                sda.Fill(attend);
                ViewBag.attend = attend;
            }
            return View();
        }
        [HttpPost]
        public ActionResult addopening(string title,string detail,string type, string city, int? minsalary,int? maxsalary, string gender, string shift , string exp, string education, int? vacancy, DateTime lastdate)
        {
            string command = $"insert into tbl_opening values('{title}','{detail}','{type}','{city}',{minsalary},{maxsalary},'{gender}','{shift}','{exp}','{education}',{vacancy},'{lastdate.ToString("yyy-MM-dd")}','{DateTime.Now.ToString("yyyy-MM-dd")}',1)";
            SqlCommand cmd = new SqlCommand(command,con);
            con.Open();
            int result=cmd.ExecuteNonQuery();
            con.Close();
            
            return Content("<script>alert('data insert successfully');location.href='/admin/addopening'</script>","text/html");
        }
        public ActionResult OpeningList()
        {
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_opening",con);
            DataTable data = new DataTable();
            sda.Fill(data);
            ViewBag.opening = data;
            return View();
        }
        public ActionResult enquirylist()
        {
            SqlDataAdapter sda= new SqlDataAdapter("select * from tbl_enquiry",con);
            DataTable data = new DataTable();
            sda.Fill(data);
            ViewBag.enquiry = data;
            return View();
        }
        public ActionResult hired(string email,int? appid)
        {
            if (email == null)
            {
                return Content("<script>alert('Please select a profile');location.href='/admin/applicationdetails'</script>", "text/html");
            }
            else
            {
                string command = $"update tbl_application set ishired=1 where id={appid}";
                SqlCommand cmd=new SqlCommand(command,con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                //send mail to the user to congratulate that you are hired!
                MailMessage mail = new MailMessage("shikharajput@gmail.com", email);
                mail.Subject = "Congratulations from Company-Hiring Message";
                mail.Body =$"Congratulations!!! You are hired in our company. Here is your Login Id and Password to access employee faculty.<br/><br/> Your Login Id is:{email}<br/>Your password is:Skillpath<br/><br/> Feel Free to Contact Us on:8745963217 any time if you have any query.";
                mail.IsBodyHtml=true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com",587);
                smtp.Credentials = new NetworkCredential("shikharajput881083@gmailcom", "csdx nenh lulu zkcw");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                //save the login id and password of employee into tbl_login

                string sqlcmd = $"insert into tbl_emplogin values('{email}','techpile',{appid})";
                SqlCommand cmd1= new SqlCommand(sqlcmd,con);
                con.Open();
                cmd1.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("applicationdetails");
            }
        }
        public ActionResult rejected(int? appid)
        {
            string command = $"update tbl_application set isrejected=1 where id={appid}";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("applicationdetails");
        }
        public ActionResult acceptleave(int? id)
        {
            string command = $"update tbl_leave set isaccepted=1 where id={id}";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("leaveapplication");
        }
        public ActionResult rejectleave(int? id)
        {
            string command = $"update tbl_leave set isaccepted=0 where id={id}";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("leaveapplication");
        }
        public ActionResult logout()
        {
            Session.RemoveAll();
            return RedirectToAction("login","home");
        }
    }
}