using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.MusiCali.Models
{
    public class Result
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? statusCode { get; set; }
        public Result() { }

        public Result(string errorM, bool success)
        {
            Success = success;
            ErrorMessage = errorM;
        }
    }
}
