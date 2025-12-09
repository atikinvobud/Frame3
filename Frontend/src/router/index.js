import { createRouter, createWebHistory } from 'vue-router';
import Dashboard from '../components/AstroDashboard.vue';
import JwstPage from '../components/JwstPage.vue';



const routes = [
  {
    path: '/',
    name: 'Dashboard',
    component: Dashboard
  },
  {
    path: '/jwst',
    name: 'JWST',
    component: JwstPage
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
