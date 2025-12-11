<template>
  <div class="osdr-card">
    <div class="table-header">
      <h3>NASA OSDR</h3>
      <div class="controls">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Поиск по ключевым словам"
        />

        <label>
          Сортировать по:
          <select v-model="sortColumn">
            <option value="id">ID</option>
            <option value="datasetId">Dataset ID</option>
            <option value="title">Title</option>
            <option value="updatedAt">Updated At</option>
            <option value="insertedAt">Inserted At</option>
          </select>
        </label>

        <label>
          Направление:
          <select v-model="sortOrder">
            <option value="asc">По возрастанию</option>
            <option value="desc">По убыванию</option>
          </select>
        </label>

        <label>
          На странице:
          <input type="number" v-model.number="perPage" min="1" max="100" />
        </label>

        <button @click="applyFilters">Применить</button>
      </div>
    </div>

    <div class="table-container">
      <table class="table table-sm table-striped align-middle">
        <thead>
          <tr>
            <th>#</th>
            <th>Dataset ID</th>
            <th>Title</th>
            <th>REST URL</th>
            <th>Updated At</th>
            <th>Inserted At</th>
            <th>Raw</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in paginatedData" :key="row.id">
            <td>{{ row.id }}</td>
            <td>{{ row.datasetId ?? '—' }}</td>
            <td class="ellipsis">{{ row.title ?? '—' }}</td>
            <td>
              <a
                v-if="row.raw?.REST_URL"
                :href="row.raw.REST_URL"
                target="_blank"
                rel="noopener"
              >
                открыть
              </a>
              <span v-else>—</span>
            </td>
            <td>{{ formatDate(row.updatedAt) }}</td>
            <td>{{ formatDate(row.insertedAt) }}</td>
            <td>
              <button class="btn btn-outline-secondary btn-sm" @click="toggleRaw(row.id)">
                JSON
              </button>
            </td>
          </tr>

          <tr
            v-for="row in paginatedData"
            :key="'raw-'+row.id"
            v-show="showRaw[row.id]"
          >
            <td colspan="7">
              <pre>{{ JSON.stringify(row.raw, null, 2) }}</pre>
            </td>
          </tr>

          <tr v-if="filteredData.length === 0">
            <td colspan="7" class="text-center text-muted">нет данных</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="pagination">
      <button @click="prevPage" :disabled="page <= 1">« Назад</button>
      <span>Страница {{ page }} / {{ totalPages }}</span>
      <button @click="nextPage" :disabled="page >= totalPages">Вперёд »</button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';

const items = ref([]);
const searchQuery = ref('');
const sortColumn = ref('id');
const sortOrder = ref('asc');
const perPage = ref(20);
const page = ref(1);
const showRaw = ref({});

const loadData = async () => {
  try {
    const res = await fetch('/odsr/all');
    if (!res.ok) throw new Error('Сервер вернул ' + res.status);
    const data = await res.json();
    items.value = data.map(i => ({
      ...i,
      raw: typeof i.raw === 'string' ? JSON.parse(i.raw) : i.raw
    }));
  } catch (e) {
    console.error('Ошибка загрузки OSDR:', e);
  }
};

const filteredData = computed(() => {
  let data = [...items.value];
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase();
    data = data.filter(
      i => (i.datasetId?.toLowerCase().includes(q)) || (i.title?.toLowerCase().includes(q))
    );
  }
  data.sort((a, b) => {
    let valA = a[sortColumn.value];
    let valB = b[sortColumn.value];
    if (sortColumn.value.includes('At')) {
      valA = new Date(valA);
      valB = new Date(valB);
    }
    if (valA < valB) return sortOrder.value === 'asc' ? -1 : 1;
    if (valA > valB) return sortOrder.value === 'asc' ? 1 : -1;
    return 0;
  });
  return data;
});

const totalPages = computed(() => Math.ceil(filteredData.value.length / perPage.value));
const paginatedData = computed(() =>
  filteredData.value.slice((page.value - 1) * perPage.value, page.value * perPage.value)
);

const applyFilters = () => { page.value = 1; };
const nextPage = () => { if (page.value < totalPages.value) page.value++; };
const prevPage = () => { if (page.value > 1) page.value--; };
const toggleRaw = id => { showRaw.value[id] = !showRaw.value[id]; };
const formatDate = d => (d ? new Date(d).toLocaleString() : '—');

onMounted(loadData);
</script>

<style>
.osdr-card {
  border: 2px solid #444;
  border-radius: 8px;
  padding: 15px;
  background-color: #f9f9f9;
  margin-bottom: 20px;
  width: 100%;
  box-sizing: border-box;
}

.table-header {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.controls {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.controls input,
.controls select,
.controls button {
  padding: 5px 8px;
  border-radius: 4px;
  border: 1px solid #888;
}

.table-container {
  width: 100%;
  overflow-x: auto;
  margin-top: 0; 
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.ellipsis {
  max-width: 420px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.pagination {
  display: flex;
  align-items: center;
  gap: 10px;
  justify-content: center;
  margin-top: 10px;
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
</style>
