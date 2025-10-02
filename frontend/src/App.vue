<script setup>
import { computed, watch, onMounted } from 'vue'
import { useCartStore } from './stores/cart'
import { useAuthStore } from './stores/auth'
import { useRouter } from 'vue-router'

const cartStore = useCartStore()
const auth = useAuthStore()
const router = useRouter()

const fallbackUserId = 1

onMounted(() => {
  const uid = auth.user?.id ?? fallbackUserId
  if (!cartStore.cart) cartStore.fetchCart(uid)
})

watch(() => auth.user, (u) => {
  const uid = u?.id ?? fallbackUserId
  cartStore.fetchCart(uid)
})

const count = computed(() => cartStore.itemCount())
const isLogged = computed(() => !!auth.user)
</script>

<template>
  <div id="app" class="min-h-screen">
    <header class="bg-white/60 dark:bg-slate-800/60 backdrop-blur-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center gap-4 py-4">
          <h1 class="text-2xl font-bold m-0">ShoppingCart Demo</h1>

          <nav class="ml-auto flex items-center gap-3 text-sm">
            <router-link to="/" class="text-gray-700 dark:text-gray-200 hover:text-indigo-600">商品</router-link>
            <span class="text-gray-400">|</span>
            <router-link to="/cart" class="text-gray-700 dark:text-gray-200 hover:text-indigo-600">
              購物車 <span class="ml-1 inline-block bg-indigo-100 text-indigo-800 px-2 py-0.5 rounded-full text-xs">{{ count }}</span>
            </router-link>
            <span class="text-gray-400">|</span>

            <template v-if="!isLogged">
              <router-link to="/login" class="text-gray-700 dark:text-gray-200 hover:text-indigo-600">登入</router-link>
              <span class="text-gray-400">|</span>
              <router-link to="/register" class="text-gray-700 dark:text-gray-200 hover:text-indigo-600">註冊</router-link>
            </template>

            <template v-else>
              <div class="text-gray-700 dark:text-gray-200">你好！<span class="font-semibold">{{ auth.user.userName }}</span>！</div>
              <span class="text-gray-400">|</span>
              <button @click.prevent="auth.logout(); router.push({ name: 'Products' })"
                      class="ml-2 px-3 py-1 bg-red-500 text-white rounded hover:bg-red-600">
                登出
              </button>
            </template>
          </nav>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <router-view />
    </main>
  </div>
</template>
