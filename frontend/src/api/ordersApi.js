const API = "/api/orders";

export async function getOrders() {
    const res = await fetch(API);
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`);
    return await res.json();
}

export async function getOrder(id) {
    const res = await fetch(`${API}/${id}`);
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`);
    return await res.json();
}

function flattenErrors(body) {
    if (body?.error) return body.error;
    if (Array.isArray(body?.errors)) return body.errors.join("; ");
    if (body?.errors && typeof body.errors === "object") {
        return Object.values(body.errors).flat().join("; ");
    }
    if (typeof body?.title === "string") return body.title;
    return "Не удалось создать заказ";
}

export async function createOrder(data) {
    const res = await fetch(API, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });
    const body = await res.json().catch(() => null);
    if (!res.ok) {
        throw new Error(flattenErrors(body));
    }
    return body;
}