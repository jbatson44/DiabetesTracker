using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diabetes.Models
{
    public class User
    {
        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<InsulinEntry> insulinEntries { get; set; }
        public List<CarbEntry> carbEntries { get; set; }
        public List<BloodSugarEntry> bloodSugarEntries { get; set; }
        public double A1c { get; set; }
        public DateTime chosenDate { get; set; }
        public List<Entry> allEntries { get; set; }
    }
}