// Change the port if your backend console shows a different one
const API = 'https://localhost:7146/api/products';

const $ = (id) => document.getElementById(id);
let cache = [];

function show(text, ok = true) {
  $('msg').textContent = text;
  $('msg').className = ok ? 'ok' : 'err';
  setTimeout(() => ($('msg').className = ''), 5000);
}

async function readError(res) {
  try {
    const d = await res.json();
    if (d.errors) return Object.values(d.errors).flat().join('\n'); // 400
    if (d.message) return d.message;                                 // 404 / 409
  } catch {}
  return `Request failed (${res.status})`;
}

async function loadProducts() {
  try {
    const params = new URLSearchParams();
    if ($('fCategory').value.trim()) params.set('category', $('fCategory').value.trim());
    if ($('fSearch').value.trim()) params.set('search', $('fSearch').value.trim());

    const res = await fetch(`${API}?${params}`);
    if (!res.ok) return show(await readError(res), false);
    cache = await res.json();

    $('rows').innerHTML = '';
    $('empty').style.display = cache.length ? 'none' : 'block';

    for (const p of cache) {
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${p.id}</td><td></td><td></td>
        <td>${p.price.toFixed(2)}</td><td>${p.stockQuantity}</td><td></td>
        <td><button onclick="editProduct(${p.id})">Edit</button>
            <button class="red" onclick="deleteProduct(${p.id})">Delete</button></td>`;
      tr.children[1].textContent = p.name;
      tr.children[2].textContent = p.category;
      tr.children[5].textContent = p.description ?? '';
      $('rows').appendChild(tr);
    }
  } catch {
    show('Cannot reach the API. Is the backend running at ' + API + ' ?', false);
  }
}

async function saveProduct() {
  const id = $('pid').value;
  const body = {
    name: $('name').value,
    category: $('category').value,
    description: $('description').value || null,
    price: $('price').value === '' ? 0 : parseFloat($('price').value),
    stockQuantity: $('stock').value === '' ? -1 : parseInt($('stock').value, 10),
  };

  try {
    const res = await fetch(id ? `${API}/${id}` : API, {
      method: id ? 'PUT' : 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    });
    if (!res.ok) return show(await readError(res), false);
    show(id ? 'Product updated.' : 'Product created.');
    resetForm();
    loadProducts();
  } catch {
    show('Cannot reach the API.', false);
  }
}

function editProduct(id) {
  const p = cache.find((x) => x.id === id);
  $('pid').value = p.id;
  $('name').value = p.name;
  $('category').value = p.category;
  $('price').value = p.price;
  $('stock').value = p.stockQuantity;
  $('description').value = p.description ?? '';
  $('formTitle').textContent = `Edit product #${p.id}`;
  window.scrollTo({ top: 0, behavior: 'smooth' });
}

async function deleteProduct(id) {
  if (!confirm('Delete this product?')) return;
  try {
    const res = await fetch(`${API}/${id}`, { method: 'DELETE' });
    if (!res.ok) return show(await readError(res), false);
    show('Product deleted.');
    loadProducts();
  } catch {
    show('Cannot reach the API.', false);
  }
}

function resetForm() {
  ['pid', 'name', 'category', 'price', 'stock', 'description'].forEach((i) => ($(i).value = ''));
  $('formTitle').textContent = 'Add product';
}

function clearFilters() {
  $('fCategory').value = '';
  $('fSearch').value = '';
  loadProducts();
}

loadProducts();