<template>
  <div class="border rounded p-2 text-center">
    <div class="small text-muted">{{ title }}</div>
    <div class="fs-4">{{ value ?? '—' }}</div>
  </div>
</template>

<script setup>
defineProps({
  title: String,
  value: [Number, String]
});
</script>
