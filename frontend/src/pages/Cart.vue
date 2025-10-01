<script setup>
import { computed, onMounted } from 'vue'
import { useCartStore } from '../stores/cart'

// 測試用
const userId = 1

const cartStore = useCartStore()

// 頁面載入時從 store 讀取 cart（store 會處理 404）
onMounted(() => {
  if (!cartStore.cart) {
    cartStore.fetchCart(userId)
  }
})

const cart = computed(() => cartStore.cart)
const loading = computed(() => cartStore.loading)
const error = computed(() => cartStore.error)

// 更新數量（呼叫 store）
async function updateQuantity(item, newQty) {
  if (newQty < 1) return
  try {
    await cartStore.updateItem(userId, item.id, newQty)
    // store 會在成功後更新本地 cart，所以畫面會自動反映
  } catch (err) {
    console.error('updateQuantity error', err)
    alert('更新數量失敗：' + (err?.response?.data?.message || err?.message || ''))
  }
}

// 移除項目（呼叫 store）
async function removeItem(item) {
  if (!confirm('確定要移除嗎？')) return
  try {
    await cartStore.removeItem(userId, item.id)
    // store 已移除本地 item
  } catch (err) {
    console.error('removeItem error', err)
    alert('移除失敗：' + (err?.response?.data?.message || err?.message || ''))
  }
}

// 計算總計（保護性存取）
const total = computed(() => {
  const c = cart.value
  if (!c || !c.items) return 0
  return c.items.reduce((s, i) => s + ((i.product?.price || 0) * (i.quantity || 0)), 0)
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
            <div class="title">{{ item.product?.name }}</div>
            <div class="desc">{{ item.product?.description }}</div>
            <div>單價：NT$ {{ item.product?.price }}</div>
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
          NT$ {{ total }}
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
