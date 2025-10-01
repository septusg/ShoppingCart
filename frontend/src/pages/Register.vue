<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()

const userName = ref('')
const email = ref('')
const password = ref('')
const passwordConfirm = ref('')
const loading = ref(false)
const error = ref(null)

// toast 控制
const successMsg = ref('')
const showToast = ref(false)
let toastTimer = null

async function submit() {
  error.value = null

  if (!userName.value || !email.value || !password.value) {
    error.value = '請填寫所有欄位'
    return
  }
  if (password.value !== passwordConfirm.value) {
    error.value = '密碼與確認密碼不相符'
    return
  }

  loading.value = true
  try {
    // 使用 /User/register（後端 DTO: { UserName, Email, Password }）
    const api = (await import('../services/api')).default
    const res = await api.post('/User/register', {
      UserName: userName.value,
      Email: email.value,
      Password: password.value
    })
    const data = res.data
    // 回傳格式預期: { id, userName, email, token, expiresAt }
    auth.setToken(data.token)
    auth.setUser({ id: data.id, userName: data.userName, email: data.email })
    api.defaults.headers.common['Authorization'] = `Bearer ${data.token}`

    // 顯示成功 toast（3秒後跳轉）
    successMsg.value = '註冊成功！系統將自動登入，3秒後跳轉…'
    showToast.value = true
    if (toastTimer) clearTimeout(toastTimer)
    toastTimer = setTimeout(() => {
      showToast.value = false
      router.push({ name: 'Products' })
    }, 3000)

  } catch (err) {
    const msg = err?.response?.data?.message || err?.response?.data || err?.message || '註冊失敗'
    error.value = typeof msg === 'string' ? msg : JSON.stringify(msg)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div style="max-width:480px;margin:2rem auto;position:relative">
    <h2>註冊</h2>

    <div v-if="error" style="color:crimson;margin-bottom:0.6rem">{{ error }}</div>

    <div>
      <label>名稱</label><br />
      <input v-model="userName" placeholder="顯示名稱" />
    </div>

    <div style="margin-top:0.6rem">
      <label>電子信箱</label><br />
      <input v-model="email" type="email" placeholder="email" />
    </div>

    <div style="margin-top:0.6rem">
      <label>密碼</label><br />
      <input v-model="password" type="password" placeholder="password" />
    </div>

    <div style="margin-top:0.6rem">
      <label>確認密碼</label><br />
      <input v-model="passwordConfirm" type="password" placeholder="confirm password" />
    </div>

    <div style="margin-top:1rem">
      <button @click="submit" :disabled="loading">{{ loading ? '註冊中...' : '註冊' }}</button>
    </div>

    <!-- Central Toast -->
    <div v-if="showToast" class="toast-center">
      <div class="toast-box">{{ successMsg }}</div>
    </div>
  </div>
</template>

<style scoped>
input { width:100%; padding:0.45rem; border:1px solid #ddd; border-radius:4px }
button { padding:0.5rem 0.8rem; border-radius:6px; background:#42b883; color:white; border:none; cursor:pointer }
button:disabled { opacity:0.7; cursor:default }

/* 中央浮層覆蓋層 */
.toast-center {
  position: fixed;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  pointer-events: auto;
}

/* 實際 toast box */
.toast-box {
  pointer-events: auto;
  background: #00b39b;
  color: white;
  padding: 14px 20px;
  border-radius: 10px;
  box-shadow: 0 10px 30px rgba(34,37,42,0.18);
  font-weight: 700;
  min-width: 220px;
  text-align: center;
  transform: translateY(8px);
  opacity: 0;
  animation: centerIn 260ms cubic-bezier(.2,.9,.2,1) forwards;
}

/* 進場動畫 */
@keyframes centerIn {
  to { transform: translateY(0); opacity: 1 }
}
</style>
