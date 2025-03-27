// Punkt wejścia aplikacji Vue, gdzie tworzone i konfigurowane jest całe środowisko.

import { createApp } from 'vue';
import './style.css';
import App from './App.vue';
import router from './routing/router.ts';       // Import routera dla nawigacji
import { createBootstrap } from 'bootstrap-vue-next'
import ToastContainer from 'vue-toast-notification';

// Import styli Bootstrap
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'bootstrap-vue-next/dist/bootstrap-vue-next.css'
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'vue-toast-notification/dist/theme-sugar.css';


createApp(App)      // Tworzenie instancji aplikacji Vue
.use(router)        // Dodanie routera do przekierowań stron
.use(ToastContainer) // Kontener Toast do wyświetlania na ekranie powiadomień
.use(createBootstrap()) // Style bootstrap-vue-next
.mount('#app');     // Osadzenie aplikacji w #app

// Tworzy instancję aplikacji, dodaje router, style i powiadomienia, a następnie montuje Vue w #app.