import { createRouter, createWebHistory } from 'vue-router'
import ProductList from '../pages/ProductList.vue'
import Cart from '../pages/Cart.vue'
import Login from '../pages/Login.vue'

const routes = [
  { path: '/', name: 'Products', component: ProductList },
  { path: '/cart', name: 'Cart', component: Cart },
  { path: '/login', name: 'Login', component: Login }
]


const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
