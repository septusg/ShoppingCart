<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()

const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref(null)

async function submit() {
  loading.value = true
  error.value = null
  try {
    await auth.login(email.value, password.value)
    // 登入成功，導回商品頁（或你想去的頁面）
    router.push({ name: 'Products' })
  } catch (err) {
    // 若後端回 { message: "..." } 或直接字串，這裡都嘗試取出訊息
    const msg = err?.response?.data?.message || err?.response?.data || err?.message || '登入失敗'
    error.value = typeof msg === 'string' ? msg : JSON.stringify(msg)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div style="max-width:420px;margin:2rem auto">
    <h2>登入</h2>

    <div v-if="error" style="color:crimson;margin-bottom:0.6rem">{{ error }}</div>

    <div>
      <label>電子信箱</label><br />
      <input v-model="email" type="email" placeholder="email" />
    </div>

    <div style="margin-top:0.6rem">
      <label>密碼</label><br />
      <input v-model="password" type="password" placeholder="password" />
    </div>

    <div style="margin-top:1rem">
      <button @click="submit" :disabled="loading">{{ loading ? '登入中...' : '登入' }}</button>
    </div>
  </div>
</template>

<style scoped>
input { width:100%; padding:0.4rem; border:1px solid #ddd; border-radius:4px }
button { padding:0.5rem 0.8rem; border-radius:6px; background:#42b883; color:white; border:none; cursor:pointer }
button:disabled { opacity:0.7; cursor:default }
</style>
