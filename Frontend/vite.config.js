import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
  plugins: [vue(), tailwindcss()],
  server: {
    port: 5173,
    proxy: {
      '/iss': {
        target: 'http://localhost:5170',
        changeOrigin: true
      },
      '/jwst': {
        target: 'http://localhost:5170',
        changeOrigin: true
      },
      '/odsr': {
        target: 'http://localhost:5170',
        changeOrigin: true
      },
      '/astro':{
        target: 'http://localhost:5170',
        changeOrigin: true
      }
    },
    historyApiFallback: true
  }
});

