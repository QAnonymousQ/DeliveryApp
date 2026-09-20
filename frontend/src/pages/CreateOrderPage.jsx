import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { createOrder } from '../api/ordersApi'

//Starting form state
const initialForm = {
  senderCity: '',
  senderAddress: '',
  recipientCity: '',
  recipientAddress: '',
  cargoWeight: '',
  pickupDate: ''
}

//Today's date
function today() {
  const d = new Date()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${d.getFullYear()}-${month}-${day}`
}

function CreateOrderPage() {

  //Order states
  const navigate = useNavigate()
  const [form, setForm] = useState(initialForm)
  const [errors, setErrors] = useState({})
  const [serverError, setServerError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  //Handling field changes
  const handleChange = (e) => {
    const { name, value } = e.target
    setForm((f) => ({ ...f, [name]: value }))
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: '' }))
    }
  }

  //Fields validation
  const validate = () => {
    const next = {}
    if (!form.senderCity.trim()) next.senderCity = 'Укажите город отправителя'
    if (!form.senderAddress.trim()) next.senderAddress = 'Укажите адрес отправителя'
    if (!form.recipientCity.trim()) next.recipientCity = 'Укажите город получателя'
    if (!form.recipientAddress.trim()) next.recipientAddress = 'Укажите адрес получателя'

    const weight = Number(form.cargoWeight)
    if (form.cargoWeight === '' || !Number.isFinite(weight) || weight <= 0) {
      next.cargoWeight = 'Вес груза должен быть больше 0'
    }

    if (!form.pickupDate) {
      next.pickupDate = 'Укажите дату забора груза'
    } else if (form.pickupDate < today()) {
      next.pickupDate = 'Дата забора не может быть в прошлом'
    }
    return next
  }

  //Calling after pressing CreateOrder button
  const handleSubmit = async (e) => {
    //Prevent page reloading
    e.preventDefault()
    //Clearing server errors 
    setServerError('')

    
    const next = validate()
    setErrors(next)

    if (Object.keys(next).length > 0) return

    setSubmitting(true)
    //Sending form on server
    try {
      const order = await createOrder({
        senderCity: form.senderCity.trim(),
        senderAddress: form.senderAddress.trim(),
        recipientCity: form.recipientCity.trim(),
        recipientAddress: form.recipientAddress.trim(),
        cargoWeight: Number(form.cargoWeight),
        pickupDate: form.pickupDate
      })
      navigate(`/orders/${order.id}`)
    } catch (err) {
      setServerError(err.message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="card">
      <h1 className="card__title">Новый заказ</h1>

      {serverError && <div className="alert alert--error">{serverError}</div>}

      <form className="form" onSubmit={handleSubmit} noValidate>
        <fieldset className="form__section">
          <legend>Отправитель</legend>
          <label className="field">
            <span className="field__label">Город отправителя</span>
            <input
              className="field__input"
              name="senderCity"
              type="text"
              value={form.senderCity}
              onChange={handleChange}
            />
            {errors.senderCity && <span className="field__error">{errors.senderCity}</span>}
          </label>
          <label className="field">
            <span className="field__label">Адрес отправителя</span>
            <input
              className="field__input"
              name="senderAddress"
              type="text"
              value={form.senderAddress}
              onChange={handleChange}
            />
            {errors.senderAddress && <span className="field__error">{errors.senderAddress}</span>}
          </label>
        </fieldset>

        <fieldset className="form__section">
          <legend>Получатель</legend>
          <label className="field">
            <span className="field__label">Город получателя</span>
            <input
              className="field__input"
              name="recipientCity"
              type="text"
              value={form.recipientCity}
              onChange={handleChange}
            />
            {errors.recipientCity && <span className="field__error">{errors.recipientCity}</span>}
          </label>
          <label className="field">
            <span className="field__label">Адрес получателя</span>
            <input
              className="field__input"
              name="recipientAddress"
              type="text"
              value={form.recipientAddress}
              onChange={handleChange}
            />
            {errors.recipientAddress && (
              <span className="field__error">{errors.recipientAddress}</span>
            )}
          </label>
        </fieldset>

        <fieldset className="form__section">
          <legend>Груз</legend>
          <label className="field">
            <span className="field__label">Вес груза, кг</span>
            <input
              className="field__input"
              name="cargoWeight"
              type="number"
              min="0"
              step="any"
              value={form.cargoWeight}
              onChange={handleChange}
            />
            {errors.cargoWeight && <span className="field__error">{errors.cargoWeight}</span>}
          </label>
          <label className="field">
            <span className="field__label">Дата забора груза</span>
            <input
              className="field__input"
              name="pickupDate"
              type="date"
              min={today()}
              value={form.pickupDate}
              onChange={handleChange}
            />
            {errors.pickupDate && <span className="field__error">{errors.pickupDate}</span>}
          </label>
        </fieldset>

        <div className="form__actions">
          <button className="btn btn--primary" type="submit" disabled={submitting}>
            {submitting ? 'Создание...' : 'Создать заказ'}
          </button>
          <Link className="btn btn--ghost" to="/orders">
            Отмена
          </Link>
        </div>
        <p className="card__subtitle">*Все поля обязательны для заполнения</p>
      </form>
    </div>
  )
}

export default CreateOrderPage