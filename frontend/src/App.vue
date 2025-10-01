<script setup>
import { computed, watch, onMounted } from 'vue'
import { useCartStore } from './stores/cart'
import { useAuthStore } from './stores/auth'
import { useRouter } from 'vue-router'

const cartStore = useCartStore()
const auth = useAuthStore()
const router = useRouter()

// 若開發階段沒有登入，仍保留 fallback userId = 1
const fallbackUserId = 1

// 頁面載入時：若已經有 user（localStorage），用該 userId fetch cart；否則用 fallback
onMounted(() => {
  const uid = auth.user?.id ?? fallbackUserId
  if (!cartStore.cart) cartStore.fetchCart(uid)
})

// 監聽登入狀態變化：登入後用真實 userId 去 fetch cart；登出則用 fallback (可改為顯示空購物車)
watch(() => auth.user, (u) => {
  const uid = u?.id ?? fallbackUserId
  cartStore.fetchCart(uid)
})

const count = computed(() => cartStore.itemCount())
const isLogged = computed(() => !!auth.user)
</script>

<template>
  <div id="app">
    <header style="display:flex;align-items:center;gap:1rem;padding:1rem;border-bottom:1px solid #eee">
      <h1 style="margin:0">ShoppingCart Demo</h1>
      <nav style="margin-left:auto">
        <router-link to="/">商品</router-link>
        |
        <router-link to="/cart">購物車 ({{ count }})</router-link>
        |
        <span v-if="!isLogged">
          <router-link to="/login">登入</router-link> |
          <router-link to="/register">註冊</router-link>
        </span>
        <span v-else>
          你好！{{ auth.user.userName }}！ |
          <a href="#" @click.prevent="auth.logout(); router.push({ name: 'Products' })">登出</a>
        </span>
      </nav>
    </header>

    <main style="padding:1rem;">
      <router-view />
    </main>
  </div>
</template>

<style>

</style>
