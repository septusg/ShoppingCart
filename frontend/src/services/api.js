// src/services/api.js
import axios from 'axios'

// 設定後端基礎網址
const API_BASE_URL = 'https://localhost:7055/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000, // 10 秒逾時
  headers: {
    'Content-Type': 'application/json'
  }
})

// 攔截器：每次請求前可以加一個 Authorization
api.interceptors.request.use(config => {
  // const token = localStorage.getItem('token')
  // if (token) config.headers.Authorization = `Bearer ${token}`
  return config
}, error => Promise.reject(error))

// 回應攔截器（統一處理錯誤）
api.interceptors.response.use(response => response, error => {
  // 做全域錯誤處理或 logging
  return Promise.reject(error)
})

export default api
