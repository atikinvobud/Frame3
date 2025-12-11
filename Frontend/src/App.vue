<template>
  <div id="app">
    <nav class="navbar-card">
      <ul class="navbar-list">
        <li>
          <router-link to="/dash" class="nav-link" active-class="active">Dashboard</router-link>
        </li>
        <li>
          <router-link to="/jwst" class="nav-link" active-class="active">JWST</router-link>
        </li>
        <li>
          <router-link to="/odsr" class="nav-link" active-class="active">OSDR</router-link>
        </li>
        <li>
          <router-link to="/astro" class="nav-link" active-class="active">AstroEvents</router-link>
        </li>
      </ul>
    </nav>

    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<script setup>

</script>

<style>
body, html, #app {
  margin: 0;
  padding: 0;
  font-family: Arial, sans-serif;
  background-color: #f0f0f0;
}

.navbar-card {
  border: 2px solid #444;
  border-radius: 8px;
  background-color: #f9f9f9;
  padding: 10px 20px;
  margin: 10px 20px;
  box-shadow: 2px 2px 6px rgba(0,0,0,0.1);
}

.navbar-list {
  list-style: none;
  display: flex;
  gap: 20px;
  margin: 0;
  padding: 0;
}



.nav-link {
  color: #333;
  text-decoration: none;
  font-weight: bold;
  padding: 5px 10px;
  border-radius: 4px;
  transition: background 0.2s, color 0.2s;
}

.nav-link:hover {
  background-color: #444;
  color: #fff;
}

.active {
  background-color: #4caf50;
  color: #fff;
}

.main-content {
  padding: 20px;
}
</style>
