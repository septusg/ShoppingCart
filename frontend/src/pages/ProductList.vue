<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'  // api instance

const loading = ref(false)
const error = ref(null)
const products = ref([])

// 測試用的 userId（之後從登入拿）
const userId = 1

async function fetchProducts() {
  loading.value = true
  error.value = null
  try {
    const res = await api.get('/Product')
    products.value = res.data
  } catch (err) {
    error.value = err?.response?.data || err.message || 'Unknown error'
  } finally {
    loading.value = false
  }
}



import { useRouter } from 'vue-router'
const router = useRouter()

async function addToCart(product) {
  try {
    // 呼叫後端並拿到回傳的 cartDto（updatedCart）
    const res = await api.post(`/Cart/${userId}`, { productId: product.id, quantity: 1 })

    // res.data 就是後端回傳的 CartDto
    const updatedCart = res.data
    console.log('加入購物車後端回傳 cart:', updatedCart)

    alert(`已加入購物車：${product.name}（數量已更新為 ${updatedCart.items.find(i => i.productId === product.id)?.quantity ?? '?' }）`)

    // 寫這行會自動跳轉到購物車頁面
    // router.push({ name: 'Cart' })

  } catch (err) {
    console.error('addToCart error', err)
    const status = err?.response?.status
    const data = err?.response?.data
    const message = data?.message || JSON.stringify(data) || err.message
    alert(`加入購物車失敗（HTTP ${status}）：${message}`)
  }
}



onMounted(() => {
  fetchProducts()
})
</script>

<template>
  <div>
    <h2>商品清單</h2>
    <div v-if="loading">載入中…</div>
    <div v-else-if="error" style="color:crimson">{{ error }}</div>
    <div v-else>
      <div v-if="products.length === 0">目前沒有商品</div>
      <div class="cards">
        <div class="card" v-for="p in products" :key="p.id">
          <img v-if="p.imageUrl" :src="p.imageUrl" alt="" style="width:100%;height:140px;object-fit:cover;border-radius:6px" />
          <h3>{{ p.name }}</h3>
          <p>{{ p.description }}</p>
          <p><strong>NT$ {{ p.price }}</strong></p>
          <p>庫存：{{ p.stockQuantity }}</p>
          <button @click="addToCart(p)">加入購物車</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px,1fr));
  gap: 1rem;
}
.card {
  padding: 0.8rem;
  border-radius: 8px;
  background: #fafafa;
  box-shadow: 0 2px 6px rgba(0,0,0,0.04);
}
button {
  margin-top: 0.6rem;
  padding: 0.5rem 0.8rem;
  border-radius: 6px;
  border: none;
  background: #42b883;
  color: white;
  cursor: pointer;
}
button:hover { filter:brightness(0.95) }
</style>
