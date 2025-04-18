﻿using BankLoyaltySystem.Models;
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

        // صفحة البداية
        public IActionResult Index(int? customerId)
        {
            try
            {
                // في حالة عدم إرسال رقم العميل، يتم استخدام الرقم الافتراضي (1)
                customerId ??= 1;

                // التحقق من وجود العميل في قاعدة البيانات
                var customer = _transactionService.GetCustomerWithDetails(customerId.Value);

                if (customer == null)
                {
                    // العميل غير موجود، اعرض رسالة خطأ
                    TempData["ErrorMessage"] = "العميل غير موجود في النظام";
                    return RedirectToAction("Index");  // التوجيه إلى صفحة الخطأ
                }

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer dashboard");
                return View("Index");
            }
        }

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
        public IActionResult Withdraw(decimal amount)
        {
            try
            {
                int customerId = GetCurrentCustomerId(); // أو أي طريقة تجيب بها رقم العميل

                // استدعاء الإجراء
                var customerIdParam = new SqlParameter("@customerID", customerId);
                var amountParam = new SqlParameter("@amount", amount);

                // استدعاء الإجراء المخزن باستخدام ExecuteSqlRaw
                _context.Database.ExecuteSqlRaw("EXEC WithdrawMoney @customerID, @amount", customerIdParam, amountParam);

                TempData["SuccessMessage"] = "تم السحب بنجاح!";
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
    }
}
