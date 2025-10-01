<script setup>
import { computed } from 'vue'
import { useCartStore } from './stores/cart'

const cartStore = useCartStore()

// 測試用（之後改成登入拿到的 id）
const userId = 1

// 在載入 App 時抓一次購物車
if (!cartStore.cart) {
  cartStore.fetchCart(userId)
}

const count = computed(() => cartStore.itemCount())
</script>

<template>
  <div id="app">
    <header style="display:flex;align-items:center;gap:1rem;padding:1rem;border-bottom:1px solid #eee">
      <h1 style="margin:0">ShoppingCart Demo</h1>
      <nav style="margin-left:auto">
        <router-link to="/">商品</router-link>
        |
        <router-link to="/cart">購物車 ({{ count }})</router-link>
      </nav>
    </header>

    <main style="padding:1rem;">
      <router-view />
    </main>
  </div>
</template>
