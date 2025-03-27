// Konfiguracja Vite, określająca użycie Vue jako frameworka (plugins: [vue()]).

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
})