using EMSProject.DataAccess;
using EMSProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace EMSProject.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDAL _dal;
        private readonly IConfiguration _configuration;

        public EmployeeController(EmployeeDAL dal, IConfiguration configuration)
        {
            _dal = dal;
            _configuration = configuration;
        }

        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("IsLoggedIn") == "true";
        }

        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Index", "Login");

            List<Employee> employees = new List<Employee>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Get employee list
                SqlCommand cmd = new SqlCommand("SELECT * FROM Employees", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Department = reader["Department"].ToString(),
                        Salary = Convert.ToDecimal(reader["Salary"])
                    });
                }
                reader.Close();

                SqlCommand statsCmd = new SqlCommand(@"
                    SELECT 
                        COUNT(*) AS TotalEmployees,
                        COUNT(DISTINCT Department) AS TotalDepartments,
                        AVG(Salary) AS AvgSalary
                    FROM Employees", conn);

                SqlDataReader statsReader = statsCmd.ExecuteReader();
                if (statsReader.Read())
                {
                    ViewBag.TotalEmployees = statsReader["TotalEmployees"] != DBNull.Value ? Convert.ToInt32(statsReader["TotalEmployees"]) : 0;
                    ViewBag.TotalDepartments = statsReader["TotalDepartments"] != DBNull.Value ? Convert.ToInt32(statsReader["TotalDepartments"]) : 0;
                    ViewBag.AvgSalary = statsReader["AvgSalary"] != DBNull.Value ? Convert.ToDecimal(statsReader["AvgSalary"]) : 0;
                }
            }

            return View(employees);
        }

        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee emp)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _dal.AddEmployee(emp);
                return RedirectToAction("Index");
            }
            return View(emp);
        }

        public IActionResult Edit(int id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Index", "Login");

            var employee = _dal.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee emp)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _dal.UpdateEmployee(emp);
                return RedirectToAction("Index");
            }
            return View(emp);
        }

        public IActionResult Details(int id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Index", "Login");

            var emp = _dal.GetEmployeeById(id);
            if (emp == null)
            {
                return NotFound();
            }
            return View(emp);
        }

        public IActionResult Delete(int id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Index", "Login");

            var employee = _dal.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Index", "Login");

            _dal.DeleteEmployee(id);
            return RedirectToAction("Index");
        }
    }
}
