const BASE_URL = '/api';

function getHeaders(): Record<string, string> {
  const headers: Record<string, string> = { 'Content-Type': 'application/json' };
  const token = localStorage.getItem('firmeza_token');
  if (token) headers['Authorization'] = `Bearer ${token}`;
  return headers;
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (response.status === 401) {
    localStorage.removeItem('firmeza_token');
    window.location.href = '/login';
    throw new Error('Sesión expirada. Por favor inicia sesión nuevamente.');
  }

  if (response.status === 204) return undefined as T;

  if (!response.ok) {
    let message = `Error ${response.status}`;
    try {
      const data = await response.json();
      message =
        data.message ??
        data.error ??
        (Array.isArray(data.errors) ? data.errors.join(', ') : undefined) ??
        message;
    } catch {
      // use default message
    }
    throw new Error(message);
  }

  return response.json() as Promise<T>;
}

export function get<T>(path: string): Promise<T> {
  return fetch(`${BASE_URL}${path}`, { headers: getHeaders() }).then(handleResponse<T>);
}

export function post<T>(path: string, body?: unknown): Promise<T> {
  return fetch(`${BASE_URL}${path}`, {
    method: 'POST',
    headers: getHeaders(),
    body: body !== undefined ? JSON.stringify(body) : undefined,
  }).then(handleResponse<T>);
}

export function put<T>(path: string, body?: unknown): Promise<T> {
  return fetch(`${BASE_URL}${path}`, {
    method: 'PUT',
    headers: getHeaders(),
    body: body !== undefined ? JSON.stringify(body) : undefined,
  }).then(handleResponse<T>);
}

export function del<T>(path: string): Promise<T> {
  return fetch(`${BASE_URL}${path}`, {
    method: 'DELETE',
    headers: getHeaders(),
  }).then(handleResponse<T>);
}