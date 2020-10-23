using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diabetes.Models
{
    public class InsulinEntry
    {
        public int units { get; set; }
        public int insulinType { get; set; }
        public DateTime insertTime { get; set; }
    }
}