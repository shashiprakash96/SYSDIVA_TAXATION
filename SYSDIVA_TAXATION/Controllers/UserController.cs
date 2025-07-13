using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SYSDIVA_TAXATION.Models;
using SYSDIVA_TAXATION.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetUsers(string searchTerm)
        {
            List<Users> users = _userRepository.GetUsers();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
           
            return Json(users);
        }
        public IActionResult ExportToExcel()
        {
            List<Users> user = _userRepository.GetUsers();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("user");
                var currentRow = 1;

                // Adding Headers
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "Age";
                worksheet.Cell(currentRow, 5).Value = "Salary";
                worksheet.Cell(currentRow, 6).Value = "Gender";
                worksheet.Cell(currentRow, 7).Value = "Created On";
                worksheet.Cell(currentRow, 8).Value = "Is Active";

                // Adding Data
                foreach (var emp in user)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Id;
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    worksheet.Cell(currentRow, 3).Value = emp.Email;
                    worksheet.Cell(currentRow, 4).Value = emp.Age;
                    worksheet.Cell(currentRow, 5).Value = emp.Salary;
                    worksheet.Cell(currentRow, 6).Value = emp.Gender;
                    worksheet.Cell(currentRow, 7).Value = emp.CreatedOn;
                    worksheet.Cell(currentRow, 8).Value = emp.IsActive ? "Yes" : "No";
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                }
            }
        }
        [HttpGet]
        public IActionResult Insert()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Insert([FromBody] Users user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return validation errors
            }

            bool result = _userRepository.InsertUser(user);

            if (result)
            {
                return Json(new { success = true, message = "User inserted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "User insertion failed" });
            }
        }

        [HttpPost]
        public IActionResult Update(Users users)
        {
            if (ModelState.IsValid)
            {
                _userRepository.UpdateUser(users);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
       
        public IActionResult UserDelete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid user ID" });
            }

            bool isDeleted = _userRepository.DeleteUser(id);

            if (isDeleted)
            {
                return Json(new { success = true, message = "User deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to delete user" });
            }
        }

        [HttpGet("GetUserById")]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }
        [HttpPost]
        public IActionResult DeleteSelected(List<int> selectedIds)
        {
            _userRepository.DeleteSelected(selectedIds); // Call your method

            return RedirectToAction("Index"); // Or whatever list view you return
        }




    }


}


