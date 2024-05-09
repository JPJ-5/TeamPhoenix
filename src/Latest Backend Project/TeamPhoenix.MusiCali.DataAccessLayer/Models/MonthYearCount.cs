namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class MonthYearCount
    {
        public DateTime monthYear;
        public long count;
        public MonthYearCount(int monthOfCount, int yearOfCount, long countWithinMonthYear)
        {
            monthYear = new DateTime(yearOfCount, monthOfCount, 1);
            count = countWithinMonthYear;
        }
    }
}
