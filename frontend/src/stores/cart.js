// src/stores/cart.js
import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '../services/api' // 你現有的 axios instance

export const useCartStore = defineStore('cart', () => {
  const cart = ref(null) // cartDto or null
  const loading = ref(false)
  const error = ref(null)

  async function fetchCart(userId) {
    loading.value = true
    error.value = null
    try {
      const res = await api.get(`/Cart/${userId}`)
      cart.value = res.data
    } catch (err) {
      // 若 404（尚未建立 cart），初始化一個空 cart
      if (err?.response?.status === 404) {
        cart.value = { id: null, userId, items: [] }
      } else {
        error.value = err?.response?.data || err.message || JSON.stringify(err)
      }
    } finally {
      loading.value = false
    }
  }

  async function addToCart(userId, productId, quantity = 1) {
    loading.value = true
    error.value = null
    try {
      const res = await api.post(`/Cart/${userId}`, { productId, quantity })
      cart.value = res.data
      return res.data
    } catch (err) {
      error.value = err?.response?.data || err.message || JSON.stringify(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateItem(userId, cartItemId, quantity) {
    loading.value = true
    error.value = null
    try {
      await api.put(`/Cart/${userId}/${cartItemId}`, quantity)
      // 更新本地 cart（若有）
      const item = cart.value?.items?.find(i => i.id === cartItemId)
      if (item) item.quantity = quantity
    } catch (err) {
      error.value = err?.response?.data || err.message || JSON.stringify(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function removeItem(userId, cartItemId) {
    loading.value = true
    error.value = null
    try {
      await api.delete(`/Cart/${userId}/${cartItemId}`)
      if (cart.value) {
        cart.value.items = cart.value.items.filter(i => i.id !== cartItemId)
      }
    } catch (err) {
      error.value = err?.response?.data || err.message || JSON.stringify(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  function itemCount() {
    if (!cart.value || !cart.value.items) return 0
    return cart.value.items.reduce((s, i) => s + (i.quantity || 0), 0)
  }

  return {
    cart,
    loading,
    error,
    fetchCart,
    addToCart,
    updateItem,
    removeItem,
    itemCount
  }
})
