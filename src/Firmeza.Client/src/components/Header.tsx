import { Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

interface HeaderProps {
  cartCount: number;
}

export default function Header({ cartCount }: HeaderProps) {
  const { email, isAuthenticated, logout } = useAuth();

  return (
    <header className="bg-gray-900 text-white px-6 py-4">
      <div className="max-w-6xl mx-auto flex items-center justify-between">
        <Link to="/products" className="text-xl font-bold tracking-wide hover:text-gray-300">
          Firmeza
        </Link>

        <nav className="flex items-center gap-6 text-base">
          {isAuthenticated ? (
            <>
              <Link to="/products" className="hover:text-gray-300">
                Productos
              </Link>
              <Link to="/cart" className="relative hover:text-gray-300">
                Carrito
                {cartCount > 0 && (
                  <span className="absolute -top-2 -right-5 bg-green-600 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                    {cartCount}
                  </span>
                )}
              </Link>
              <span className="text-gray-400 text-sm hidden sm:inline">{email}</span>
              <button
                onClick={logout}
                className="bg-gray-700 hover:bg-gray-600 text-white px-4 py-2 rounded-lg text-sm"
              >
                Cerrar sesión
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="hover:text-gray-300">
                Iniciar sesión
              </Link>
              <Link
                to="/register"
                className="bg-green-600 hover:bg-green-500 text-white px-4 py-2 rounded-lg text-sm"
              >
                Registrarse
              </Link>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}