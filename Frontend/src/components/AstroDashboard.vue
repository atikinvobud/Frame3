<template>
  <div class="dashboard-container">

    <!-- Карта + ISS Info -->
    <div class="dashboard-main">

      <!-- Карта -->
      <div class="map-container">
        <IssMap :iss="iss" />
      </div>

      <!-- ISS Info -->
      <div class="iss-info">
        <h3>МКС — данные</h3>
        <div class="iss-cards">
          <div class="iss-card">
            <span class="title">Скорость МКС</span>
            <span class="value">{{ iss?.velocity ?? '—' }} км/ч</span>
          </div>
          <div class="iss-card">
            <span class="title">Высота МКС</span>
            <span class="value">{{ iss?.altitude ?? '—' }} км</span>
          </div>
          <div class="iss-card">
            <span class="title">Широта</span>
            <span class="value">{{ iss?.latitude ?? '—' }}</span>
          </div>
          <div class="iss-card">
            <span class="title">Долгота</span>
            <span class="value">{{ iss?.longitude ?? '—' }}</span>
          </div>
          <div class="iss-card">
            <span class="title">Видимость</span>
            <span class="value">{{ iss?.visibility ?? '—' }}</span>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import IssMap from './IssMap.vue';
import JwstGallery from './JwstGallery.vue';

const iss = ref({});

const loadIss = async () => {
  try {
    const res = await fetch('/iss/last');
    const data = await res.json();
    iss.value = JSON.parse(data.payload ?? '{}');
  } catch(e) {
    console.error('Ошибка загрузки ISS:', e);
  }
};

onMounted(() => {
  loadIss();
  setInterval(loadIss, 15000);
});
</script>

<style>
.dashboard-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.dashboard-main {
  display: flex;
  gap: 20px;
  margin-top: 20px;
}

/* Карта */
.map-container {
  flex: 2;
  min-height: 400px;
  border: 2px solid #444;
  border-radius: 8px;
  overflow: hidden;
}

/* ISS Info */
.iss-info {
  flex: 1;
}

.iss-info h3 {
  margin-bottom: 15px;
  color: #333;
}

.iss-cards {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.iss-card {
  padding: 12px;
  border: 1px solid #888;
  border-radius: 6px;
  background-color: #f5f5f5;
  display: flex;
  justify-content: space-between;
  font-weight: bold;
  color: #222;
}

.iss-card .title {
  font-size: 14px;
}

.iss-card .value {
  font-size: 16px;
  color: #0055aa;
}
</style>
