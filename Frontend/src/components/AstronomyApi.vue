<template>
  <div class="astro-card">
    <h3>Астрономические события (AstronomyAPI)</h3>

    <!-- Форма фильтров -->
    <form @submit.prevent="applyFilters" class="astro-form">
      <input type="number" step="0.0001" v-model.number="lat" placeholder="lat" />
      <input type="number" step="0.0001" v-model.number="lon" placeholder="lon" />
      <input type="number" min="1" max="365" v-model.number="days" placeholder="дней" />
      <button type="submit">Показать</button>
    </form>

    <!-- Таблица -->
    <div class="table-container">
      <table class="table table-sm align-middle">
        <thead>
          <tr>
            <th>#</th>
            <th>Тело</th>
            <th>Событие</th>
            <th>Когда (UTC)</th>
            <th>Дополнительно</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="loading">
            <td colspan="5" class="text-muted">Загрузка…</td>
          </tr>
          <tr v-else-if="error">
            <td colspan="5" class="text-danger">{{ error }}</td>
          </tr>
          <tr v-else-if="events.length === 0">
            <td colspan="5" class="text-muted">события не найдены</td>
          </tr>
          <tr v-else v-for="(row, i) in events" :key="i">
            <td>{{ i + 1 }}</td>
            <td>{{ row.name || '—' }}</td>
            <td>{{ row.type || '—' }}</td>
            <td><code>{{ row.when || '—' }}</code></td>
            <td>{{ row.extra || '' }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Полный JSON -->
    <details v-if="rawData">
      <summary>Полный JSON</summary>
      <pre class="bg-light rounded p-2 small">{{ JSON.stringify(rawData, null, 2) }}</pre>
    </details>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const lat = ref(55.7558);
const lon = ref(37.6176);
const days = ref(7);

const events = ref([]);
const rawData = ref(null);
const loading = ref(false);
const error = ref('');

// Нормализация одной ячейки события
function normalizeCell(entryName, cell) {
  const type = cell.type || '';
  const when = cell.eventHighlights?.peak?.date || cell.rise || cell.set || '';
  const extra = cell.eventHighlights ? JSON.stringify(cell.eventHighlights) : JSON.stringify(cell.extraInfo || {});
  return { name: entryName, type, when, extra };
}

// Сбор всех событий из API
function parseEvents(apiData) {
  const rows = [];
  if (!Array.isArray(apiData)) return rows;

  for (const obj of apiData) {
    const table = obj?.data?.data?.table;
    if (!table || !table.rows) continue;

    for (const row of table.rows) {
      const entryName = row.entry?.name || row.entry?.id || obj.body || '—';
      const cells = row.cells || [];

      if (cells.length === 0) {
        rows.push({ name: entryName, type: '—', when: '—', extra: '' });
      } else {
        for (const cell of cells) {
          let when = '';
          let extra = '';

          if (cell.eventHighlights?.peak) {
            when = cell.eventHighlights.peak.date;
            const ph = cell.eventHighlights;
            extra = `partialStart: ${ph.partialStart?.date || '—'} (alt ${ph.partialStart?.altitude ?? '—'}), peak: ${ph.peak.date} (alt ${ph.peak.altitude ?? '—'}), partialEnd: ${ph.partialEnd?.date || '—'} (alt ${ph.partialEnd?.altitude ?? '—'})`;
          } else if (cell.rise || cell.set) {
            when = cell.rise || cell.set;
            extra = JSON.stringify(cell.extraInfo || {});
          }

          rows.push({ name: entryName, type: cell.type || '—', when: when || '—', extra });
        }
      }
    }
  }

  return rows;
}


async function loadData() {
  loading.value = true;
  events.value = [];
  error.value = '';
  rawData.value = null;

  // Валидация параметров
  const validLat = Math.max(-90, Math.min(90, lat.value));
  const validLon = Math.max(-180, Math.min(180, lon.value));
  const validDays = Math.floor(Math.max(1, Math.min(365, days.value)));

  const params = new URLSearchParams({
    lat: validLat,
    lon: validLon,
    days: validDays
  });

  try {
    const res = await fetch(`/astro/events?${params.toString()}`);
    if (!res.ok) throw new Error(`Сервер вернул ${res.status}`);
    const js = await res.json();
    rawData.value = js;
    events.value = parseEvents(js).slice(0, 200);
  } catch (e) {
    console.error('Ошибка загрузки ASTRO:', e);
    error.value = e.message;
  } finally {
    loading.value = false;
  }
}

function applyFilters() {
  loadData();
}

// Автозагрузка
loadData();
</script>


<style>
.astro-card {
  border: 2px solid #444;
  border-radius: 8px;
  padding: 15px;
  background-color: #f9f9f9;
  margin-bottom: 20px;
  width: 100%;
  box-sizing: border-box;
}

.astro-form {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 15px;
}

.astro-form input,
.astro-form button {
  padding: 5px 8px;
  border-radius: 4px;
  border: 1px solid #888;
}

/* Контейнер таблицы с горизонтальным скроллом */
.table-container {
  width: 100%;
  overflow-x: auto;
}

/* Стили для самой таблицы */
.table {
  width: 100%;
  border-collapse: collapse;
}

/* Заголовки и ячейки */
.table th,
.table td {
  padding: 6px 10px;
  border: 1px solid #ccc;
  text-align: left;
  vertical-align: top;
  white-space: nowrap; /* по умолчанию не переносим */
}

/* Колонка "Дополнительно" */
.table td.extra {
  max-width: 400px;       /* ограничиваем ширину */
  white-space: normal;    /* разрешаем перенос */
  word-break: break-word; /* длинные слова переносятся */
}

/* Стилизация полосы прокрутки (необязательно) */
.table-container::-webkit-scrollbar {
  height: 8px;
}

.table-container::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 4px;
}

.table-container::-webkit-scrollbar-thumb:hover {
  background: #555;
}

</style>
