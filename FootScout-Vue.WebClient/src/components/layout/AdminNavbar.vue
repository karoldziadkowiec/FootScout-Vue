<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { AccountService } from '../../services/api/AccountService';
import { useToast } from 'vue-toast-notification';
import '../../style.css';
import '../../styles/layout/AdminNavbar.css';

const route = useRoute();
const router = useRouter();
const toast = useToast();

const logout = async () => {
  await AccountService.logout();
  router.push('/');
  toast.success('Logged out successfully.');
};

// Funkcja sprawdzająca, czy dany link powinien być podświetlony
const isActive = (path: string) => route.path.startsWith(path);
</script>

<template>
  <nav class="navbar navbar-expand-lg navbar-primary bg-primary sticky-top">
    <div class="container">
      <img src="../../img/logo.png" alt="logo" class="logo" />
      <router-link to="/admin/dashboard" class="navbar-brand">FootScout</router-link>
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <!-- Lewa strona -->
        <ul class="navbar-nav me-auto blue-links">
          <li class="nav-item">
            <router-link to="/admin/dashboard" class="nav-link" :class="{ active: isActive('/admin/dashboard') }">
              <i class="bi bi-grid"></i> Dashboard
            </router-link>
          </li>
          <li class="nav-item">
            <router-link to="/admin/users" class="nav-link" :class="{ active: isActive('/admin/users') }">
              <i class="bi bi-people-fill"></i> Users
            </router-link>
          </li>
          <li class="nav-item">
            <router-link to="/admin/chats" class="nav-link" :class="{ active: isActive('/admin/chats') }">
              <i class="bi bi-chat-text-fill"></i> Chats
            </router-link>
          </li>
          <li class="nav-item">
            <router-link to="/admin/player-advertisements" class="nav-link" :class="{ active: isActive('/admin/player-advertisements') }">
              <i class="bi bi-list-nested"></i> Advertisements
            </router-link>
          </li>
          <li class="nav-item">
            <router-link to="/admin/club-offers" class="nav-link" :class="{ active: isActive('/admin/club-offers') }">
              <i class="bi bi-briefcase-fill"></i> Offers
            </router-link>
          </li>

          <!-- Dropdown Reports & Stats -->
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
              <i class="bi bi-bar-chart-fill"></i> Reports & Stats
            </a>
            <ul class="dropdown-menu">
              <li>
                <router-link
                  to="/admin/raports/users"
                  class="dropdown-item"
                  :class="{ active: isActive('/admin/raports/users') }"
                >
                  <i class="bi bi-people-fill"></i> Users
                </router-link>
              </li>
              <li>
                <router-link
                  to="/admin/raports/chats"
                  class="dropdown-item"
                  :class="{ active: isActive('/admin/raports/chats') }"
                >
                  <i class="bi bi-chat-text-fill"></i> Chats
                </router-link>
              </li>
              <li>
                <router-link
                  to="/admin/raports/player-advertisements"
                  class="dropdown-item"
                  :class="{ active: isActive('/admin/raports/player-advertisements') }"
                >
                  <i class="bi bi-person-bounding-box"></i> Player Advertisements
                </router-link>
              </li>
              <li>
                <router-link
                  to="/admin/raports/club-offers"
                  class="dropdown-item"
                  :class="{ active: isActive('/admin/raports/club-offers') }"
                >
                  <i class="bi bi-briefcase-fill"></i> Club Offers
                </router-link>
              </li>
            </ul>
          </li>

          <!-- Services -->
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
              <i class="bi bi-gear-fill"></i> Service
            </a>
            <ul class="dropdown-menu">
              <li>
                <router-link
                  to="/admin/support"
                  class="dropdown-item"
                  :class="{ active: isActive('/admin/support') }"
                >
                  <i class="bi bi-cone-striped"></i> Reported Problems
                </router-link>
              </li>
              <li>
                <router-link
                  to="/admin/player-positions"
                  class="dropdown-item"
                  :class="{ active: isActive('/admin/player-positions') }"
                >
                  <i class="bi bi-person-standing"></i> Player Positions
                </router-link>
              </li>
              <li>
                <router-link
                  to="/admin/make-admin"
                  class="dropdown-item"
                  :class="{ active: isActive('/admin/make-admin') }"
                >
                  <i class="bi bi-universal-access-circle"></i> Make an Admin
                </router-link>
              </li>
            </ul>
          </li>
        </ul>

        <!-- Prawa strona -->
        <ul class="navbar-nav ms-auto blue-links">
          <li class="nav-item">
            <router-link to="/chats" class="nav-link" :class="{ active: isActive('/chats') }">
              <i class="bi bi-chat-fill"></i> Chat
            </router-link>
          </li>

          <!-- Dropdown My Profile -->
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
              <i class="bi bi-person-circle"></i> My Profile
            </a>
            <ul class="dropdown-menu">
              <li>
                <router-link
                  to="/my-profile"
                  class="dropdown-item"
                  :class="{ active: isActive('/my-profile') }"
                >
                  <i class="bi bi-person-fill"></i> Profile
                </router-link>
              </li>
              <li>
                <router-link
                  to="/support"
                  class="dropdown-item"
                  :class="{ active: isActive('/support') }"
                >
                  <i class="bi bi-wrench-adjustable"></i> Support
                </router-link>
              </li>
              <li>
                <button @click="logout" class="dropdown-item">
                  <i class="bi bi-box-arrow-left"></i> Log out
                </button>
              </li>
            </ul>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>