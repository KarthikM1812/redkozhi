// ✅ Load cart or initialize empty
let cart = JSON.parse(localStorage.getItem('cart')) || [];

// ✅ Check login status from layout meta tag
const isLoggedIn = document.querySelector('meta[name="user-authenticated"]')?.content === "true";

// ✅ Show alert banner
function showBanner(message, type = "warning") {
    const banner = document.getElementById("alertBanner");
    if (!banner) return;
    banner.className = `alert alert-${type} text-center`;
    banner.textContent = message;
    banner.classList.remove("d-none");
    setTimeout(() => banner.classList.add("d-none"), 3000);
}

// ✅ Render cart on order page
function renderCart() {
    const cartContainer = document.getElementById('cartSummary');
    if (!cartContainer) return;

    if (cart.length === 0) {
        cartContainer.innerHTML = "<p>Your cart is empty.</p>";
        return;
    }

    let html = "<table class='table table-striped'><thead><tr><th>Item</th><th>Qty</th><th>Price</th><th>Total</th><th>Action</th></tr></thead><tbody>";
    let total = 0;

    cart.forEach(item => {
        if (!item.name || !item.price || !item.quantity) return;

        const subTotal = item.price * item.quantity;
        total += subTotal;

        html += `<tr>
            <td>${item.name}</td>
            <td>${item.quantity}</td>
            <td>₹${item.price}</td>
            <td>₹${subTotal}</td>
            <td><button class="btn btn-sm btn-danger remove-btn" data-name="${item.name}">Remove</button></td>
        </tr>`;
    });

    html += `<tr><td colspan="3"><strong>Total</strong></td><td><strong>₹${total}</strong></td><td></td></tr></tbody></table>`;
    cartContainer.innerHTML = html;
}

// ✅ Update UI cart quantities (for both grid + scroll)
function updateUI() {
    const cart = JSON.parse(localStorage.getItem('cart') || '[]');

    // Scroll section
    $('.scroll-section .item-card').each(function () {
        const name = $(this).data('name');
        const found = cart.find(item => item.name.toLowerCase() === name.toLowerCase());
        $(this).find('.cart-qty').text(found ? found.quantity : '');
    });

    // Menu grid
    $('.menu-grd .m-card').each(function () {
        const name = $(this).find('.m-title').text().trim();
        const found = cart.find(item => item.name.toLowerCase() === name.toLowerCase());
        $(this).find('.cart-qty').text(found ? found.quantity : '');
    });
}

// ✅ Add to cart from menu grid
$(document).on('click', '.m-btn', function () {
    if (!isLoggedIn) {
        showBanner("Please login to add items to cart.", "danger");
        return;
    }

    const card = $(this).closest('.m-card');
    const name = card.find('.m-title').text().trim();
    const price = parseFloat(card.find('.m-price').text().replace('₹', '').trim());

    if (!name || isNaN(price)) {
        showBanner("Invalid item data.", "danger");
        return;
    }

    const existing = cart.find(item => item.name.toLowerCase() === name.toLowerCase());
    if (existing) {
        existing.quantity++;
    } else {
        cart.push({ name, price, quantity: 1 });
    }

    localStorage.setItem('cart', JSON.stringify(cart));
    showBanner(`${name} added to cart!`, "success");
    renderCart();
    updateUI();
});

// ✅ Add to cart from scroll section
$(document).on('click', '.scroll-section .add-to-cart', function () {
    if (!isLoggedIn) {
        showBanner("Please login to add items to cart.", "danger");
        return;
    }

    const card = $(this).closest('.item-card');
    const name = card.data('name');
    const price = parseFloat(card.data('price'));

    if (!name || isNaN(price)) {
        showBanner("Invalid item data.", "danger");
        return;
    }

    const existing = cart.find(item => item.name.toLowerCase() === name.toLowerCase());
    if (existing) {
        existing.quantity++;
    } else {
        cart.push({ name, price, quantity: 1 });
    }

    localStorage.setItem('cart', JSON.stringify(cart));
    showBanner(`${name} added to cart!`, "success");
    renderCart();
    updateUI();
});

// ✅ Remove item from cart
$(document).on('click', '.remove-btn', function () {
    const name = $(this).data('name');
    cart = cart.filter(item => item.name && item.name.toLowerCase() !== name.toLowerCase());
    localStorage.setItem('cart', JSON.stringify(cart));
    renderCart();
    updateUI();
});

// ✅ Render cart count in header (if applicable)
function renderCartCount() {
    const countSpan = document.getElementById("cart-count");
    if (!countSpan) return;
    const totalCount = cart.reduce((sum, item) => sum + item.quantity, 0);
    countSpan.textContent = totalCount;
}

// ✅ Submit order (for /Order page)
$(document).ready(function () {
    renderCart();
    updateUI();
    renderCartCount();

    $('#orderForm').submit(function (e) {
        e.preventDefault();

        if (!isLoggedIn) {
            showBanner("Please login before placing order.", "danger");
            return;
        }

        if (cart.length === 0) {
            showBanner("Cart is empty.", "danger");
            return;
        }

        const orderData = {
            name: $('#name').val(),
            phone: $('#phone').val(),
            address: $('#address').val(),
            notes: $('#notes').val(),
            cart: cart
        };

        if (!orderData.name || !orderData.phone || !orderData.address) {
            showBanner("Please fill all required fields.", "warning");
            return;
        }

        $.ajax({
            url: '/Home/SubmitOrder',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(orderData),
            success: function (res) {
                localStorage.removeItem('cart');
                showBanner("Order placed successfully!", "success");
                window.location.href = '/Home/OrderSummary?orderId=' + res.orderId;
            },
            error: function () {
                showBanner("Failed to submit order. Try again.", "danger");
            }
        });
    });
});
