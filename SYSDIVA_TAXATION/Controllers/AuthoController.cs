using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SYSDIVA_TAXATION.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Controllers
{
    public class AuthoController : Controller
    {
        private readonly string _connectionString;
        private readonly IPasswordHasher<LoginUsers> _passwordHasher;

        public AuthoController(IConfiguration configuration, IPasswordHasher<LoginUsers> passwordHasher)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _passwordHasher = passwordHasher;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(LoginUsers user)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    // Check if email already exists
                    string checkEmailQuery = "SELECT COUNT(*) FROM LoginUsers WHERE Email = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkEmailQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", user.Email);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            ViewBag.Message = "Email already registered.";
                            return View(user);
                        }
                    }

                    // Hash password
                    var hashedPassword = _passwordHasher.HashPassword(user, user.Password);

                    // Insert user if email not found
                    string insertQuery = "INSERT INTO LoginUsers (Username, Email, Password) VALUES (@Username, @Email, @Password)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", user.Username);
                        insertCmd.Parameters.AddWithValue("@Email", user.Email);
                        insertCmd.Parameters.AddWithValue("@Password", hashedPassword);

                        insertCmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Login");
            }

            return View(user);
        }


        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginUsers user)
        {
            LoginUsers dbUser = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM LoginUsers WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    dbUser = new LoginUsers
                    {
                        Id = (int)reader["Id"],
                        Username = reader["Username"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString()
                    };
                }
            }

            if (dbUser != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(dbUser, dbUser.Password, user.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    // Session tracking
                    HttpContext.Session.SetString("UserEmail", dbUser.Email);

                    // Cookie authentication
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dbUser.Username),
                    new Claim(ClaimTypes.Email, dbUser.Email)
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Student");
                }
            }

            ViewBag.Message = "Invalid credentials!";
            return View();
        }

        //[Authorize]
        public IActionResult Welcome()
        {
            ViewBag.User = HttpContext.User.Identity.Name;
            ViewBag.Email = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
