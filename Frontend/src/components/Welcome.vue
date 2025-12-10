<template>
  <div class="home-page">
    <div class="home-card">

      <h1 class="title">Добро пожаловать 👋</h1>

      <p class="subtitle">
        Это главная страница панели мониторинга. Выберите один из разделов в меню,
        чтобы просмотреть данные из различных космических сервисов.
      </p>

      <div class="buttons">
        <router-link to="/jwst" class="nav-btn">JWST</router-link>
        <router-link to="/odsr" class="nav-btn">ODSR</router-link>
        <router-link to="/astro" class="nav-btn">Astronomy API</router-link>
        <router-link to="/dash" class="nav-btn">ISS</router-link>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "Home"
};
</script>

<style scoped>
.home-page {
  padding: 40px;
  display: flex;
  justify-content: center;
}

.home-card {
  width: 100%;
  max-width: 780px;
  background: #ffffff;
  border-radius: 14px;
  box-shadow: 0 4px 16px rgba(0,0,0,0.08);
  padding: 35px 40px;
  position: relative;
}

.title {
  font-size: 32px;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 10px;
}

.subtitle {
  font-size: 17px;
  color: #4b5563;
  margin-bottom: 25px;
  line-height: 1.5;
}

.buttons {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.nav-btn {
  padding: 10px 18px;
  background: #f3f4f6;
  border-radius: 8px;
  text-decoration: none;
  font-size: 15px;
  color: #374151;
  transition: 0.2s;
  border: 1px solid #e5e7eb;
}

.nav-btn:hover {
  background: #e5e7eb;
  transform: translateY(-2px);
}
</style>
