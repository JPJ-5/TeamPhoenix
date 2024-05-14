namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class MonthYearCountResult
    {
        public List<MonthYearCount>? Values { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public MonthYearCountResult(List<MonthYearCount>? CountPerMonthYear, String message, bool result)
        {
            Values = CountPerMonthYear;
            ErrorMessage = message;
            Success = result;
        }
    }
}
