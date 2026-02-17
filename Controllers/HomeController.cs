using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=LAPTOP-CI7LLE4G\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;");
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string name, long? number, string email,string message)
        {
            string command = $"insert into tbl_enquiry values('{name}',{number},'{email}','{message}','{DateTime.Now.ToString("yyyy-MM-dd")}')";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('adding application');location.href='/home/contact'</script>");
        }
        public ActionResult Team()
        {
            return View();
        }
        public ActionResult Opening()
        {
            SqlDataAdapter sda=new SqlDataAdapter("select * from tbl_opening order by id desc",con);
            DataTable data = new DataTable();
            sda.Fill(data);
            ViewBag.opening = data;
            return View();
        }
        public ActionResult Apply(int? jobid)
        {
            if (jobid.HasValue)
            {
                return View();
            }
            else
            {
                return Content("<script>alert('please select a job to apply');location.href='/home/opening'</script>");
            }
            
        }
        [HttpPost]
        public ActionResult Apply(int? jobid,string name,string email,long? mobno, string address,string exp, int? salary,string quali, string gender, HttpPostedFileBase resume,HttpPostedFileBase profile)
        {
            string command = $"insert into tbl_application values({jobid},'{name}',{mobno},'{email}','{address}','{quali}','{exp}',{salary},'{gender}','{resume.FileName}','{profile.FileName}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}',1,0,0)";
            SqlCommand cmd=new SqlCommand(command,con);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            //move the uploaded file in the server folder
            resume.SaveAs(Server.MapPath("/Content/resume/")+resume.FileName);
            profile.SaveAs(Server.MapPath("/Content/profile/")+profile.FileName);
            return Content("<script>alert('Successfully Applied. Please wait for admin response.');location.href='/home/opening'</script>");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string userid, string password)
        {

            if (userid.Equals("techpile") && password.Equals("123"))
            {
                Session["admin"] = userid;
                return Content("<script>alert('Welcome Admin');location.href='/admin/dashedboard'</script>", "text/html");
            }
            else
            {
                return Content("<script>alert('Enter valid Userid and Password');location.href='/admin/dashedboard'</script>", "text/html");
            }
        }
        public ActionResult Emplogin()
        { 
            return View();
        }
        [HttpPost]
        public ActionResult Emplogin(string userid,string password)
        {
            SqlDataAdapter adapter = new SqlDataAdapter($"select * from tbl_emplogin where email='{userid}' and password='{password}'",con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            if (data.Rows.Count>0)
            {
                Session["emp"] = userid;
                return RedirectToAction("Dashboard","Employee");
            }
            else
            {
                return Content("<script>alert('Invalid Id or Password');location.href='/home/emplogin'</script>");
            }
                
        }
        public ActionResult Application(string email)
        {
            if (email != null)
            {
                string command = $"select * from tbl_application where emailid='{email}'";
                SqlDataAdapter sda=new SqlDataAdapter(command,con);
                DataTable dt = new DataTable(); 
                sda.Fill(dt);
                ViewBag.app = dt;
            }
            return View();
        }
        public ActionResult Services()
        {
            return View();
        }
        
        //[HttpPost]
        //public ActionResult Contact(string name,string email,long? number,string message)
        //{
        //    string command = $"insert into tbl_enquiry values('{name}','{email}','{number}','{message}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}')";
        //    SqlCommand cmd = new SqlCommand(command,con);
        //    con.Open();
        //    int result= cmd.ExecuteNonQuery();
        //    con.Close();
        //    return Content("<script>alert('thank you , we will contanct you soon');location.href='/home/contact'</script>","text/html");
        //}
    }
}