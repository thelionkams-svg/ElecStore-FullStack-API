// ===== منطق صفحة الفواتير (Sales) - Live Cart =====

let currentSaleID = null;
let productsMap = {}; // { productID: { name, price, quantityInStock } }

document.addEventListener("DOMContentLoaded", () => {
    loadNavbar("sales");

    loadProductsIntoSelect().then(() => startNewInvoice());

    document.getElementById("addItemForm").addEventListener("submit", addItemToCart);
    document.getElementById("btnNewInvoice").addEventListener("click", startNewInvoice);
});

// جلب المنتجات لملء القائمة المنسدلة، مع حفظ السعر والمخزون لكل منتج
function loadProductsIntoSelect() {
    // تم التعديل: استخدام fetchWithAuth بدلاً من fetch
    return fetchWithAuth(`${API_BASE_URL}/Product`)
        .then(response => response.json())
        .then(products => {
            const select = document.getElementById("saleProduct");
            select.innerHTML = "";

            products.forEach(product => {
                productsMap[product.productID] = {
                    name: product.productName,
                    price: product.price,
                    quantityInStock: product.quantityInStock
                };

                const option = document.createElement("option");
                option.value = product.productID;
                option.innerText = `${product.productName} (متوفر: ${product.quantityInStock}) - ${product.price} ج.م`;
                select.appendChild(option);
            });
        })
        .catch(error => console.error("خطأ في جلب المنتجات:", error));
}

// إنشاء فاتورة جديدة فاضية (Header) والبدء بسلة فاضية
function startNewInvoice() {
    // تم التعديل: استخدام fetchWithAuth بدلاً من fetch
    fetchWithAuth(`${API_BASE_URL}/Sales`, { method: "POST" })
        .then(response => {
            if (!response.ok) throw new Error("فشل إنشاء الفاتورة");
            return response.json();
        })
        .then(sale => {
            currentSaleID = sale.saleID;
            document.getElementById("currentSaleLabel").innerText = `فاتورة رقم #${currentSaleID}`;
            renderCart([], 0);
        })
        .catch(error => {
            console.error("خطأ في إنشاء فاتورة جديدة:", error);
            alert("حصل خطأ أثناء إنشاء فاتورة جديدة، تأكد إن الـ API شغال وإنك مسجل دخول.");
        });
}

// إضافة صنف للفاتورة الحالية
function addItemToCart(event) {
    event.preventDefault();

    if (!currentSaleID) {
        alert("لسه مفيش فاتورة مفتوحة، حاول تاني.");
        return;
    }

    const productID = parseInt(document.getElementById("saleProduct").value);
    const quantity = parseInt(document.getElementById("saleQuantity").value);
    const productInfo = productsMap[productID];

    if (!productInfo) {
        alert("اختر منتج صحيح.");
        return;
    }

    const payload = {
        saleID: currentSaleID,
        productID: productID,
        quantitySold: quantity,
        unitPrice: productInfo.price
    };

    // تم التعديل: استخدام fetchWithAuth بدلاً من fetch
    fetchWithAuth(`${API_BASE_URL}/SaleDetails`, {
        method: "POST",
        body: JSON.stringify(payload)
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text || "فشل إضافة الصنف"); });
            }
            return response.json();
        })
        .then(result => {
            renderCart(result.items, result.totalAmount);
            document.getElementById("saleQuantity").value = 1;
        })
        .catch(error => {
            console.error("خطأ في إضافة الصنف:", error);
            alert("تعذر إضافة الصنف: " + error.message + " (تأكد إن الكمية المطلوبة متوفرة في المخزون)");
        });
}

// حذف صنف من الفاتورة الحالية، وتحديث السلة بعد الحذف
function removeItem(salesDetailsID) {
    if (!confirm("متأكد إنك عايز تحذف الصنف ده من الفاتورة؟")) return;

    // تم التعديل: استخدام fetchWithAuth بدلاً من fetch
    fetchWithAuth(`${API_BASE_URL}/SaleDetails/${salesDetailsID}`, { method: "DELETE" })
        .then(response => {
            if (!response.ok) throw new Error("فشل الحذف");
            return fetchWithAuth(`${API_BASE_URL}/Sales/${currentSaleID}`);
        })
        .then(response => response.json())
        .then(sale => renderCart(sale.items, sale.totalAmount))
        .catch(error => {
            console.error("خطأ في حذف الصنف:", error);
            alert("حصل خطأ أثناء حذف الصنف.");
        });
}

// رسم جدول السلة الحالية + الإجمالي
function renderCart(items, totalAmount) {
    const tableBody = document.querySelector("#cartTable tbody");
    tableBody.innerHTML = "";

    items.forEach(item => {
        const productName = productsMap[item.productID]?.name ?? item.productID;

        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${productName}</td>
            <td>${item.quantitySold}</td>
            <td>${item.unitPrice}</td>
            <td>${item.totalPrice}</td>
            <td>
                <button class="btn btn-sm btn-outline-danger" onclick="removeItem(${item.salesDetailsID})">حذف</button>
            </td>
        `;
        tableBody.appendChild(row);
    });

    document.getElementById("cartTotal").innerText = totalAmount;
}













