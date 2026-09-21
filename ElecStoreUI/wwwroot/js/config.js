// ===== إعدادات مشتركة لكل صفحات الموقع =====
const API_BASE_URL = "https://localhost:7140/api";

// دالة للتحقق من تسجيل الدخول قبل أي عملية حساسة
function requireLogin() {
    if (!localStorage.getItem("token")) {
        window.location.href = "login.html";
        return false;
    }
    return true;
}

// دالة مساعدة لإرسال الطلبات مع التوكن
function fetchWithAuth(url, options = {}) {
    const token = localStorage.getItem("token");

    // إعداد الـ Headers الافتراضية
    const headers = {
        "Content-Type": "application/json",
        ...options.headers
    };

    // إذا كان هناك توكن، نضيفه
    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    // دمج الإعدادات وإرسال الطلب
    return fetch(url, {
        ...options,
        headers: headers
    }).then(response => {
        // إذا كان الرد 401 (غير مصرح)، نوجه المستخدم لتسجيل الدخول
        if (response.status === 401) {
            localStorage.removeItem("token");
            window.location.href = "login.html";
            throw new Error("UNAUTHORIZED");
        }
        return response;
    });
}







