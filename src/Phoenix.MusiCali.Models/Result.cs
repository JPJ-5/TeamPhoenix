﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.MusiCali.Models
{
    public class Result
    {
        public bool hasError { get; set; }
        public string? errorMessage { get; set; }
        public string? statusCode { get; set; }
    }
}
