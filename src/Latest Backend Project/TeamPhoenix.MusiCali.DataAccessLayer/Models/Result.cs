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

        public bool hasError{ get; set; }
        public string? errorMessage { get; set; }
        public string? statusCode { get; set; }
=======
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }

        public object? value { get; set; }
>>>>>>> 72f693b5df2daec32f621445a51313f102d5003c:src/Latest Backend Project/TeamPhoenix.MusiCali.DataAccessLayer/Models/Result.cs
        public Result() { }

        public Result(string errorM, bool success)
        {
            Success = success;
            errorMessage = errorM;
        }
    }

}
