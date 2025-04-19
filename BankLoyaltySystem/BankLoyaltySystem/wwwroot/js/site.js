// يمكنك إضافة أي دوال جافاسكريبت لاحقاً
$(document).ready(function () {
    // تأكيد العمليات قبل التنفيذ
    $('.transaction-form').on('submit', function (e) {
        const amount = parseFloat($(this).find('input[name="amount"]').val());
        if (isNaN(amount) {
            alert('الرجاء إدخال مبلغ صحيح');
            return false;
        }

        const action = $(this).attr('action').includes('Deposit') ? 'إيداع' : 'سحب';
        return confirm(`هل أنت متأكد من أنك تريد ${action} مبلغ ${amount.toFixed(2)}؟`);
    });
});