<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const loading = ref(false)
const error = ref(null)
const cart = ref(null)

// 測試用 userId
const userId = 1

async function fetchCart() {
  loading.value = true
  error.value = null
  try {
    const res = await api.get(`/Cart/${userId}`)
    cart.value = res.data
  } catch (err) {
    // 若後端回傳 404（沒有購物車），設定空購物車物件
    if (err?.response?.status === 404) {
      cart.value = { id: null, userId, items: [] }
    } else {
      error.value = err?.response?.data || err.message || 'Unknown error'
    }
  } finally {
    loading.value = false
  }
}

async function updateQuantity(item, newQty) {
  if (newQty < 1) return
  try {
    await api.put(`/Cart/${userId}/${item.id}`, newQty)
    item.quantity = newQty
  } catch (err) {
    alert('更新數量失敗：' + (err?.message || ''))
  }
}

async function removeItem(item) {
  if (!confirm('確定要移除嗎？')) return
  try {
    await api.delete(`/Cart/${userId}/${item.id}`)
    // 本地移除
    cart.value.items = cart.value.items.filter(i => i.id !== item.id)
  } catch (err) {
    alert('刪除失敗：' + (err?.message || ''))
  }
}

onMounted(() => {
  fetchCart()
})
</script>

<template>
  <div>
    <h2>我的購物車</h2>

    <div v-if="loading">載入中…</div>
    <div v-else-if="error" style="color:crimson">{{ error }}</div>
    <div v-else>
      <div v-if="!cart || cart.items.length === 0">購物車空空如也</div>

      <div v-else>
        <div v-for="item in cart.items" :key="item.id" class="cart-item">
          <div class="left">
            <div class="title">{{ item.product.name }}</div>
            <div class="desc">{{ item.product.description }}</div>
            <div>單價：NT$ {{ item.product.price }}</div>
          </div>
          <div class="right">
            <div>
              數量：
              <button @click="updateQuantity(item, item.quantity - 1)">-</button>
              <span>{{ item.quantity }}</span>
              <button @click="updateQuantity(item, item.quantity + 1)">+</button>
            </div>
            <div>
              <button @click="removeItem(item)">移除</button>
            </div>
          </div>
        </div>

        <div class="summary">
          <strong>總計：</strong>
          NT$ {{ cart.items.reduce((s,i) => s + i.product.price * i.quantity, 0) }}
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.cart-item {
  display:flex;
  justify-content:space-between;
  gap:1rem;
  padding:0.8rem;
  border-radius:8px;
  background:#fff;
  box-shadow:0 1px 4px rgba(0,0,0,0.04);
  margin-bottom:0.8rem;
}
.cart-item .left { flex:1 }
.cart-item .right { display:flex; flex-direction:column; align-items:flex-end; gap:0.5rem }
button { cursor:pointer }
.summary { margin-top:1rem; font-size:1.1rem }
</style>
