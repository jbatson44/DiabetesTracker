using Diabetes.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Configuration;

namespace Diabetes.Controllers
{
    public class HomeController : Controller
    {
        public static User user;
        public static DBConnect database;

        public ActionResult Index(int? userId)
        {
            if ((userId == 0 || userId == null) && user == null)
            {
                return RedirectToAction("SignIn", "Login");
            }
            if (user == null)
            {
                database = new DBConnect();
                user = new User();
                database.LoadUser((int)userId, user);
            }
            return View(user);
        }

        public ActionResult SignOut()
        {
            user = null;
            return RedirectToAction("SignIn", "Login");
        }

        public ActionResult AddData()
        {
            if (user == null)
            {
                return RedirectToAction("SignIn", "Login");
            }

            return View();
        }

        public ActionResult AddBloodSugarLevel(int BSLevel, DateTime dateTime)
        {
            database.AddBloodSugarLevel(BSLevel, dateTime, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCarbs(int carbs, DateTime dateTime)
        {
            database.AddCarbs(carbs, dateTime, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddInsulin(int units, DateTime dateTime, int insulinType)
        {
            database.AddInsulin(units, dateTime, insulinType, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Tracker()
        {
            if (user == null)
            {
                return RedirectToAction("SignIn", "Login");
            }

            return View(user);
        }

        public ActionResult GetDataByTimeframe(int timeFrame, bool includeBlood, bool includeCarbs, bool includeInsulin)
        {
            DateTime endTime = DateTime.Now;
            DateTime beginTime = endTime.AddDays(-timeFrame);

            if (includeBlood)
            {
                database.GetBloodSugar(endTime, beginTime, user);
                CalculateA1c();
            }
            if (includeCarbs)
            {
                database.GetCarbs(endTime, beginTime, user);
            }
            if (includeInsulin)
            {
                database.GetInsulin(endTime, beginTime, user);
            }
            var json = JsonConvert.SerializeObject(user.bloodSugarEntries);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataByDates(DateTime endTime, DateTime beginTime, bool includeBlood, bool includeCarbs, bool includeInsulin)
        {
            if (includeBlood)
            {
                database.GetBloodSugar(endTime, beginTime, user);
                CalculateA1c();
            }
            if (includeCarbs)
            {
                database.GetCarbs(endTime, beginTime, user);
            }
            if (includeInsulin)
            {
                database.GetInsulin(endTime, beginTime, user);
            }
            var json = JsonConvert.SerializeObject(user.bloodSugarEntries);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public void CalculateA1c()
        {
            if (user.bloodSugarEntries != null && user.bloodSugarEntries.Count > 0)
            {
                int total = user.bloodSugarEntries.Sum(x => x.bloodSugar);
                double average = total / user.bloodSugarEntries.Count;
                user.A1c = (46.7 + average) / 28.7;
            }
        }
    }
}