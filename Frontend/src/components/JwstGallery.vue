<template>
  <div class="jwst-card">
    <h3>JWST — последние изображения</h3>

    <form @submit.prevent="applyFilter" class="jwst-form">
      <label>
        Фильтр по:
        <select v-model="filter.source" @change="toggleInputs">
          <option value="jpg">JPG</option>
          <option value="suffix">Suffix</option>
          <option value="program">Program</option>
        </select>
      </label>

      <input v-if="filter.source === 'suffix'" v-model="filter.suffix" placeholder="Suffix" />
      <input v-if="filter.source === 'program'" v-model="filter.program" placeholder="Program" type="number" />

      <label>
        Инструмент:
        <select v-model="filter.instrument">
          <option value="">Все</option>
          <option value="NIRCam">NIRCam</option>
          <option value="NIRSpec">NIRSpec</option>
          <option value="MIRI">MIRI</option>
          <option value="FGS/NIRISS">FGS/NIRISS</option>
        </select>
      </label>

      <label>
        Изображений на странице:
        <input type="number" v-model.number="filter.perPage" min="1" max="100" />
      </label>

      <button type="submit">Применить</button>
    </form>

    <div ref="galleryContainer" class="gallery">
      <div v-if="loading" class="loading">Загрузка…</div>
      <figure v-for="item in images" :key="item.id" class="gallery-item">
        <a :href="item.location" target="_blank" rel="noreferrer">
          <img :src="item.thumbnail || item.location" :alt="item.id" loading="lazy"/>
        </a>
        <figcaption class="caption">{{ item.id }}</figcaption>
      </figure>
    </div>

    <div class="pagination">
      <button @click="prevPage" :disabled="filter.page <= 1">« Назад</button>
      <span>Страница {{ filter.page }}</span>
      <button @click="nextPage">Вперёд »</button>
    </div>

    <div class="info">
      Источник: {{ filter.source }} · Показано {{ images.length }}
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';

const images = ref([]);
const galleryContainer = ref(null);
const loading = ref(true);

const filter = ref({
  source: 'jpg',
  suffix: '',
  program: '',
  instrument: '',
  page: 1,
  perPage: 24
});

const toggleInputs = () => {};

const loadImages = async () => {
  loading.value = true;
  try {
    const qs = {
      source: filter.value.source,
      page: filter.value.page,
      perPage: filter.value.perPage
    };
    if (filter.value.source === 'suffix' && filter.value.suffix) qs.suffix = filter.value.suffix;
    if (filter.value.source === 'program' && filter.value.program) qs.program = filter.value.program;
    if (filter.value.instrument) qs.instrument = filter.value.instrument;

    const url = '/jwst/feed?' + new URLSearchParams(qs).toString();
    const res = await fetch(url);
    const data = await res.json();

    if (data?.body) {
      images.value = data.body
        .filter(item => item.location)
        .map(item => ({
          id: item.id,
          location: item.location,
          thumbnail: item.thumbnail || '' 
        }));
    } else {
      images.value = [];
    }
  } catch(e) {
    console.error('Ошибка загрузки JWST:', e);
    images.value = [];
  } finally {
    loading.value = false;
  }
};

const applyFilter = () => {
  filter.value.page = 1;
  loadImages();
};

const nextPage = () => {
  filter.value.page++;
  loadImages();
};
const prevPage = () => {
  if (filter.value.page > 1) {
    filter.value.page--;
    loadImages();
  }
};

onMounted(() => {
  loadImages();
});
</script>

<style>
.jwst-card {
  border: 2px solid #444;
  border-radius: 8px;
  padding: 15px;
  background-color: #f9f9f9;
  margin-bottom: 20px;
}

.jwst-card h3 {
  margin-bottom: 10px;
  color: #333;
}

.jwst-form {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 15px;
}
.jwst-form input,
.jwst-form select,
.jwst-form button {
  padding: 5px 8px;
  border: 1px solid #888;
  border-radius: 4px;
}

.gallery {
  display: flex;
  flex-wrap: wrap;
  justify-content: center; 
  gap: 10px;
  overflow-x: hidden;
}
.gallery-item {
  width: 200px;
  border: 1px solid #888;
  border-radius: 6px;
  overflow: hidden;
  background-color: #fff;
  text-align: center;
}
.gallery-item img {
  width: 100%;
  height: auto; 
  display: block;
}
.caption {
  padding: 5px;
  font-size: 12px;
  color: #222;
}

.pagination {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-top: 10px;
  justify-content: center;
}
.pagination button {
  padding: 5px 10px;
  border-radius: 4px;
  border: 1px solid #888;
  background-color: #eee;
  cursor: pointer;
}
.pagination button:hover {
  background-color: #ddd;
}
.pagination button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.info {
  margin-top: 10px;
  font-size: 12px;
  color: #555;
  text-align: center;
}
.loading {
  padding: 10px;
  color: #666;
  text-align: center;
}
</style>

