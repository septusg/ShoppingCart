<template>
  <div id="app">
    <header style="display:flex;align-items:center;gap:1rem;padding:1rem;border-bottom:1px solid #eee">
      <h1 style="margin:0">ShoppingCart Demo</h1>
      <nav style="margin-left:auto">
        <router-link to="/">商品</router-link>
        |
        <router-link to="/cart">購物車</router-link>
      </nav>
    </header>

    <main style="padding:1rem;">
      <router-view />
    </main>
  </div>
</template>

<script setup>

</script>

<style>

</style>

