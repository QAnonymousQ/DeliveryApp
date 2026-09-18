import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { getOrders } from '../api/ordersApi'

function formatNumber(value) {
  return new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 2 }).format(Number(value))
}

function formatDate(value) {
  if (!value) return '—'
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return '—'
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${day}.${month}.${d.getFullYear()}`
}

function formatDateTime(value) {
  if (!value) return '—'
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return '—'
  const time = String(d.getHours()).padStart(2, '0') + ':' + String(d.getMinutes()).padStart(2, '0')
  return `${formatDate(value)}, ${time}`
}

function OrderListPage() {
  const navigate = useNavigate()
  const [orders, setOrders] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    let cancelled = false
    getOrders()
      .then((data) => {
        if (!cancelled) setOrders(data)
      })
      .catch((err) => {
        if (!cancelled) setError(err.message)
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [])

  if (loading) {
    return <div className="card"><p className="card__empty">Загрузка...</p></div>
  }

  if (error) {
    return <div className="card"><div className="alert alert--error">{error}</div></div>
  }

  return (
    <div className="card">
      <div className="card__header">
        <div>
          <h1 className="card__title">Список заказов</h1>
          <p className="card__subtitle">
            {orders.length === 0 ? 'Заказов пока нет' : `Всего заказов: ${orders.length}`}
          </p>
        </div>
        <Link className="btn btn--primary" to="/create">
          Новый заказ
        </Link>
      </div>

      {orders.length === 0 ? (
        <p className="card__empty">
          Заказов нет.{' '}
          <Link to="/create">Создайте первый заказ</Link>
        </p>
      ) : (
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Номер заказа</th>
                <th>Отправитель</th>
                <th>Получатель</th>
                <th className="table__num">Вес, кг</th>
                <th>Дата забора</th>
                <th>Создан</th>
              </tr>
            </thead>
            <tbody>
              {orders.map((order) => (
                <tr
                  key={order.id}
                  className="table__row"
                  onClick={() => navigate(`/orders/${order.id}`)}
                >
                  <td><span className="order-number">{order.orderNumber}</span></td>
                  <td>
                    {order.senderCity}, {order.senderAddress}
                  </td>
                  <td>
                    {order.recipientCity}, {order.recipientAddress}
                  </td>
                  <td className="table__num">{formatNumber(order.cargoWeight)}</td>
                  <td>{formatDate(order.pickupDate)}</td>
                  <td>{formatDateTime(order.createdAt)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

export default OrderListPage