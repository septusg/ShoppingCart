<!-- src/components/HelloWorld.vue -->
<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const loading = ref(false)
const error = ref(null)
const products = ref([])

async function fetchProducts() {
  loading.value = true
  error.value = null
  try {
    // 呼叫 ProductController 的 GET 所有商品
    const res = await api.get('/Product')
    products.value = res.data
  } catch (err) {
    error.value = err?.response?.data || err.message || 'Unknown error'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchProducts()
})
</script>

<template>
  <div class="product-list">
    <h2>商品清單</h2>

    <div v-if="loading">載入中，請稍等…</div>

    <div v-else-if="error" class="error">
      取得資料失敗：{{ error }}
    </div>

    <div v-else>
      <div v-if="products.length === 0">目前沒有商品</div>

      <div class="cards">
        <div class="card" v-for="product in products" :key="product.id">
          <h3>{{ product.name }}</h3>
          <p>{{ product.description }}</p>
          <p><strong>價格：</strong>NT$ {{ product.price }}</p>
          <p><strong>庫存：</strong> {{ product.stockQuantity }}</p>
          <p><strong>類別：</strong> {{ product.category }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.product-list {
  padding: 1em;
}

.error {
  color: crimson;
}

.cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 1em;
  margin-top: 1em;
}

.card {
  padding: 1em;
  border-radius: 8px;
  background: #f7f7f7;
  box-shadow: 0 2px 6px rgba(0,0,0,0.1);
  transition: transform 0.2s, box-shadow 0.2s;
}

.card:hover {
  transform: translateY(-4px);
  box-shadow: 0 6px 12px rgba(0,0,0,0.15);
}
</style>
