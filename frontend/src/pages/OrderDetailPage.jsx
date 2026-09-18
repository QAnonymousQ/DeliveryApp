import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getOrder } from '../api/ordersApi'

function formatDate(value) {
  if (!value) return '—'
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return '—'
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${day}.${month}.${d.getFullYear()}`
}

function formatWeight(value) {
  return new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 2 }).format(Number(value))
}

function Field({ label, children }) {
  return (
    <div className="detail-field">
      <span className="detail-field__label">{label}</span>
      <span className="detail-field__value">{children}</span>
    </div>
  )
}

function OrderDetailPage() {
  const { id } = useParams()
  const [order, setOrder] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    let cancelled = false
    getOrder(id)
      .then((data) => {
        if (!cancelled) setOrder(data)
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
  }, [id])

  if (loading) {
    return <div className="card"><p className="card__empty">Загрузка...</p></div>
  }

  if (error) {
    return (
      <div className="card">
        <div className="alert alert--error">{error}</div>
        <Link className="btn btn--ghost" to="/orders">К списку заказов</Link>
      </div>
    )
  }

  return (
    <div className="card">
      <div className="card__header">
        <div>
          <h1 className="card__title">Заказ {order.orderNumber}</h1>
          <p className="card__subtitle">Режим просмотра (только чтение)</p>
        </div>
      </div>

      <div className="detail">
        <section className="detail__section">
          <h2 className="detail__heading">Отправитель</h2>
          <Field label="Город отправителя">{order.senderCity}</Field>
          <Field label="Адрес отправителя">{order.senderAddress}</Field>
        </section>

        <section className="detail__section">
          <h2 className="detail__heading">Получатель</h2>
          <Field label="Город получателя">{order.recipientCity}</Field>
          <Field label="Адрес получателя">{order.recipientAddress}</Field>
        </section>

        <section className="detail__section">
          <h2 className="detail__heading">Груз</h2>
          <Field label="Вес груза">{formatWeight(order.cargoWeight)} кг</Field>
          <Field label="Дата забора груза">{formatDate(order.pickupDate)}</Field>
        </section>
      </div>

      <div className="form__actions">
        <Link className="btn btn--primary" to="/orders">К списку заказов</Link>
      </div>
    </div>
  )
}

export default OrderDetailPage