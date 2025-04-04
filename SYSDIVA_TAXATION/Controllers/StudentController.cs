using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using SYSDIVA_TAXATION.Models;
using SYSDIVA_TAXATION.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Reflection.Metadata;

namespace SYSDIVA_TAXATION.Controllers
{
    public class StudentController : Controller
    {
        private readonly UserRepository _userRepository;

        public StudentController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            
            return View();
        } 
        public IActionResult GetStudentList()
        {
            var data = _userRepository.GetAllStudent();
            return Json(data);
        }
        public IActionResult AddNewStudent()
        {
            return  View();
        } 
        [HttpPost]
        public IActionResult AddNewStudent(Student student)
        {
            _userRepository.AddStudent(student);

            return Json(new
            {
                success = true,
                message = "Student added successfully.",
                redirectUrl = Url.Action("StudentList") // Replace 'StudentList' with your actual list action
            });
        }

    }
}
