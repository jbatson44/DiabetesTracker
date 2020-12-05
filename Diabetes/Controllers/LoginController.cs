using Diabetes.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        public static DBConnect database;
        // GET: Login
        public ActionResult SignIn()
        {
            if (database == null)
            {
                database = new DBConnect();
            }
            return View();
        }
        
        public ActionResult Login(string userName, string passWord)
        {
            int id = database.Login(userName, passWord);
            
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

        public ActionResult CreateAccount()
        {
            if (database == null)
            {
                database = new DBConnect();
            }
            return View();
        }

        public ActionResult CreateNewUser(string firstName, string lastName, string userName, string passWord)
        {
            database.CreateUser(firstName, lastName, userName, passWord);
            
            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}