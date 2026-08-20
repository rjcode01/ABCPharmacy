const $ = (s) => document.querySelector(s);

const api = "/api";
function toast(msg) {
  const t = $("#toast")
  t.textContent = msg
  t.style.display = "block"
  setTimeout(() => (t.style.display = "none"), 2500)
}

function money(v) {
  return new Intl.NumberFormat("en-IN", {
    style: "currency",
    currency: "INR",
  }).format(v);
}

function formatDate(v) {
  return new Date(v).toLocaleDateString("en-IN");
}

async function loadMedicines() {
  const q = $("#search").value.trim();
  const res = await fetch(
    `${api}/medicines${q ? "?search=" + encodeURIComponent(q) : ""}`,
  );
  const items = await res.json();
  const tbody = $("#medicineRows");
  tbody.innerHTML = "";
  items.forEach((m) => {
    const days = (new Date(m.expiryDate) - new Date()) / 86400000;
    const tr = document.createElement("tr");
    if (days < 30) tr.classList.add("expiring");
    if (m.quantity < 10) tr.classList.add("low-stock");
    tr.innerHTML = `<td><strong>${m.fullName}</strong></td><td>${formatDate(m.expiryDate)}</td><td>${m.quantity}</td><td>${money(m.price)}</td><td>${m.brand}</td><td><button class="sell" data-id="${m.id}" data-name="${m.fullName}" ${m.quantity === 0 ? "disabled" : ""}>Sell</button></td>`;
    tbody.appendChild(tr);
  });
  document
    .querySelectorAll(".sell")
    .forEach(
      (b) => (b.onclick = () => openSale(+b.dataset.id, b.dataset.name)),
    );

}


async function loadSales() {
  const res = await fetch(`${api}/sales`);
  const items = await res.json();
  $("#salesRows").innerHTML =
    items
      .map(
        (s) =>
          `<tr><td>${s.medicineName}</td><td>${s.quantitySold}</td><td>${money(s.unitPrice)}</td><td>${money(s.totalAmount)}</td><td>${new Date(s.soldAt).toLocaleString("en-IN")}</td></tr>`,
      )
      .join("") || '<tr><td colspan="5">No sales recorded.</td></tr>';
}

$("#search").addEventListener("input", loadMedicines);

$("#refreshBtn").onclick = loadMedicines;
$("#salesRefresh").onclick = loadSales;

$("#addBtn").onclick = () => $("#modal").classList.remove("hidden");
$("#closeModal").onclick = () => $("#modal").classList.add("hidden");

$("#medicineForm").onsubmit = async (e) => {
  e.preventDefault();
  const f = new FormData(e.target);
  const body = Object.fromEntries(f.entries());
  body.quantity = +body.quantity;
  body.price = +body.price;
  const res = await fetch(`${api}/medicines`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
  if (!res.ok) {
    toast(await res.text());
    return;
  }
  e.target.reset();
  $("#modal").classList.add("hidden");
  toast("Medicine added successfully");
  loadMedicines();
};


function openSale(id, name) {
  $("#saleForm [name=medicineId]").value = id;
  $("#saleMedicine").textContent = `Medicine: ${name}`;
  $("#saleModal").classList.remove("hidden");
}

$("#closeSale").onclick = () => $("#saleModal").classList.add("hidden");

$("#saleForm").onsubmit = async (e) => {
  e.preventDefault();
  const f = new FormData(e.target);
  const body = {
    medicineId: +f.get("medicineId"),
    quantitySold: +f.get("quantitySold"),
  };
  const res = await fetch(`${api}/sales`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
  if (!res.ok) {
    toast(await res.text());
    return;
  }
  e.target.reset();
  $("#saleModal").classList.add("hidden");
  toast("Sale recorded and stock updated");
  loadMedicines();
  loadSales();
};


document.querySelectorAll(".nav-btn").forEach(
  (b) =>
    (b.onclick = () => {
      document
        .querySelectorAll(".nav-btn")
        .forEach((x) => x.classList.remove("active"));
      b.classList.add("active");
      const sales = b.dataset.view === "sales";
      $("#medicinesView").classList.toggle("hidden", sales);
      $("#salesView").classList.toggle("hidden", !sales);
      if (sales) loadSales();
    }),
);

loadMedicines();
