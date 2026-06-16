import { useCallback } from 'react';

interface JwtPayload {
  sub: string;
  email?: string;
  role?: string;
  exp: number;
  [key: string]: unknown;
}

function decodeJwt(token: string): JwtPayload | null {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) return null;
    const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4);
    return JSON.parse(atob(padded)) as JwtPayload;
  } catch {
    return null;
  }
}

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';

export function useAuth() {
  const token = localStorage.getItem('firmeza_token');

  let email: string | null = null;
  let role: string | null = null;
  let isAuthenticated = false;

  if (token) {
    const payload = decodeJwt(token);
    if (payload && payload.exp > Date.now() / 1000) {
      const rawEmail = payload.email ?? payload[EMAIL_CLAIM];
      email = Array.isArray(rawEmail) ? (rawEmail[0] as string) : (rawEmail as string) ?? null;

      const rawRole = payload.role ?? payload[ROLE_CLAIM];
      role = Array.isArray(rawRole) ? (rawRole[0] as string) : (rawRole as string) ?? null;

      isAuthenticated = true;
    }
  }

  const logout = useCallback(() => {
    localStorage.removeItem('firmeza_token');
    window.location.href = '/login';
  }, []);

  return { token, email, role, isAuthenticated, logout };
}