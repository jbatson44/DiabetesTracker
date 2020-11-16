﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diabetes.Models
{
    public class InsulinEntry : Entry
    {
        public int units { get; set; }
        public int insulinType { get; set; }
    }
}