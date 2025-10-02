import axios from 'axios'

// 設定後端基礎網址
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7055/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 請求攔截器：每次 request 都從 localStorage 讀 token 並帶上 Authorization
api.interceptors.request.use(config => {
  try {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers = config.headers || {}
      config.headers['Authorization'] = `Bearer ${token}`
    }
  } catch (e) {
    // 忽略 localStorage 讀取錯誤
  }
  return config
}, error => Promise.reject(error))

// 回應攔截器：統一處理 401（可在此做自動登出或導向 login）
api.interceptors.response.use(
  res => res,
  err => {
    if (err?.response?.status === 401) {
      try {
        // 移除 token（讓前端狀態不一致時也能回復）
        localStorage.removeItem('token')
        localStorage.removeItem('user')
      } catch (e) {}
      // 通知 user 或導頁：window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)

export default api
