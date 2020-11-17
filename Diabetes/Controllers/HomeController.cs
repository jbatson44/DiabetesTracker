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
                user.chosenDate = DateTime.Today;
            }
            return View(user);
        }

        public ActionResult Calendar(int? userId)
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
                user.allEntries = new List<Entry>();
                GetDataByTimeframe(30, true, true, true);
                user.chosenDate = DateTime.Today;
            }
            GetDataByTimeframe(5, true, true, true);
            return View(user);
        }

        public ActionResult IncrementDecrementDate(int move)
        {
            user.chosenDate = user.chosenDate.AddDays(move);
            
            GetDataByDates(DateTime.Now, user.chosenDate, true, true, true);
            
            return Json("success", JsonRequestBehavior.AllowGet);
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

        public ActionResult AddBloodSugarLevelTwo(int BSLevel, DateTime dateTime, int hours, int minutes)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, 0);
            database.AddBloodSugarLevel(BSLevel, date, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBloodSugar(int entryId)
        {
            database.DeleteBloodSugarLevel(entryId, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCarbs(int carbs, DateTime dateTime)
        {
            database.AddCarbs(carbs, dateTime, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCarbsTwo(int carbs, DateTime dateTime, int hours, int minutes)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, 0);
            database.AddCarbs(carbs, date, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCarbs(int entryId)
        {
            database.DeleteCarbs(entryId, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddInsulin(int units, DateTime dateTime, int insulinType)
        {
            database.AddInsulin(units, dateTime, insulinType, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddInsulinTwo(int units, DateTime dateTime, int insulinType, int hours, int minutes)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, 0);
            database.AddInsulin(units, date, insulinType, user.userId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteInsulin(int entryId)
        {
            database.DeleteInsulin(entryId, user.userId);
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

            user.allEntries.Clear();
            if (includeBlood)
            {
                database.GetBloodSugar(endTime, beginTime, user);
                CalculateA1c();
                user.allEntries.AddRange(user.bloodSugarEntries);
            }
            if (includeCarbs)
            {
                database.GetCarbs(endTime, beginTime, user);
                user.allEntries.AddRange(user.carbEntries);
            }
            if (includeInsulin)
            {
                database.GetInsulin(endTime, beginTime, user);
                user.allEntries.AddRange(user.insulinEntries);
            }
            user.allEntries = user.allEntries.OrderBy(entry => entry.insertTime).ToList();
            var json = JsonConvert.SerializeObject(user.bloodSugarEntries);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataByDates(DateTime endTime, DateTime beginTime, bool includeBlood, bool includeCarbs, bool includeInsulin)
        {
            user.allEntries.Clear();

            if (includeBlood)
            {
                database.GetBloodSugar(endTime, beginTime, user);
                CalculateA1c();
                user.allEntries.AddRange(user.bloodSugarEntries);
            }
            if (includeCarbs)
            {
                database.GetCarbs(endTime, beginTime, user);
                user.allEntries.AddRange(user.carbEntries);
            }
            if (includeInsulin)
            {
                database.GetInsulin(endTime, beginTime, user);
                user.allEntries.AddRange(user.insulinEntries);
            }
            user.allEntries = user.allEntries.OrderBy(entry => entry.insertTime).ToList();
            var json = JsonConvert.SerializeObject(user.bloodSugarEntries);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public void CalculateA1c()
        {
            if (user.bloodSugarEntries != null && user.bloodSugarEntries.Count > 0)
            {
                int total = (int)user.bloodSugarEntries.Sum(x => x.bloodSugar);
                double average = total / user.bloodSugarEntries.Count;
                user.A1c = (46.7 + average) / 28.7;
            }
        }

        [HttpPost]
        public JsonResult BSChart()
        {
            DateTime furthestBack = user.bloodSugarEntries.Last().insertTime;
            DateTime earliest = new DateTime(furthestBack.Year, furthestBack.Month, furthestBack.Day);//DateTime.Now.AddDays(-30);
           
            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", System.Type.GetType("System.String"));
            dt.Columns.Add("Blood", System.Type.GetType("System.Int32"));

            foreach(BloodSugarEntry bse in user.bloodSugarEntries)
            {
                if (bse.insertTime >= earliest && bse.insertTime < DateTime.Now)
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
        [HttpPost]
        public JsonResult InsulinChart()
        {
            DateTime furthestBack = user.insulinEntries.Last().insertTime;
            DateTime earliest = new DateTime(furthestBack.Year, furthestBack.Month, furthestBack.Day);//DateTime.Now.AddDays(-30);

            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", System.Type.GetType("System.String"));
            dt.Columns.Add("Insulin", System.Type.GetType("System.Int32"));

            foreach (InsulinEntry bse in user.insulinEntries)
            {
                if (bse.insertTime >= earliest && bse.insertTime < DateTime.Now)
                {
                    DataRow dr = dt.NewRow();
                    dr["Date"] = bse.insertTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    dr["Insulin"] = bse.units;
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
        [HttpPost]
        public JsonResult CarbChart()
        {
            DateTime furthestBack = user.carbEntries.Last().insertTime;
            DateTime earliest = new DateTime(furthestBack.Year, furthestBack.Month, furthestBack.Day);//DateTime.Now.AddDays(-30);

            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", System.Type.GetType("System.String"));
            dt.Columns.Add("Carbs", System.Type.GetType("System.Int32"));

            foreach (CarbEntry bse in user.carbEntries)
            {
                if (bse.insertTime >= earliest && bse.insertTime < DateTime.Now)
                {
                    DataRow dr = dt.NewRow();
                    dr["Date"] = bse.insertTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    dr["Carbs"] = bse.carbs;
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