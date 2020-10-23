using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Diabetes.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult SignIn()
        {
            return View();
        }
        
        public ActionResult Login(string userName, string passWord)
        {
            using (SqlConnection conn = new SqlConnection("Server=LAPTOP-PRFN4MOU;Database=Diabetes;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand("spcLogin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@userName", SqlDbType.VarChar).Value = userName;
                    cmd.Parameters.Add("@passWord", SqlDbType.VarChar).Value = passWord;

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    int id = 0;
                    while (rdr.Read())
                    {
                        id = int.Parse(rdr["ID"].ToString());
                    }

                    conn.Close();

                    if (id == 0)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json("error", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { userId = id }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        public ActionResult CreateAccount()
        {
            return View();
        }

        public ActionResult CreateNewUser(string firstName, string lastName, string userName, string passWord)
        {
            using (SqlConnection conn = new SqlConnection("Server=LAPTOP-PRFN4MOU;Database=Diabetes;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand("spcAddNewUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = firstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = lastName;
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userName;
                    cmd.Parameters.Add("@PassWord", SqlDbType.VarChar).Value = passWord;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            //int id = 0;
            //if (id == 0)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return Json("error", JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
                return Json("success", JsonRequestBehavior.AllowGet);
            //}
        }
    }
}