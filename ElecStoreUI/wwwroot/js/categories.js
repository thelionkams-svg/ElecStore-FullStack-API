// ===== منطق صفحة الفئات (Categories) =====

let categoryModal; // مرجع لعنصر الـ Modal بتاع Bootstrap

document.addEventListener("DOMContentLoaded", () => {
    loadNavbar("categories");
    categoryModal = new bootstrap.Modal(document.getElementById("categoryModal"));
    loadCategories();

    document.getElementById("btnAddCategory").addEventListener("click", openAddModal);
    document.getElementById("categoryForm").addEventListener("submit", saveCategory);
});

// جلب كل الفئات وعرضها في الجدول
function loadCategories() {
    fetchWithAuth(`${API_BASE_URL}/Category`)
        .then(response => response.json())
        .then(data => renderCategoriesTable(data))
        .catch(error => {
            if (error.message === "UNAUTHORIZED") return;
            console.error("خطأ في جلب الفئات:", error);
        });
}

function renderCategoriesTable(categories) {
    const tableBody = document.querySelector("#categoriesTable tbody");
    tableBody.innerHTML = "";

    categories.forEach(category => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${category.categoryID}</td>
            <td>${category.categoryName}</td>
            <td>${category.cDescription ?? ""}</td>
            <td>
                <button class="btn btn-sm btn-outline-primary" onclick="openEditModal(${category.categoryID}, '${escapeQuotes(category.categoryName)}', '${escapeQuotes(category.cDescription ?? "")}')">تعديل</button>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteCategory(${category.categoryID})">حذف</button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}

// فتح الـ Modal في وضع "إضافة"
function openAddModal() {
    if (!requireLogin()) return; // التحقق من تسجيل الدخول
    document.getElementById("categoryModalTitle").innerText = "إضافة فئة جديدة";
    document.getElementById("categoryId").value = "";
    document.getElementById("categoryName").value = "";
    document.getElementById("categoryDescription").value = "";
    categoryModal.show();
}

// فتح الـ Modal في وضع "تعديل" مع تعبئة البيانات الحالية
function openEditModal(id, name, description) {
    if (!requireLogin()) return; // التحقق من تسجيل الدخول
    document.getElementById("categoryModalTitle").innerText = "تعديل الفئة";
    document.getElementById("categoryId").value = id;
    document.getElementById("categoryName").value = name;
    document.getElementById("categoryDescription").value = description;
    categoryModal.show();
}

// حفظ الفئة (إضافة جديدة أو تعديل حسب وجود ID)
function saveCategory(event) {
    event.preventDefault();
    if (!requireLogin()) return; // التحقق من تسجيل الدخول

    const id = document.getElementById("categoryId").value;
    const payload = {
        categoryName: document.getElementById("categoryName").value,
        cDescription: document.getElementById("categoryDescription").value
    };

    const isEdit = id !== "";
    const url = isEdit ? `${API_BASE_URL}/Category/${id}` : `${API_BASE_URL}/Category`;
    const method = isEdit ? "PUT" : "POST";

    if (isEdit) payload.categoryID = parseInt(id);

    fetchWithAuth(url, {
        method: method,
        body: JSON.stringify(payload)
    })
        .then(response => {
            if (!response.ok) throw new Error("فشل الحفظ");
            return response.json();
        })
        .then(() => {
            categoryModal.hide();
            loadCategories();
        })
        .catch(error => {
            if (error.message === "UNAUTHORIZED") return;
            console.error("خطأ في حفظ الفئة:", error);
            alert("حصل خطأ أثناء حفظ الفئة.");
        });
}

// حذف فئة بعد تأكيد من المستخدم
function deleteCategory(id) {
    if (!requireLogin()) return; // التحقق من تسجيل الدخول
    if (!confirm("متأكد إنك عايز تحذف الفئة دي؟")) return;

    fetchWithAuth(`${API_BASE_URL}/Category/${id}`, { method: "DELETE" })
        .then(response => {
            if (!response.ok) throw new Error("فشل الحذف");
            loadCategories();
        })
        .catch(error => {
            if (error.message === "UNAUTHORIZED") return;
            console.error("خطأ في حذف الفئة:", error);
            alert("حصل خطأ أثناء حذف الفئة.");
        });
}

// دالة مساعدة بسيطة عشان تتجنب مشاكل النص لو فيه علامات تنصيص جوه الاسم
function escapeQuotes(text) {
    return String(text).replace(/'/g, "\\'");
}









