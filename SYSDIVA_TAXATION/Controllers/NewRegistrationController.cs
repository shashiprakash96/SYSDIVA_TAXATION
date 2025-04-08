using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SYSDIVA_TAXATION.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Controllers
{
    public class NewRegistrationController : Controller
    {
        private readonly string _connectionString;

        public NewRegistrationController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult Index()
        {
            return View();
        }
        private long GenerateNewRegistrationId()
        {
            long newId = 100000000000; // default starting ID if none exists

            using SqlConnection con = new SqlConnection( _connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(RegistrationId), 100000000000) + 1 FROM RegisterNewUsers", con);
            newId = (long)cmd.ExecuteScalar();
            return newId;
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new UserViewModel
            {
                RegistrationId = GenerateNewRegistrationId()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO RegisterNewUsers (RegistrationId, Name, Email) VALUES (@RegId, @Name, @Email)", con);
                cmd.Parameters.AddWithValue("@RegId", model.RegistrationId);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.ExecuteNonQuery();
                return RedirectToAction("Index","Student");
            }
            return View(model);
        }
        public IActionResult List()
        {
            List<UserViewModel> users = new List<UserViewModel>();

            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT RegistrationId, Name, Email FROM RegisterNewUsers", con);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new UserViewModel
                {
                    RegistrationId = Convert.ToInt64(reader["RegistrationId"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString()
                });
            }

            return View(users);
        }


    }
}
