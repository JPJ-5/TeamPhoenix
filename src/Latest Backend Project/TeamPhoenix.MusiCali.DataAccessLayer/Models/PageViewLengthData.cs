namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class PageViewLengthData
    {
        public string pageViewName;
        public decimal length;
        public PageViewLengthData(string viewName, decimal lengthOfPageView)
        {
            pageViewName = viewName;
            length = lengthOfPageView;
        }
    }
}
