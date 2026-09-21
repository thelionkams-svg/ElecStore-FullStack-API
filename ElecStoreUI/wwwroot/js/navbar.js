// ===== شريط التنقل العلوي المشترك بين كل الصفحات =====

function loadNavbar(activePage) {
    const isLoggedIn = localStorage.getItem("token") !== null;

    const navHtml = `
        <nav class="navbar navbar-expand-lg navbar-elecstore">
            <div class="container">
                <a class="navbar-brand fw-bold" href="index.html">🏪 ElecStore</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navMenu">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navMenu">
                    <ul class="navbar-nav ms-auto">
                        <li class="nav-item">
                            <a class="nav-link ${activePage === 'categories' ? 'fw-bold' : ''}" href="categories.html">الفئات</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link ${activePage === 'products' ? 'fw-bold' : ''}" href="products.html">المنتجات</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link ${activePage === 'sales' ? 'fw-bold' : ''}" href="sales.html">الفواتير</a>
                        </li>
                    </ul>
                    <ul class="navbar-nav">
                        ${isLoggedIn
            ? `<li class="nav-item"><a class="nav-link text-danger" href="#" onclick="logout()">تسجيل الخروج</a></li>`
            : `<li class="nav-item"><a class="nav-link text-primary" href="login.html">تسجيل الدخول</a></li>`
        }
                    </ul>
                </div>
            </div>
        </nav>
    `;

    document.getElementById("navbar-placeholder").innerHTML = navHtml;

}


// دالة تسجيل الخروج
function logout() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}










