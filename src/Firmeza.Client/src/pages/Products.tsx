import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { get } from '../lib/api';
import { useCartContext } from '../context/CartContext';
import { useAuth } from '../hooks/useAuth';

interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  isActive: boolean;
}

export default function Products() {
  const { addItem } = useCartContext();
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [quantities, setQuantities] = useState<Record<number, number>>({});
  const [added, setAdded] = useState<Record<number, boolean>>({});

  useEffect(() => {
    get<Product[]>('/products')
      .then(data => setProducts(data))
      .catch(err => setError(err instanceof Error ? err.message : 'Error al cargar productos'))
      .finally(() => setLoading(false));
  }, []);

  function getQty(id: number): number {
    return quantities[id] ?? 1;
  }

  function setQty(id: number, value: number) {
    setQuantities(prev => ({ ...prev, [id]: Math.max(1, value) }));
  }

  function handleAdd(product: Product) {
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }
    addItem({ id: product.id, name: product.name, price: product.price }, getQty(product.id));
    setAdded(prev => ({ ...prev, [product.id]: true }));
    setTimeout(() => setAdded(prev => ({ ...prev, [product.id]: false })), 1500);
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20 text-gray-500 text-base">
        Cargando productos...
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 text-red-700 rounded-lg p-4 text-base">
        {error}
      </div>
    );
  }

  if (products.length === 0) {
    return (
      <div className="text-center py-20 text-gray-500 text-base">
        No hay productos disponibles por el momento.
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Productos</h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        {products.map(product => (
          <div key={product.id} className="bg-white rounded-xl shadow p-6 flex flex-col gap-3">
            <h2 className="font-bold text-lg text-gray-900">{product.name}</h2>
            {product.description && (
              <p className="text-gray-500 text-base flex-1">{product.description}</p>
            )}
            <p className="text-xl font-semibold text-gray-800">
              ${product.price.toFixed(2)}
            </p>
            <p className="text-sm text-gray-500">
              {product.stock > 0 ? `${product.stock} en stock` : 'Sin stock'}
            </p>
            <div className="flex items-center gap-3 mt-2">
              {isAuthenticated && (
                <input
                  type="number"
                  min={1}
                  max={product.stock}
                  value={getQty(product.id)}
                  onChange={e => setQty(product.id, Number(e.target.value))}
                  className="w-20 border border-gray-300 rounded-lg px-3 py-2 text-base focus:outline-none focus:ring-2 focus:ring-gray-800"
                  disabled={product.stock === 0}
                />
              )}
              {isAuthenticated ? (
                <button
                  onClick={() => handleAdd(product)}
                  disabled={product.stock === 0}
                  className="flex-1 bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-500 disabled:opacity-50 text-base"
                >
                  {added[product.id] ? '¡Agregado!' : 'Agregar al carrito'}
                </button>
              ) : (
                <button
                  onClick={() => navigate('/login')}
                  className="flex-1 bg-gray-800 text-white px-6 py-3 rounded-lg hover:bg-gray-700 text-base"
                >
                  Añadir al carrito
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
