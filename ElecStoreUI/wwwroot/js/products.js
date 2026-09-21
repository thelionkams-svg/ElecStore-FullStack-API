// ===== منطق صفحة المنتجات (Products) =====

let productModal;
let categoriesMap = {}; // { categoryID: categoryName } عشان نعرض اسم الفئة في الجدول

document.addEventListener("DOMContentLoaded", () => {
    loadNavbar("products");
    productModal = new bootstrap.Modal(document.getElementById("productModal"));

    loadCategoriesIntoSelect().then(() => loadProducts());

    document.getElementById("btnAddProduct").addEventListener("click", openAddModal);
    document.getElementById("productForm").addEventListener("submit", saveProduct);
});

// جلب الفئات مرة واحدة لملء القائمة المنسدلة وبناء خريطة (ID -> اسم) للعرض
function loadCategoriesIntoSelect() {
    return fetchWithAuth(`${API_BASE_URL}/Category`)
        .then(response => response.json())
        .then(categories => {
            const select = document.getElementById("productCategory");
            select.innerHTML = "";

            categories.forEach(category => {
                categoriesMap[category.categoryID] = category.categoryName;

                const option = document.createElement("option");
                option.value = category.categoryID;
                option.innerText = category.categoryName;
                select.appendChild(option);
            });
        })
        .catch(error => {
            if (error.message === "UNAUTHORIZED") return;
            console.error("خطأ في جلب الفئات:", error);
        });
}

function loadProducts() {
    fetchWithAuth(`${API_BASE_URL}/Product`)
        .then(response => response.json())
        .then(data => renderProductsTable(data))
        .catch(error => {
            if (error.message === "UNAUTHORIZED") return;
            console.error("خطأ في جلب المنتجات:", error);
        });
}

function renderProductsTable(products) {
    const tableBody = document.querySelector("#productsTable tbody");
    tableBody.innerHTML = "";

    products.forEach(product => {
        const categoryName = categoriesMap[product.categoryID] ?? product.categoryID;

        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${product.productID}</td>
            <td>${product.productName}</td>
            <td>${categoryName}</td>
            <td>${product.price}</td>
            <td>${product.quantityInStock}</td>
            <td>
                <button class="btn btn-sm btn-outline-primary"
                    onclick='openEditModal(${JSON.stringify(product)})'>تعديل</button>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteProduct(${product.productID})">حذف</button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}

function openAddModal() {
    if (!requireLogin()) return; // التحقق من تسجيل الدخول
    document.getElementById("productModalTitle").innerText = "إضافة منتج جديد";
    document.getElementById("productId").value = "";
    document.getElementById("productName").value = "";
    document.getElementById("productCategory").value = "";
    document.getElementById("productPrice").value = "";
    document.getElementById("productQuantity").value = "";
    productModal.show();
}

function openEditModal(product) {
    if (!requireLogin()) return; // التحقق من تسجيل الدخول
    document.getElementById("productModalTitle").innerText = "تعديل المنتج";
    document.getElementById("productId").value = product.productID;
    document.getElementById("productName").value = product.productName;
    document.getElementById("productCategory").value = product.categoryID;
    document.getElementById("productPrice").value = product.price;
    document.getElementById("productQuantity").value = product.quantityInStock;
    productModal.show();
}

function saveProduct(event) {
    event.preventDefault();
    if (!requireLogin()) return; // التحقق من تسجيل الدخول

    const id = document.getElementById("productId").value;
    const payload = {
        productName: document.getElementById("productName").value,
        categoryID: parseInt(document.getElementById("productCategory").value),
        price: parseFloat(document.getElementById("productPrice").value),
        quantityInStock: parseInt(document.getElementById("productQuantity").value)
    };

    const isEdit = id !== "";
    const url = isEdit ? `${API_BASE_URL}/Product/${id}` : `${API_BASE_URL}/Product`;
    const method = isEdit ? "PUT" : "POST";

    if (isEdit) payload.productID = parseInt(id);

    fetchWithAuth(url, {
        method: method,
        body: JSON.stringify(payload)
    })
        .then(response => {
            if (!response.ok) throw new Error("فشل الحفظ");
            return response.json();
        })
        .then(() => {
            productModal.hide();
            loadProducts();
        })
        .catch(error => {
            if (error.message === "UNAUTHORIZED") return;
            console.error("خطأ في حفظ المنتج:", error);
            alert("حصل خطأ أثناء حفظ المنتج.");
        });
}

function deleteProduct(id) {
    if (!requireLogin()) return; // التحقق من تسجيل الدخول
    if (!confirm("متأكد إنك عايز تحذف المنتج ده؟")) return;

    fetchWithAuth(`${API_BASE_URL}/Product/${id}`, { method: "DELETE" })
        .then(response => {
            if (!response.ok) throw new Error("فشل الحذف");
            loadProducts();
        })
        .catch(error => {
            if (error.message === "UNAUTHORIZED") return;
            console.error("خطأ في حذف المنتج:", error);
            alert("حصل خطأ أثناء حذف المنتج.");
        });
}











