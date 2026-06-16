import { BrowserRouter, Routes, Route, Navigate, Outlet, useLocation } from 'react-router-dom';
import { CartContext } from './context/CartContext';
import { useCart } from './hooks/useCart';
import Header from './components/Header';
import Footer from './components/Footer';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';
import Register from './pages/Register';
import Products from './pages/Products';
import Cart from './pages/Cart';

function MainLayout({ cartCount }: { cartCount: number }) {
  return (
    <div className="bg-gray-100 min-h-screen flex flex-col">
      <Header cartCount={cartCount} />
      <main className="flex-1 max-w-6xl mx-auto w-full px-6 py-8">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}

function AppRoutes({ cartCount }: { cartCount: number }) {
  const location = useLocation();
  const isAuthPage = location.pathname === '/login' || location.pathname === '/register';

  if (isAuthPage) {
    return (
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
      </Routes>
    );
  }

  return (
    <Routes>
      <Route element={<MainLayout cartCount={cartCount} />}>
        <Route path="/" element={<Navigate to="/products" replace />} />
        <Route path="/products" element={<Products />} />
        <Route element={<ProtectedRoute />}>
          <Route path="/cart" element={<Cart />} />
        </Route>
        <Route path="*" element={<Navigate to="/products" replace />} />
      </Route>
    </Routes>
  );
}

export default function App() {
  const cart = useCart();

  return (
    <CartContext.Provider value={cart}>
      <BrowserRouter>
        <AppRoutes cartCount={cart.items.length} />
      </BrowserRouter>
    </CartContext.Provider>
  );
}