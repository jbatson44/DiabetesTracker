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
        public static SignInModel signIn;
        // GET: Login
        public ActionResult SignIn(bool signInFail = false)
        {
            if (database == null)
            {
                database = new DBConnect();
            }
            if (signIn == null)
            {
                signIn = new SignInModel();
            }
            signIn.SignInFailed = signInFail;
            return View(signIn);
        }
        
        public ActionResult Login(string userName, string passWord)
        {
            int id = database.Login(userName, passWord);
            
            if (id == 0)
            {
                return RedirectToAction("SignIn", "Login", new { signInFail = true });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { userId = id});
            }  
        }

        public ActionResult CreateAccount()
        {
            if (database == null)
            {
                database = new DBConnect();
            }
            if (signIn == null)
            {
                signIn = new SignInModel()
                {
                    SignInFailed = false
                };
            }
            return View(signIn);
        }

        public ActionResult CreateNewUser(string firstName, string lastName, string userName, string passWord)
        {
            database.CreateUser(firstName, lastName, userName, passWord);

            return RedirectToAction("SignIn", "Login");
        }
    }
}