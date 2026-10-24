let items = new Map();

items.set("Milk", 50);
items.set("Rice", 200);
items.set("Pen", 10);
items.set("Bread", 40);

let cart = [];

function addCart() {
    let prod = document.getElementById("prod").value;
    let quant = Number(document.getElementById("quant").value);

    if (quant <= 0) {
        document.getElementById("res").innerHTML =
            "Please enter a valid quantity";
        return;
    }

    if (items.has(prod)) {
        let cost = items.get(prod);
        let price = cost * quant;

        cart.push({
            product: prod,
            quantity: quant,
            cost: cost,
            price: price
        });

        document.getElementById("res").innerHTML =
            `${prod} added to cart. Subtotal: ₹${price}`;

    } else {
        document.getElementById("res").innerHTML =
            `${prod} not available`;
    }

    document.getElementById("quant").value = "";
}

function bill() {
    let total = 0;
    let output = "<h3>Jes Store Bill</h3>";

    for (let item of cart) {
        output += `
            ${item.product}:
            ${item.quantity} × ₹${item.cost}
            = ₹${item.price}<br>
        `;

        total += item.price;
    }

    output += `<h3>Grand Total: ₹${total}</h3>`;

    document.getElementById("res").innerHTML = output;
}