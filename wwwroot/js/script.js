// Initialize cart
let cart = JSON.parse(sessionStorage.getItem('cart')) || [];

// Check if user is logged in via meta tag
const isLoggedIn = document.querySelector('meta[name="user-authenticated"]')?.content === "true";

// Show alert banner
function showBanner(message, type = "warning") {
    const banner = document.getElementById("alertBanner");
    if (!banner) return;
    banner.className = `alert alert-${type} text-center`;
    banner.textContent = message;
    banner.classList.remove("d-none");
    setTimeout(() => banner.classList.add("d-none"), 3000);
}

// Render cart summary table
function renderCart() {
    const cartContainer = document.getElementById('cartSummary');
    if (!cartContainer) return;

    if (cart.length === 0) {
        cartContainer.innerHTML = "<p>Your cart is empty.</p>";
        return;
    }

    let html = `<table class='table table-striped'>
<thead><tr><th>Item</th><th>Qty</th><th>Price</th><th>Total</th><th>Action</th></tr></thead><tbody>`;
    let total = 0;

    cart.forEach(item => {
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

// Update quantity badges in UI
function updateUI() {
    $('.menu-grd .m-card').each(function () {
        const name = $(this).data('name');
        const found = cart.find(item => item.name.toLowerCase() === name.toLowerCase());
        $(this).find('.cart-qty').text(found ? found.quantity : '');
    });

    $('.scroll-section .item-card').each(function () {
        const name = $(this).data('name');
        const found = cart.find(item => item.name.toLowerCase() === name.toLowerCase());
        $(this).find('.cart-qty').text(found ? found.quantity : '');
    });
}

// Update cart count in header
function renderCartCount() {
    const count = cart.reduce((sum, item) => sum + item.quantity, 0);
    const span = document.getElementById('cart-count');
    if (span) span.textContent = count;
}

// Add to cart from menu grid
$(document).on('click', '.m-btn', function () {
    if (!isLoggedIn) {
        showBanner("Please login to add items to cart", "danger");
        return;
    }

    const card = $(this).closest('.m-card');
    const name = card.data('name');
    const price = parseFloat(card.data('price'));
    const available = parseInt(card.data('stock') || 0);

    if (!name || isNaN(price)) {
        showBanner("Invalid item", "warning");
        return;
    }

    const found = cart.find(item => item.name.toLowerCase() === name.toLowerCase());

    if (found) {
        if (found.quantity >= available) {
            showBanner("Out of stock!", "warning");
            return;
        }
        found.quantity++;
    } else {
        if (available <= 0) {
            showBanner("Out of stock!", "warning");
            return;
        }
        cart.push({ name, price, quantity: 1 });
    }

    sessionStorage.setItem('cart', JSON.stringify(cart));
    renderCart();
    updateUI();
    renderCartCount();
    showBanner(`${name} added to cart`, "success");
});

// Add to cart from scroll section
$(document).on('click', '.scroll-section .add-to-cart', function () {
    if (!isLoggedIn) {
        showBanner("Login first", "danger");
        return;
    }

    const card = $(this).closest('.item-card');
    const name = card.data('name');
    const price = parseFloat(card.data('price'));
    const available = parseInt(card.data('stock') || 0);

    if (!name || isNaN(price)) {
        showBanner("Invalid item", "warning");
        return;
    }

    const found = cart.find(item => item.name.toLowerCase() === name.toLowerCase());

    if (found) {
        if (found.quantity >= available) {
            showBanner("Out of stock!", "warning");
            return;
        }
        found.quantity++;
    } else {
        if (available <= 0) {
            showBanner("Out of stock!", "warning");
            return;
        }
        cart.push({ name, price, quantity: 1 });
    }

    sessionStorage.setItem('cart', JSON.stringify(cart));
    renderCart();
    updateUI();
    renderCartCount();
    showBanner(`${name} added to cart`, "success");
});

// Remove item from cart
$(document).on('click', '.remove-btn', function () {
    const name = $(this).data('name');
    cart = cart.filter(item => item.name.toLowerCase() !== name.toLowerCase());
    sessionStorage.setItem('cart', JSON.stringify(cart));
    renderCart();
    updateUI();
    renderCartCount();
});

// On page ready, initialize
$(document).ready(function () {
    renderCart();
    updateUI();
    renderCartCount();

    // Handle order submission
    $('#orderForm').submit(function (e) {
        e.preventDefault();

        if (!isLoggedIn) {
            showBanner("Please login to place order.", "danger");
            return;
        }

        if (cart.length === 0) {
            showBanner("Cart is empty.", "danger");
            return;
        }

        const data = {
            name: $('#name').val()?.trim(),
            phone: $('#phone').val()?.trim(),
            address: $('#address').val()?.trim(),
            notes: $('#notes').val()?.trim(),
            cart: cart
        };

        if (!data.name || !data.phone || !data.address) {
            showBanner("Please fill all required fields.", "warning");
            return;
        }

        $.ajax({
            url: '/Home/SubmitOrder',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (res) {
                sessionStorage.removeItem('cart');
                showBanner("Order placed successfully!", "success");

                if (res.redirectUrl)
                    window.location.href = res.redirectUrl;
                else
                    window.location.href = '/Home/OrderSummary';
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                showBanner("Failed to place order. " + (xhr.responseText || ""), "danger");
            }
        });
    });
});
