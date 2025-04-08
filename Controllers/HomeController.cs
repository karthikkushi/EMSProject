using System.Diagnostics;
using EMSProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace EMSProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    COUNT(*) AS TotalEmployees,
                    COUNT(DISTINCT Department) AS TotalDepartments,
                    AVG(Salary) AS AvgSalary
                FROM Employees", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ViewBag.TotalEmployees = reader["TotalEmployees"] != DBNull.Value ? Convert.ToInt32(reader["TotalEmployees"]) : 0;
                    ViewBag.TotalDepartments = reader["TotalDepartments"] != DBNull.Value ? Convert.ToInt32(reader["TotalDepartments"]) : 0;
                    ViewBag.AvgSalary = reader["AvgSalary"] != DBNull.Value ? Convert.ToDecimal(reader["AvgSalary"]) : 0;
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
