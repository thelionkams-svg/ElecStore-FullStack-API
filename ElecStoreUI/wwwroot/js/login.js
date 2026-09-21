// ===== منطق صفحة تسجيل الدخول =====

document.addEventListener("DOMContentLoaded", () => {
    // إذا كان المستخدم مسجل دخول بالفعل، نوجهه للصفحة الرئيسية
    if (localStorage.getItem("token")) {
        window.location.href = "index.html";
        return;
    }

    document.getElementById("loginForm").addEventListener("submit", handleLogin);
});

function handleLogin(event) {
    event.preventDefault();

    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const errorDiv = document.getElementById("errorMessage");

    // إخفاء رسالة الخطأ السابقة
    errorDiv.classList.add("d-none");

    fetch(`${API_BASE_URL}/Auth/Login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username: username, password: password })
    })
        .then(response => {
            if (!response.ok) {
                // إذا كان الرد 401 (Unauthorized) أو أي خطأ آخر
                throw new Error("اسم المستخدم أو كلمة المرور غير صحيحة");
            }
            return response.json();
        })
        .then(data => {
            // تخزين التوكن في LocalStorage
            localStorage.setItem("token", data.token);
            // التوجه للصفحة الرئيسية
            window.location.href = "index.html";
        })
        .catch(error => {
            console.error("خطأ في تسجيل الدخول:", error);
            errorDiv.innerText = error.message;
            errorDiv.classList.remove("d-none");
        });
}








