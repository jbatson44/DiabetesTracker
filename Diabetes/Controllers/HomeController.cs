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
                GetDataByTimeframe(30, true, true, true);
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

        [HttpPost]
        public JsonResult NewChart()
        {
            DateTime earliest = DateTime.Now.AddDays(-30);
            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", System.Type.GetType("System.String"));
            dt.Columns.Add("Blood", System.Type.GetType("System.Int32"));

            foreach(BloodSugarEntry bse in user.bloodSugarEntries)
            {
                if (bse.insertTime > earliest && bse.insertTime < DateTime.Now)
                {
                    DataRow dr = dt.NewRow();
                    dr["Date"] = bse.insertTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    dr["Blood"] = bse.bloodSugar;
                    dt.Rows.Add(dr);
                }
            }

            DateTime current = earliest;
            List<object> labels = new List<object>();
            while (current < DateTime.Today)
            {
                labels.Add(current);
                current = current.AddDays(1);
            }
            iData.Add(labels);
            //Looping and extracting each DataColumn to List<Object>  
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                x.Reverse();
                iData.Add(x);
            }
            List<object> beginEnd = new List<object>();
            beginEnd.Add(earliest.ToString("yyyy-MM-ddTHH:mm:ss"));
            beginEnd.Add(DateTime.Today.ToString("yyyy-MM-ddTHH:mm:ss"));
            iData.Add(beginEnd);
            //Source data returned as JSON  
            return Json(iData, JsonRequestBehavior.AllowGet);
        }
    }
}