using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class Result
    {
        public bool Success { get; set; }

        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }

        public object? value { get; set; }
        public Result() { }

        public Result(string errorM, bool success)
        {
            Success = success;
            ErrorMessage = errorM;
        }
    }

}
