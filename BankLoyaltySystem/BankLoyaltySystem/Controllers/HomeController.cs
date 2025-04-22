using BankLoyaltySystem.Models;
using BankLoyaltySystem.Models.ViewModels;
using BankLoyaltySystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankLoyaltySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<HomeController> _logger;
        private readonly BankLoyaltySystem1Context _context; // إضافة الـ DbContext

        // تعديل الكونستركتر ليشمل DbContext
        public HomeController(ITransactionService transactionService, ILogger<HomeController> logger, BankLoyaltySystem1Context context)
        {
            _transactionService = transactionService;
            _logger = logger;
            _context = context; // تخصيص الـ DbContext
        }

        [HttpGet]
        public IActionResult EnterCustomerId()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnterCustomerId(int customerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "رقم العميل غير موجود.";
                return RedirectToAction("EnterCustomerId");
            }

            return RedirectToAction("Index", new { customerId });
        }

        public IActionResult Index(int customerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "العميل غير موجود.";
                return RedirectToAction("EnterCustomerId");
            }

            return View("Index", customer);  // تأكد من إرسال كائن من نوع Customer
        }





        //// صفحة البداية
        //public IActionResult Index(int? customerId)
        //{
        //    try
        //    {
        //        // في حالة عدم إرسال رقم العميل، يتم استخدام الرقم الافتراضي (1)
        //        customerId ??= 1;

        //        // التحقق من وجود العميل في قاعدة البيانات
        //        var customer = _transactionService.GetCustomerWithDetails(customerId.Value);

        //        if (customer == null)
        //        {
        //            // العميل غير موجود، اعرض رسالة خطأ
        //            TempData["ErrorMessage"] = "العميل غير موجود في النظام";
        //            return RedirectToAction("Index");  // التوجيه إلى صفحة الخطأ
        //        }

        //        return View(customer);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error loading customer dashboard");
        //        return View("Index");
        //    }
        //}

        // إيداع
        [HttpPost]
        public IActionResult Deposit(decimal amount)
        {
            try
            {
                int customerId = GetCurrentCustomerId(); // أو أي طريقة تجيب بها رقم العميل

                // استدعاء الإجراء
                var customerIdParam = new SqlParameter("@customerID", customerId);
                var amountParam = new SqlParameter("@amount", amount);

                // استدعاء الإجراء المخزن باستخدام ExecuteSqlRaw
                _context.Database.ExecuteSqlRaw("EXEC DepositMoney @customerID, @amount", customerIdParam, amountParam);

                TempData["SuccessMessage"] = "تم الإيداع بنجاح!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تنفيذ الإيداع");
                TempData["ErrorMessage"] = "حدث خطأ أثناء الإيداع: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // سحب
        [HttpPost]
        //public IActionResult Withdraw(decimal amount)
        //{
        //    try
        //    {
        //        int customerId = GetCurrentCustomerId(); // أو أي طريقة تجيب بها رقم العميل

        //        // استدعاء الإجراء
        //        var customerIdParam = new SqlParameter("@customerID", customerId);
        //        var amountParam = new SqlParameter("@amount", amount);

        //        // استدعاء الإجراء المخزن باستخدام ExecuteSqlRaw
        //        _context.Database.ExecuteSqlRaw("EXEC WithdrawMoney @customerID, @amount", customerIdParam, amountParam);

        //        TempData["SuccessMessage"] = "تم السحب بنجاح!";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "خطأ أثناء تنفيذ السحب");
        //        TempData["ErrorMessage"] = "حدث خطأ أثناء السحب: " + ex.Message;
        //    }

        //    return RedirectToAction("Index");

        //}



        [HttpPost]
        public IActionResult Withdraw(decimal amount)
        {
            try
            {
                int customerId = GetCurrentCustomerId(); // أو أي طريقة تجيب بها رقم العميل

                // استدعاء الإجراء
                var customerIdParam = new SqlParameter("@customerID", customerId);
                var amountParam = new SqlParameter("@amount", amount);

                // استدعاء الإجراء المخزن باستخدام ExecuteSqlRaw
                var result = _context.Database.ExecuteSqlRaw("EXEC WithdrawMoney @customerID, @amount", customerIdParam, amountParam);

                // الحصول على الرسالة الناتجة من SQL
                var message = result.ToString();

                // إذا كان هناك خطأ في السحب
                if (message.Contains("خطأ"))
                {
                    TempData["ErrorMessage"] = message;
                }
                else
                {
                    TempData["SuccessMessage"] = message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تنفيذ السحب");
                TempData["ErrorMessage"] = "حدث خطأ أثناء السحب: " + ex.Message;
            }

            return RedirectToAction("Index");
        }




        // للحصول على رقم العميل (للإختبار)
        private int GetCurrentCustomerId()
        {
            // في هذا المثال، يتم استخدام العميل رقم 1. في التطبيق الحقيقي يمكنك استخدام بيانات المستخدم الموثقة
            return 1;
        }


        public IActionResult TransactionHistory()
        {
            int customerId = GetCurrentCustomerId();

            // استرجاع عمليات الإيداع
            var deposits = _context.Deposits
                .Where(d => d.CustomerId == customerId)
                .Select(d => new TransactionHistoryViewModel
                {
                    TransactionType = "إيداع",
                    Amount = d.Amount,
                    Date = d.DepositDate ?? DateTime.Now
                });

            // استرجاع عمليات السحب
            var withdrawals = _context.Withdrawals
                .Where(w => w.CustomerId == customerId)
                .Select(w => new TransactionHistoryViewModel
                {
                    TransactionType = "سحب",
                    Amount = w.Amount,
                    Date = w.WithdrawalDate ?? DateTime.Now
                });

            // دمج العمليات في قائمة واحدة
            var transactions = deposits
                .Union(withdrawals)
                .OrderByDescending(t => t.Date)
                .ToList();

            return View(transactions);
        }

    }


}
