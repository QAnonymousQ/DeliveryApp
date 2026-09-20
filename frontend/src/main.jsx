import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import App from './App.jsx'
import CreateOrderPage from './pages/CreateOrderPage.jsx'
import OrderListPage from './pages/OrderListPage.jsx'
import OrderDetailPage from './pages/OrderDetailPage.jsx'

//Routing configuration
const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      { index: true, element: <OrderListPage /> },
      { path: 'orders', element: <OrderListPage /> },
      { path: 'create', element: <CreateOrderPage /> },
      { path: 'orders/:id', element: <OrderDetailPage /> }
    ]
  }
])

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
)