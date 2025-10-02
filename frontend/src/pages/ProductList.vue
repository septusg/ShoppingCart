<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'  // api instance
import { useAuthStore } from '../stores/auth'
import { useCartStore } from '../stores/cart'

const auth = useAuthStore()
const userId = auth.user?.id || 1  // 若沒登入 fallback 或用匿名 cart 流程

const loading = ref(false)
const error = ref(null)
const products = ref([])

const cartStore = useCartStore()

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

async function addToCart(product) {
  try {
    const updatedCart = await cartStore.addToCart(userId, product.id, 1)
    console.log('加入購物車後端回傳 cart:', updatedCart)
    const qty = updatedCart.items.find(i => i.productId === product.id)?.quantity ?? '?'
    alert(`已加入購物車：${product.name}（數量已更新為 ${qty}）`)
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
    <div class="flex items-center justify-between mb-6">
      <h2 class="text-2xl font-bold">商品清單</h2>
    </div>

    <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
      <!-- loading skeleton -->
      <div v-for="i in 8" :key="i" class="animate-pulse bg-white rounded-lg p-4 h-56"></div>
    </div>

    <div v-else-if="error" class="text-red-600">{{ error }}</div>

    <div v-else>
      <div v-if="products.length === 0" class="text-gray-500">目前沒有商品</div>

      <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 mt-4">
        <div v-for="p in products" :key="p.id" class="bg-white dark:bg-slate-800 rounded-xl shadow-sm p-4 flex flex-col">
          <div class="mb-3">
            <img
              v-if="p.imageUrl"
              :src="p.imageUrl"
              :alt="p.name"
              class="w-full h-full object-cover rounded-md"
            />
            <div v-else class="w-full h-36 bg-gray-100 dark:bg-slate-700 rounded-md flex items-center justify-center text-gray-400">
              無圖片
            </div>
          </div>

          <div class="flex-1">
            <h3 class="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-1">{{ p.name }}</h3>
            <p class="text-sm text-gray-500 dark:text-gray-300 mb-3 line-clamp-2">{{ p.description }}</p>

            <div class="flex items-center justify-between mt-4">
              <div>
                <div class="text-indigo-600 font-bold text-lg">NT$ {{ p.price }}</div>
                <div class="text-xs text-gray-400">庫存：{{ p.stockQuantity }}</div>
              </div>
            </div>
          </div>

          <div class="mt-4">
            <button
              @click="addToCart(p)"
              :disabled="p.stockQuantity <= 0"
              class="w-full inline-flex items-center justify-center px-4 py-2 rounded-md text-white bg-indigo-600 hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              加入購物車
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

