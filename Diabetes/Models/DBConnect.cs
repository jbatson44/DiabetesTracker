﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Diabetes.Models
{
    public class DBConnect
    {
        public void LoadUser(int userId, User user)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DiabetesDatabase"].ConnectionString))
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
        public void GetBloodSugar(DateTime endTime, DateTime beginTime, User user)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DiabetesDatabase"].ConnectionString))
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
        }

        public void GetCarbs(DateTime endTime, DateTime beginTime, User user)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DiabetesDatabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spcGetUserCarbsByTimeFrame", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = user.userId;
                    cmd.Parameters.Add("@beginDate", SqlDbType.DateTime).Value = beginTime;
                    cmd.Parameters.Add("@endDate", SqlDbType.DateTime).Value = endTime;

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    user.carbEntries = new List<CarbEntry>();
                    while (rdr.Read())
                    {
                        CarbEntry carbEntry = new CarbEntry
                        {
                            carbs = int.Parse(rdr["carbs"].ToString()),
                            insertTime = (DateTime)rdr["creationDate"]
                        };

                        user.carbEntries.Add(carbEntry);
                    }

                    conn.Close();
                }
            }
        }

        public void GetInsulin(DateTime endTime, DateTime beginTime, User user)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DiabetesDatabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spcGetUserInsulinByTimeFrame", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = user.userId;
                    cmd.Parameters.Add("@beginDate", SqlDbType.DateTime).Value = beginTime;
                    cmd.Parameters.Add("@endDate", SqlDbType.DateTime).Value = endTime;

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    user.insulinEntries = new List<InsulinEntry>();
                    while (rdr.Read())
                    {
                        InsulinEntry insulinEntry = new InsulinEntry
                        {
                            units = int.Parse(rdr["units"].ToString()),
                            insertTime = (DateTime)rdr["creationDate"],
                            insulinType = int.Parse(rdr["insulinType"].ToString())
                        };

                        user.insulinEntries.Add(insulinEntry);
                    }

                    conn.Close();
                }
            }
        }
        
        public void AddBloodSugarLevel(int BSLevel, DateTime dateTime, int userId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DiabetesDatabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spcAddBloodSugarLevel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = userId;
                    cmd.Parameters.Add("@BSLevel", SqlDbType.Int).Value = BSLevel;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = dateTime;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddCarbs(int carbs, DateTime dateTime, int userId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DiabetesDatabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spcAddCarbs", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = userId;
                    cmd.Parameters.Add("@carbs", SqlDbType.Int).Value = carbs;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = dateTime;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddInsulin(int units, DateTime dateTime, int insulinType, int userId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DiabetesDatabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spcAddInsulin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = userId;
                    cmd.Parameters.Add("@units", SqlDbType.Int).Value = units;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = dateTime;
                    cmd.Parameters.Add("@insulinType", SqlDbType.Int).Value = insulinType;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}