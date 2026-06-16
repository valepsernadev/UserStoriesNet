import { createContext, useContext } from 'react';
import type { CartItem } from '../hooks/useCart';

interface CartProduct {
  id: number;
  name: string;
  price: number;
}

export interface CartContextType {
  items: CartItem[];
  addItem: (product: CartProduct, quantity: number) => void;
  removeItem: (productId: number) => void;
  updateQuantity: (productId: number, qty: number) => void;
  clearCart: () => void;
  subtotal: number;
  iva: number;
  total: number;
}

export const CartContext = createContext<CartContextType | null>(null);

export function useCartContext(): CartContextType {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCartContext must be used within CartContext.Provider');
  return ctx;
}