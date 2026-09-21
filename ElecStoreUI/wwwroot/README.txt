خطوات التركيب في مشروع ElecStoreUI
====================================

1. احذف فولدر wwwroot القديم بالكامل من مشروع ElecStoreUI.
2. فك ضغط هذا الملف، وانسخ فولدر wwwroot الجديد (بكل محتوياته) مكانه.
3. احذف فولدرين Controllers و Views بالكامل من المشروع (مش محتاجينهم خالص دلوقتي).
4. افتح Program.cs بتاع ElecStoreUI، وامسح محتواه واستبدله بالكود ده بالظبط:

    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();

    app.UseDefaultFiles();   // يخلي index.html تتفتح تلقائياً عند فتح الموقع
    app.UseStaticFiles();    // يسمح بتقديم ملفات HTML/CSS/JS من wwwroot

    app.Run();

5. افتح js/config.js وتأكد إن رقم البورت (API_BASE_URL) مطابق للبورت الحالي بتاع ElecStoreAPI.
6. شغّل ElecStoreAPI و ElecStoreUI مع بعض (Multiple startup projects) وجرب.

ملاحظة: تأكد إن CORS مفعّل في ElecStoreAPI (Program.cs بتاعه) قبل التجربة.
