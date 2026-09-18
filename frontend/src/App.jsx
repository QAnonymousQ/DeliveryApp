import { NavLink, Outlet } from 'react-router-dom'

function App() {
  return (
    <div className="layout">
      <header className="layout__header">
        <div className="layout__brand">Доставка</div>
        <nav className="layout__nav">
          <NavLink
            to="/orders"
            className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}
          >
            Заказы
          </NavLink>
          <NavLink
            to="/create"
            className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}
          >
            Новый заказ
          </NavLink>
        </nav>
      </header>

      <main className="layout__content">
        <Outlet />
      </main>
    </div>
  )
}

export default App