<script setup lang="ts">
import { useRouter } from 'vue-router';
import { AccountService } from '../../services/api/AccountService';
import { useToast } from 'vue-toast-notification';
import '../../style.css';
import '../../styles/layout/Navbar.css';

const router = useRouter();
const toast = useToast();

const logout = async () => {
  await AccountService.logout();
  router.push('/');
  toast.success('Logged out successfully.');
};

</script>

<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark sticky-top">
    <div class="container">
      <img src="../../img/logo.png" alt="logo" class="logo" />
      <router-link to="/home" class="navbar-brand">FootScout</router-link>
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
        <ul class="navbar-nav me-auto green-links">
          <li class="nav-item">
            <router-link to="/home" class="nav-link">
              <i class="bi bi-house-fill"></i> Home
            </router-link>
          </li>
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
              <i class="bi bi-list-nested"></i> Advertisements
            </a>
            <ul class="dropdown-menu">
              <li>
                <router-link to="/player-advertisements" class="dropdown-item">
                  <i class="bi bi-person-bounding-box"></i> Player's
                </router-link>
              </li>
              <li>
                <router-link to="/club-advertisements" class="dropdown-item">
                  <i class="bi bi-shield-fill"></i> Club's
                </router-link>
              </li>
            </ul>
          </li>
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
              <i class="bi bi-file-earmark-plus"></i> New Advertisement
            </a>
            <ul class="dropdown-menu">
              <li>
                <router-link to="/new-player-advertisement" class="dropdown-item">
                  <i class="bi bi-person-bounding-box"></i> as Player
                </router-link>
              </li>
              <li>
                <router-link to="/new-club-advertisement" class="dropdown-item">
                  <i class="bi bi-shield-fill"></i> as Club
                </router-link>
              </li>
            </ul>
          </li>
        </ul>

        <!-- Prawa strona -->
        <ul class="navbar-nav ms-auto green-links">
          <li class="nav-item">
            <router-link to="/chats" class="nav-link">
              <i class="bi bi-chat-fill"></i> Chat
            </router-link>
          </li>
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
              <i class="bi bi-briefcase-fill"></i> My Offers
            </a>
            <ul class="dropdown-menu">
              <li>
                <router-link to="/my-offers-as-player" class="dropdown-item">
                  <i class="bi bi-person-bounding-box"></i> as Player
                </router-link>
              </li>
              <li>
                <router-link to="/my-offers-as-club" class="dropdown-item">
                  <i class="bi bi-shield-fill"></i> as Club
                </router-link>
              </li>
            </ul>
          </li>
          <li class="nav-item dropdown sidebar-dropdown">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
              <i class="bi bi-person-circle"></i> My Profile
            </a>
            <ul class="dropdown-menu">
              <li>
                <router-link to="/my-profile" class="dropdown-item">
                  <i class="bi bi-person-fill"></i> Profile
                </router-link>
              </li>
              <li>
                <router-link to="/club-history" class="dropdown-item">
                  <i class="bi bi-clock-history"></i> Club History
                </router-link>
              </li>
              <li>
                <router-link to="/my-player-advertisements" class="dropdown-item">
                  <i class="bi bi-person-bounding-box"></i> Player Ads
                </router-link>
              </li>
              <li>
                <router-link to="/my-club-advertisements" class="dropdown-item">
                  <i class="bi bi-shield-fill"></i> Club Ads
                </router-link>
              </li>
              <li>
                <router-link to="/my-favorite-player-advertisements" class="dropdown-item">
                  <i class="bi bi-chat-square-heart"></i> Favorite Player Ads
                </router-link>
              </li>
              <li>
                <router-link to="/my-favorite-club-advertisements" class="dropdown-item">
                  <i class="bi bi-chat-square-heart-fill"></i> Favorite Club Ads
                </router-link>
              </li>
              <li>
                <router-link to="/support" class="dropdown-item">
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