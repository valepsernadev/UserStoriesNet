import { useState, useCallback } from 'react';

export interface CartItem {
  productId: number;
  name: string;
  price: number;
  quantity: number;
}

interface CartProduct {
  id: number;
  name: string;
  price: number;
}

export function useCart() {
  const [items, setItems] = useState<CartItem[]>([]);

  const addItem = useCallback((product: CartProduct, quantity: number) => {
    setItems(prev => {
      const existing = prev.find(i => i.productId === product.id);
      if (existing) {
        return prev.map(i =>
          i.productId === product.id ? { ...i, quantity: i.quantity + quantity } : i,
        );
      }
      return [...prev, { productId: product.id, name: product.name, price: product.price, quantity }];
    });
  }, []);

  const removeItem = useCallback((productId: number) => {
    setItems(prev => prev.filter(i => i.productId !== productId));
  }, []);

  const updateQuantity = useCallback((productId: number, qty: number) => {
    if (qty <= 0) {
      setItems(prev => prev.filter(i => i.productId !== productId));
      return;
    }
    setItems(prev => prev.map(i => (i.productId === productId ? { ...i, quantity: qty } : i)));
  }, []);

  const clearCart = useCallback(() => {
    setItems([]);
  }, []);

  const subtotal = items.reduce((sum, i) => sum + i.price * i.quantity, 0);
  const iva = subtotal * 0.19;
  const total = subtotal + iva;

  return { items, addItem, removeItem, updateQuantity, clearCart, subtotal, iva, total };
}