<template>
  <div class="iss-map-card">
    <h3>МКС — положение и движение</h3>
    <div ref="mapContainer" class="map"></div>
    <div class="charts">
      <div class="chart-wrapper">
        <canvas ref="speedChart"></canvas>
      </div>
      <div class="chart-wrapper">
        <canvas ref="altChart"></canvas>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import L from "leaflet";
import Chart from "chart.js/auto";

import "leaflet/dist/leaflet.css";

L.Icon.Default.mergeOptions({
  iconUrl: "/leaflet/marker-icon.png",
  iconRetinaUrl: "/leaflet/marker-icon-2x.png",
  shadowUrl: "/leaflet/marker-shadow.png"
});

const mapContainer = ref(null);
const speedChart = ref(null);
const altChart = ref(null);

const speedHistory = [];
const altHistory = [];
const timeLabels = [];

const MAX_POINTS = 20;

const fetchLast = async () => {
  try {
    const res = await fetch("/iss/last");
    const data = await res.json();
    return JSON.parse(data.payload || "{}");
  } catch (e) {
    console.error("Ошибка загрузки ISS last:", e);
    return {};
  }
};

onMounted(async () => {
  const map = L.map(mapContainer.value, {
    attributionControl: false,
  }).setView([0, 0], 2);

  L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png").addTo(map);

  const marker = L.marker([0, 0]).addTo(map).bindPopup("МКС");

  const speed = new Chart(speedChart.value, {
    type: "line",
    data: {
      labels: [],
      datasets: [
        {
          label: "Скорость (км/ч)",
          data: [],
          borderColor: "#3498db",
          fill: false,
        },
      ],
    },
  });

  const alt = new Chart(altChart.value, {
    type: "line",
    data: {
      labels: [],
      datasets: [
        {
          label: "Высота (км)",
          data: [],
          borderColor: "#2ecc71",
          fill: false,
        },
      ],
    },
  });

  const update = async () => {
    const data = await fetchLast();
    if (!data.latitude || !data.longitude) return;

    const latlng = [data.latitude, data.longitude];
    marker.setLatLng(latlng);
    map.setView(latlng, map.getZoom());

    const time = new Date(data.timestamp * 1000).toLocaleTimeString();

    timeLabels.push(time);
    speedHistory.push(data.velocity);
    altHistory.push(data.altitude);

    if (timeLabels.length > MAX_POINTS) {
      timeLabels.shift();
      speedHistory.shift();
      altHistory.shift();
    }

    speed.data.labels = [...timeLabels];
    speed.data.datasets[0].data = [...speedHistory];
    speed.update();

    alt.data.labels = [...timeLabels];
    alt.data.datasets[0].data = [...altHistory];
    alt.update();
  };

  await update();
  setInterval(update, 60000);
});
</script>

<style>
.iss-map-card {
  border: 2px solid #444;
  border-radius: 8px;
  padding: 15px;
  background-color: #f9f9f9;
}
.iss-map-card h3 { margin-bottom: 10px; color: #333; font-size: 18px; }
.map { height: 300px; border: 1px solid #888; border-radius: 6px; margin-bottom: 15px; }
.charts { display: flex; gap: 10px; }
.chart-wrapper { flex: 1; height: 150px; border: 1px solid #888; border-radius: 6px; background-color: #fff; padding: 5px; }
</style>
