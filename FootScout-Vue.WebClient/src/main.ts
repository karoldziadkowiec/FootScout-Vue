import { createApp } from 'vue';
import './style.css';
import App from './App.vue';
import router from './routing/router.ts';
import { createBootstrap } from 'bootstrap-vue-next'
import ToastContainer from 'vue-toast-notification';
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'bootstrap-vue-next/dist/bootstrap-vue-next.css'
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'vue-toast-notification/dist/theme-sugar.css';

createApp(App)
.use(router) // Router do przekierowań stron
.use(ToastContainer) // Kontener Toast do wyświetlania na ekranie komunikatów
.use(createBootstrap()) // Style bootstrap-vue-next
.mount('#app');