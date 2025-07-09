using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class TestController : Controller
    {
        private readonly string _connectionString;

        public TestController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult StartTest()
        {
            TempData["StartTime"] = DateTime.Now;
            var model = new TestViewModel
            {
                Questions = GetQuestionsFromDb()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitTest(TestViewModel model)
        {
            var startTime = TempData["StartTime"] as DateTime?;
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            decimal totalMarks = 0;
            foreach (var question in model.Questions)
            {
                if (!string.IsNullOrEmpty(question.SelectedOption))
                {
                    if (question.SelectedOption == question.CorrectOption)
                    {
                        totalMarks += 2; // Correct answer
                    }
                    else
                    {
                        totalMarks -= 0.5m; // Wrong answer
                    }
                }
                // Optional: You can decide what to do if the question is left unanswered
            }

            SaveResultToDb(model.UserName, totalMarks);
            ViewBag.Marks = totalMarks;
            return View("Result");
        }

        private List<Question> GetQuestionsFromDb()
        {
            List<Question> questions = new List<Question>();
            using SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Id, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption FROM Questions", con);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                questions.Add(new Question
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    QuestionText = reader["QuestionText"].ToString(),
                    OptionA = reader["OptionA"].ToString(),
                    OptionB = reader["OptionB"].ToString(),
                    OptionC = reader["OptionC"].ToString(),
                    OptionD = reader["OptionD"].ToString(),
                    CorrectOption = reader["CorrectOption"].ToString()
                });
            }

            return questions;
        }


        private void SaveResultToDb(string userName, decimal marks)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand ("INSERT INTO TestResults(UserName, TotalMarks) VALUES (@User, @Marks)", con);
            cmd.Parameters.AddWithValue("@User", userName);
            cmd.Parameters.AddWithValue("@Marks", marks);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
