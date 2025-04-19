using BankLoyaltySystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankLoyaltySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ITransactionService transactionService, ILogger<HomeController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                // In a real app, get customerId from authenticated user
                int customerId = GetCurrentCustomerId();
                var customer = _transactionService.GetCustomerWithDetails(customerId);
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer dashboard");
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Deposit(decimal amount)
        {
            try
            {
                int customerId = GetCurrentCustomerId();
                _transactionService.Deposit(customerId, amount);
                TempData["SuccessMessage"] = "Deposit completed successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing deposit");
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Withdraw(decimal amount)
        {
            try
            {
                int customerId = GetCurrentCustomerId();
                _transactionService.Withdraw(customerId, amount);
                TempData["SuccessMessage"] = "Withdrawal completed successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal");
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        private int GetCurrentCustomerId()
        {
            // For demo purposes, using customer with ID 1
            // In a real app, get from authenticated user claims
            return 1;
        }
    }
}