import { createRouter, createWebHistory } from 'vue-router';
import Dashboard from '../components/AstroDashboard.vue';
import JwstPage from '../components/JwstPage.vue';
import OdsrPage from '../components/OdsrDashboard.vue';
import AstroPage from '../components/AstronomyApi.vue';
import HomePage from '../components/Welcome.vue'

const routes = [
  {
    path: '/dash',
    name: 'Dashboard',
    component: Dashboard
  },
  {
    path: '/jwst',
    name: 'JWST',
    component: JwstPage
  },
  {
    path: '/odsr',
    name: 'ODSR',
    component: OdsrPage
  },
  {
    path: '/astro',
    name: 'ASTRO',
    component: AstroPage
  },
  {
    path: '/',
    name: 'Home',
    component: HomePage

  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
