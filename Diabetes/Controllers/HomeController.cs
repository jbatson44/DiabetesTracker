using Diabetes.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Diabetes.Controllers
{
    public class HomeController : Controller
    {
        public static User user;
        public ActionResult Index(int? userId)
        {
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("SignIn", "Login");
            }
            if (user == null)
            {
                user = new User();
                LoadUser((int)userId);
            }
            return View(user);
        }

        public ActionResult SignOut()
        {
            user = null;
            return RedirectToAction("SignIn", "Login");
        }

        private void LoadUser(int userId)
        {
            using (SqlConnection conn = new SqlConnection("Server=LAPTOP-PRFN4MOU;Database=Diabetes;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand("spcGetUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userId;

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    user.userId = userId;
                    user.firstName = string.Empty;
                    user.lastName = string.Empty;
                    while (rdr.Read())
                    {
                        user.firstName = rdr["firstName"].ToString();
                        user.lastName = rdr["LastName"].ToString();
                    }

                    conn.Close();
                }
            }
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
            using (SqlConnection conn = new SqlConnection("Server=LAPTOP-PRFN4MOU;Database=Diabetes;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand("spcAddBloodSugarLevel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = user.userId;
                    cmd.Parameters.Add("@BSLevel", SqlDbType.Int).Value = BSLevel;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = dateTime;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCarbs(int carbs, DateTime dateTime)
        {
            using (SqlConnection conn = new SqlConnection("Server=LAPTOP-PRFN4MOU;Database=Diabetes;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand("spcAddCarbs", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = user.userId;
                    cmd.Parameters.Add("@carbs", SqlDbType.Int).Value = carbs;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = dateTime;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddInsulin(int units, DateTime dateTime, int insulinType)
        {
            using (SqlConnection conn = new SqlConnection("Server=LAPTOP-PRFN4MOU;Database=Diabetes;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand("spcAddInsulin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = user.userId;
                    cmd.Parameters.Add("@units", SqlDbType.Int).Value = units;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = dateTime;
                    cmd.Parameters.Add("@insulinType", SqlDbType.Int).Value = insulinType;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
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

        public ActionResult GetData(int timeFrame)
        {
            DateTime endTime = DateTime.Now;
            DateTime beginTime = endTime.AddDays(-timeFrame);
            using (SqlConnection conn = new SqlConnection("Server=LAPTOP-PRFN4MOU;Database=Diabetes;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand("spcGetUserBloodSugarByTimeFrame", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = user.userId;
                    cmd.Parameters.Add("@beginDate", SqlDbType.DateTime).Value = beginTime;
                    cmd.Parameters.Add("@endDate", SqlDbType.DateTime).Value = endTime;

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    user.bloodSugarEntries = new List<BloodSugarEntry>();
                    while (rdr.Read())
                    {
                        BloodSugarEntry bloodSugarEntry = new BloodSugarEntry
                        {
                            bloodSugar = int.Parse(rdr["BSLevel"].ToString()),
                            insertTime = (DateTime)rdr["creationDate"]
                        };

                        user.bloodSugarEntries.Add(bloodSugarEntry);
                    }

                    conn.Close();
                }
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}