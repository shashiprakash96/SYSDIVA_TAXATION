using Microsoft.AspNetCore.Mvc;
using SYSDIVA_TAXATION.Models;
using SYSDIVA_TAXATION.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Controllers
{
    public class BillingController : Controller
    {
        private readonly UserRepository _userRepository;

        public BillingController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new BillViewModel
            {
                FoodItems = _userRepository.GetFoodItems()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(BillViewModel model, List<int> foodIds, List<int> quantities)
        {
            model.SelectedItems = new List<BillItem>();
            //model.SelectedItems = new();
            for (int i = 0; i < foodIds.Count; i++)
            {
                var food = _userRepository.GetFoodItems().First(f => f.Id == foodIds[i]);
                int qty = quantities[i];
                if (qty > 0)
                {
                    model.SelectedItems.Add(new BillItem
                    {
                        FoodItemId = food.Id,
                        FoodName = food.Name,
                        Quantity = qty,
                        SubTotal = food.Price * qty
                    });
                }
            }

            model.TotalAmount = model.SelectedItems.Sum(x => x.SubTotal);
            _userRepository.SaveBill(model.CustomerName, model.SelectedItems);

            return View("BillSummary", model);
        }
    }
}

