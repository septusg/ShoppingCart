<script setup>
import { computed, onMounted } from 'vue'
import { useCartStore } from '../stores/cart'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const userId = auth.user?.id || 1  

const cartStore = useCartStore()

onMounted(() => {
  if (!cartStore.cart) {
    cartStore.fetchCart(userId)
  }
})

const cart = computed(() => cartStore.cart)
const loading = computed(() => cartStore.loading)
const error = computed(() => cartStore.error)

async function updateQuantity(item, newQty) {
  if (newQty < 1) return
  try {
    await cartStore.updateItem(userId, item.id, newQty)
  } catch (err) {
    console.error('updateQuantity error', err)
    alert('更新數量失敗：' + (err?.response?.data?.message || err?.message || ''))
  }
}

async function removeItem(item) {
  if (!confirm('確定要移除嗎？')) return
  try {
    await cartStore.removeItem(userId, item.id)
  } catch (err) {
    console.error('removeItem error', err)
    alert('移除失敗：' + (err?.response?.data?.message || err?.message || ''))
  }
}

const total = computed(() => {
  const c = cart.value
  if (!c || !c.items) return 0
  return c.items.reduce((s, i) => s + ((i.product?.price || 0) * (i.quantity || 0)), 0)
})
</script>

<template>
  <div>
    <h2 class="text-2xl font-bold mb-4">我的購物車</h2>

    <div v-if="loading" class="space-y-3">
      <div class="animate-pulse bg-white p-4 rounded-md h-20"></div>
      <div class="animate-pulse bg-white p-4 rounded-md h-20"></div>
    </div>

    <div v-else-if="error" class="text-red-600">{{ error }}</div>

    <div v-else>
      <div v-if="!cart || cart.items.length === 0" class="text-gray-500">購物車空空如也</div>

      <div v-else class="space-y-4">
        <div v-for="item in cart.items" :key="item.id" class="bg-white dark:bg-slate-800 rounded-lg p-4 flex flex-col sm:flex-row items-start sm:items-center gap-4 shadow-sm">
          <div class="flex items-center gap-4 w-full">
            <div class="w-20 h-20 bg-gray-100 dark:bg-slate-700 rounded-md overflow-hidden flex-shrink-0">
              <img v-if="item.product?.imageUrl" :src="item.product.imageUrl" alt="" class="w-full h-full object-cover"/>
              <div v-else class="w-full h-full flex items-center justify-center text-gray-400">無圖</div>
            </div>

            <div class="flex-1">
              <div class="text-lg font-semibold text-gray-800 dark:text-gray-100">{{ item.product?.name }}</div>
              <div class="text-sm text-gray-500 dark:text-gray-300">{{ item.product?.description }}</div>
              <div class="text-sm text-gray-700 dark:text-gray-200 mt-1">單價：NT$ {{ item.product?.price }}</div>
            </div>
          </div>

          <div class="flex flex-col items-end gap-3">
            <div class="flex items-center gap-2">
              <button @click="updateQuantity(item, item.quantity - 1)" class="px-3 py-1 rounded-md border bg-gray-100 hover:bg-gray-200">-</button>
              <div class="px-3 py-1 border rounded-md min-w-[40px] text-center">{{ item.quantity }}</div>
              <button @click="updateQuantity(item, item.quantity + 1)" class="px-3 py-1 rounded-md border bg-gray-100 hover:bg-gray-200">+</button>
            </div>

            <div class="flex items-center gap-2">
              <div class="text-sm text-gray-600">小計：NT$ {{ (item.product?.price || 0) * (item.quantity || 0) }}</div>
              <button @click="removeItem(item)" class="ml-2 px-3 py-1 rounded-md bg-red-500 text-white hover:bg-red-600">移除</button>
            </div>
          </div>
        </div>

        <div class="mt-4 p-4 bg-gray-50 dark:bg-slate-900 rounded-lg flex justify-between items-center">
          <div class="text-lg font-semibold">總計</div>
          <div class="text-xl font-bold">NT$ {{ total }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

