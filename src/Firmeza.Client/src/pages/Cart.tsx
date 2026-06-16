import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCartContext } from '../context/CartContext';
import { useAuth } from '../hooks/useAuth';
import { post } from '../lib/api';

interface SaleResponse {
  id: number;
}

export default function Cart() {
  const { items, removeItem, updateQuantity, clearCart, subtotal, iva, total } = useCartContext();
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  if (!isAuthenticated) {
    return (
      <div className="bg-white rounded-xl shadow p-8 text-center">
        <p className="text-base text-gray-700 mb-4">
          Debes iniciar sesión para comprar.
        </p>
        <Link to="/login" className="bg-gray-800 text-white px-6 py-3 rounded-lg hover:bg-gray-700 text-base">
          Iniciar sesión
        </Link>
      </div>
    );
  }

  if (successMessage) {
    return (
      <div className="bg-green-50 border border-green-200 text-green-700 rounded-lg p-8 text-center text-base">
        {successMessage}
      </div>
    );
  }

  if (items.length === 0) {
    return (
      <div className="bg-white rounded-xl shadow p-8 text-center">
        <p className="text-base text-gray-700 mb-4">Tu carrito está vacío.</p>
        <Link to="/products" className="bg-gray-800 text-white px-6 py-3 rounded-lg hover:bg-gray-700 text-base">
          Ver productos
        </Link>
      </div>
    );
  }

  async function handleCheckout() {
    setError(null);
    setLoading(true);
    try {
      const payload = {
        items: items.map(i => ({ productId: i.productId, quantity: i.quantity })),
      };
      const sale = await post<SaleResponse>('/sales', payload);
      clearCart();
      setSuccessMessage(
        `Venta #${sale.id} registrada. El comprobante fue enviado a tu correo.`,
      );
      setTimeout(() => navigate('/products'), 3000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al procesar la compra');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="max-w-2xl mx-auto">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Carrito de compras</h1>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 rounded-lg p-4 mb-4 text-base">
          {error}
        </div>
      )}

      <div className="bg-white rounded-xl shadow divide-y divide-gray-100">
        {items.map(item => (
          <div key={item.productId} className="p-4 flex items-center gap-4">
            <div className="flex-1">
              <p className="font-bold text-gray-900 text-base">{item.name}</p>
              <p className="text-sm text-gray-500">${item.price.toFixed(2)} c/u</p>
            </div>
            <input
              type="number"
              min={1}
              value={item.quantity}
              onChange={e => updateQuantity(item.productId, Number(e.target.value))}
              className="w-20 border border-gray-300 rounded-lg px-3 py-2 text-base focus:outline-none focus:ring-2 focus:ring-gray-800"
            />
            <p className="w-24 text-right font-semibold text-gray-800 text-base">
              ${(item.price * item.quantity).toFixed(2)}
            </p>
            <button
              onClick={() => removeItem(item.productId)}
              className="text-red-500 hover:text-red-700 text-base px-2"
              aria-label="Eliminar"
            >
              ✕
            </button>
          </div>
        ))}
      </div>

      <div className="bg-white rounded-xl shadow p-6 mt-4 space-y-2">
        <div className="flex justify-between text-base text-gray-700">
          <span>Subtotal</span>
          <span>${subtotal.toFixed(2)}</span>
        </div>
        <div className="flex justify-between text-base text-gray-700">
          <span>IVA (19%)</span>
          <span>${iva.toFixed(2)}</span>
        </div>
        <div className="flex justify-between text-lg font-bold text-gray-900 pt-2 border-t border-gray-100">
          <span>Total</span>
          <span>${total.toFixed(2)}</span>
        </div>
      </div>

      <button
        onClick={handleCheckout}
        disabled={loading}
        className="mt-6 w-full bg-gray-800 text-white px-6 py-3 rounded-lg hover:bg-gray-700 disabled:opacity-50 text-base"
      >
        {loading ? 'Procesando...' : 'Confirmar compra'}
      </button>
    </div>
  );
}