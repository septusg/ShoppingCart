import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '../services/api'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || null)
  const user = ref(localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')) : null)
  const loading = ref(false)
  const error = ref(null)

  function setToken(t) {
    token.value = t
    if (t) localStorage.setItem('token', t)
    else localStorage.removeItem('token')
  }
  function setUser(u) {
    user.value = u
    if (u) localStorage.setItem('user', JSON.stringify(u))
    else localStorage.removeItem('user')
  }

  async function login(email, password) {
    loading.value = true
    error.value = null
    try {
      // 後端現在期待 { Email, Password }
      const res = await api.post('/User/login', { Email: email, Password: password })
      const data = res.data
      // 後端回傳 { token, expiresAt, user: { id, userName, email } }
      setToken(data.token)
      setUser(data.user)
      // 保險：把 axios 預設 header 設好（攔截器也會自動處理）
      api.defaults.headers.common['Authorization'] = `Bearer ${data.token}`
      return data
    } catch (err) {
      error.value = err?.response?.data || err?.message || JSON.stringify(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  function logout() {
    setToken(null)
    setUser(null)
    delete api.defaults.headers.common['Authorization']
  }

  function isAuthenticated() {
    return !!token.value
  }

  return { token, user, loading, error, login, logout, setToken, setUser, isAuthenticated }
})
