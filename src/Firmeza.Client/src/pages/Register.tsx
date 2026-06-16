import { useState, FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import * as api from '../lib/api';

export default function Register() {
  const navigate = useNavigate();

  const [fullName, setFullName] = useState('');
  const [document, setDocument] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      await api.post('/auth/register', { email, password });
      await api.post('/clients', { fullName, document, email, phone });
      navigate('/login', { state: { message: 'Registro exitoso. Inicia sesión.' } });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al registrarse');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen bg-gray-100 flex flex-col">
      <main className="flex-1 flex items-center justify-center px-4 py-8">
        <div className="bg-white rounded-xl shadow p-8 w-full max-w-md">
          <h1 className="text-2xl font-bold text-gray-900 mb-6">Crear cuenta</h1>

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 rounded-lg p-4 mb-4 text-base">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Nombre completo</label>
              <input
                type="text"
                value={fullName}
                onChange={e => setFullName(e.target.value)}
                required
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-base focus:outline-none focus:ring-2 focus:ring-gray-800"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Documento</label>
              <input
                type="text"
                value={document}
                onChange={e => setDocument(e.target.value)}
                required
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-base focus:outline-none focus:ring-2 focus:ring-gray-800"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
              <input
                type="email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                required
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-base focus:outline-none focus:ring-2 focus:ring-gray-800"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Teléfono</label>
              <input
                type="tel"
                value={phone}
                onChange={e => setPhone(e.target.value)}
                required
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-base focus:outline-none focus:ring-2 focus:ring-gray-800"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Contraseña</label>
              <input
                type="password"
                value={password}
                onChange={e => setPassword(e.target.value)}
                required
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-base focus:outline-none focus:ring-2 focus:ring-gray-800"
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-gray-800 text-white px-6 py-3 rounded-lg hover:bg-gray-700 disabled:opacity-50 text-base"
            >
              {loading ? 'Registrando...' : 'Registrarse'}
            </button>
          </form>

          <p className="mt-4 text-sm text-gray-600 text-center">
            ¿Ya tienes cuenta?{' '}
            <Link to="/login" className="text-gray-800 font-medium hover:underline">
              Inicia sesión
            </Link>
          </p>
        </div>
      </main>
      <footer className="bg-gray-800 text-gray-400 text-center py-4 text-base">
        Firmeza © 2026 — Materiales de Construcción
      </footer>
    </div>
  );
}