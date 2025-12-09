<template>
  <div class="jwst-page">
    <h2>JWST — последние изображения</h2>
    <JwstGallery />
  </div>
</template>

<script setup>
import JwstGallery from './JwstGallery.vue';
</script>

<style>
.jwst-page {
  padding: 20px;
}
.jwst-page h2 {
  margin-bottom: 15px;
  color: #333;
}
</style>
