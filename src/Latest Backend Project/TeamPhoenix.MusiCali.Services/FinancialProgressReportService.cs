using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.IdentityModel.Tokens;

namespace TeamPhoenix.MusiCali.Services
{
    public class FinancialProgressReportService
    {
        private FinancialProgressReportDAO financialProgressReportDAO;
        private RecoverUserDAO recoverUserDAO;
        public FinancialProgressReportService(IConfiguration configuration)
        {
            financialProgressReportDAO = new FinancialProgressReportDAO(configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
        }


        public HashSet<FinancialInfoModel> GetReport(string username, string frequency)
        {
            try
            {
                var infos = new HashSet<FinancialInfoModel>();  // To store financial information
                var userHash = recoverUserDAO.GetUserHash(username);  // Retrieve user hash based on username

                Console.WriteLine(frequency);  // Debugging: Output the frequency

                // Throw an exception if frequency is null or empty
                if (string.IsNullOrEmpty(frequency))
                {
                    throw new Exception($"{username} provided an empty or null frequency");
                }

                // Switch on the frequency to fetch appropriate reports
                switch (frequency)
                {
                    case "Yearly":
                        infos = financialProgressReportDAO.FetchYearlyReport(userHash);  // Fetch and assign yearly report
                        break;
                    case "Quarterly":
                        infos = financialProgressReportDAO.FetchQuarterlyReport(userHash);  // Fetch and assign quarterly report
                        break;
                    case "Monthly":
                        infos = financialProgressReportDAO.FetchMonthlyReport(userHash);  // Fetch and assign monthly report
                        break;
                    default:
                        throw new Exception("Invalid Frequency");  // Handle unexpected frequency value
                }

                return infos;  // Return the collected financial information
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HashSet<FinancialInfoModel>();
            }
        }
    }
}
