using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SYSDIVA_TAXATION.Models;
using SYSDIVA_TAXATION.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Controllers
{
    public class GSTController : Controller
    {
        private readonly UserRepository _userRepository;

        public GSTController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            var data = _userRepository.GetAll();
            return View(data);
        }

        [HttpPost]
        public IActionResult Create(Product model)
        {
            model.CGSTAmount = model.Price * (model.TaxRate / 2) / 100;
            model.SGSTAmount = model.Price * (model.TaxRate / 2) / 100;
            model.TotalAmount = model.Price + model.CGSTAmount + model.SGSTAmount;

            _userRepository.InsertProduct(model);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _userRepository.Delete(id);
            return RedirectToAction("Index");
        }
        public IActionResult ExportToExcel()
        {
            List<Product> user = _userRepository.GetAll();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("user");
                var currentRow = 1;

                // Adding Headers
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "ProductName";
                worksheet.Cell(currentRow, 3).Value = "Price";
                worksheet.Cell(currentRow, 4).Value = "TaxRate";
                worksheet.Cell(currentRow, 5).Value = "CGSTAmount";
                worksheet.Cell(currentRow, 6).Value = "SGSTAmountr";
                worksheet.Cell(currentRow, 7).Value = "TotalAmount";
               

                // Adding Data
                foreach (var emp in user)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Id;
                    worksheet.Cell(currentRow, 2).Value = emp.ProductName;
                    worksheet.Cell(currentRow, 3).Value = emp.ProductName;
                    worksheet.Cell(currentRow, 4).Value = emp.TaxRate;
                    worksheet.Cell(currentRow, 5).Value = emp.CGSTAmount;
                    worksheet.Cell(currentRow, 6).Value = emp.SGSTAmount;
                    worksheet.Cell(currentRow, 7).Value = emp.TotalAmount;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Product.xlsx");
                }
            }
        }

        public IActionResult ListOfWallet()
        {
            ViewBag.Balance = _userRepository.GetWalletBalance();
            var transactions = _userRepository.GetTransactions();
            return View(transactions);
        }

        [HttpPost]
        public IActionResult AddWithdraw(decimal amount, string type)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than 0.";
                return RedirectToAction("ListOfWallet");
            }

            decimal currentBalance = _userRepository.GetWalletBalance();
            if (type == "Withdraw" && amount > currentBalance)
            {
                TempData["Error"] = "Insufficient balance.";
                return RedirectToAction("ListOfWallet");
            }

            _userRepository.AddTransaction(amount, type);
            return RedirectToAction("ListOfWallet");
        }

        
       
    }
}

