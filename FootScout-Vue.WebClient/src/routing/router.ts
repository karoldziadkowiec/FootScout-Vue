// Definiuje ścieżki nawigacyjne aplikacji Vue, przypisuje im odpowiednie komponenty i role 
// użytkowników oraz obsługuje autoryzację przed zmianą trasy (czy użytkownik jest zalogowany, 
// czy ma ważny token sesji, czy posiada odpowiednią rolę)

import { createRouter, createWebHistory } from 'vue-router';
import { AccountService } from '../services/api/AccountService';
import { Role } from '../models/enums/Role';
// Publiczne dostępne strony
import Login from '../components/account/Login.vue';
import Registration from '../components/account/Registration.vue';
// Strony użytkownika
import Home from '../components/home/Home.vue';
import MyProfile from '../components/user/MyProfile.vue';
import ClubHistory from '../components/user/ClubHistory.vue';
import PlayerAdvertisements from '../components/playerAdvertisement/PlayerAdvertisements.vue';
import PlayerAdvertisement from '../components/playerAdvertisement/PlayerAdvertisement.vue';
import MyPlayerAdvertisements from '../components/user/MyPlayerAdvertisements.vue';
import MyFavoritePlayerAdvertisements from '../components/user/MyFavoritePlayerAdvertisements.vue';
import Chats from '../components/chat/Chats.vue';
import Chat from '../components/chat/Chat.vue';
import MyOffersAsPlayer from '../components/user/MyOffersAsPlayer.vue';
import MyOffersAsClub from '../components/user/MyOffersAsClub.vue';
import NewPlayerAdvertisement from '../components/playerAdvertisement/NewPlayerAdvertisement.vue';
import Support from '../components/support/Support.vue';
// Strony admina
import AdminDashboard from '../components/admin/AdminDashboard.vue';
import AdminUsers from '../components/admin/AdminUsers.vue';
import AdminChats from '../components/admin/AdminChats.vue';
import AdminChat from '../components/admin/AdminChat.vue';
import AdminPlayerAdvertisements from '../components/admin/AdminPlayerAdvertisements.vue';import AdminClubOffers from '../components/admin/AdminClubOffers.vue';
import AdminSupport from '../components/admin/AdminSupport.vue';
import AdminPlayerPositions from '../components/admin/AdminPlayerPositions.vue';
import AdminMakeAnAdmin from '../components/admin/AdminMakeAnAdmin.vue';
import AdminRaportsUsers from '../components/admin/AdminRaportsUsers.vue';
import AdminRaportsChats from '../components/admin/AdminRaportsChats.vue';
import AdminRaportsPlayerAdvertisements from '../components/admin/AdminRaportsPlayerAdvertisements.vue';
import AdminRaportsClubOffers from '../components/admin/AdminRaportsClubOffers.vue';

// ścieżki
const routes = [
  // Publiczne dostępne strony
  { path: '/', name: 'Login', component: Login },
  { path: '/registration', name: 'Registration', component: Registration },

  // Strony użytkownika
  { path: '/home', name: 'Home', component: Home, meta: { roles: [Role.User] } },
  { path: '/my-profile', name: 'MyProfile', component: MyProfile, meta: { roles: [Role.Admin, Role.User] } },
  { path: '/club-history', name: 'ClubHistory', component: ClubHistory, meta: { roles: [Role.User] } },
  { path: '/player-advertisements', name: 'PlayerAdvertisements', component: PlayerAdvertisements, meta: { roles: [Role.Admin, Role.User] } },
  { path: '/player-advertisement/:id', name: 'PlayerAdvertisement', component: PlayerAdvertisement, meta: { roles: [Role.Admin, Role.User] } },
  { path: '/my-player-advertisements', name: 'MyPlayerAdvertisements', component: MyPlayerAdvertisements, meta: { roles: [Role.User] } },
  { path: '/my-favorite-player-advertisements', name: 'MyFavoritePlayerAdvertisements', component: MyFavoritePlayerAdvertisements, meta: { roles: [Role.User] } },
  { path: '/chats', name: 'Chats', component: Chats, meta: { roles: [Role.Admin, Role.User] } },
  { path: '/chat/:id', name: 'Chat', component: Chat, meta: { roles: [Role.Admin, Role.User] } },
  { path: '/my-offers-as-player', name: 'MyOffersAsPlayer', component: MyOffersAsPlayer, meta: { roles: [Role.User] } },
  { path: '/my-offers-as-club', name: 'MyOffersAsClub', component: MyOffersAsClub, meta: { roles: [Role.User] } },
  { path: '/new-player-advertisement', name: 'NewPlayerAdvertisement', component: NewPlayerAdvertisement, meta: { roles: [Role.User] } },
  { path: '/support', name: 'Support', component: Support, meta: { roles: [Role.Admin, Role.User] } },

  // Strony admina
  { path: '/admin/dashboard', name: 'AdminDashboard', component: AdminDashboard, meta: { roles: [Role.Admin] } },
  { path: '/admin/users', name: 'AdminUsers', component: AdminUsers, meta: { roles: [Role.Admin] } },
  { path: '/admin/chats', name: 'AdminChats', component: AdminChats, meta: { roles: [Role.Admin] } },
  { path: '/admin/chat/:id', name: 'AdminChat', component: AdminChat, meta: { roles: [Role.Admin] } },
  { path: '/admin/player-advertisements', name: 'AdminPlayerAdvertisements', component: AdminPlayerAdvertisements, meta: { roles: [Role.Admin] } },
  { path: '/admin/club-offers', name: 'AdminClubOffers', component: AdminClubOffers, meta: { roles: [Role.Admin] } },
  { path: '/admin/support', name: 'AdminSupport', component: AdminSupport, meta: { roles: [Role.Admin] } },
  { path: '/admin/player-positions', name: 'AdminPlayerPositions', component: AdminPlayerPositions, meta: { roles: [Role.Admin] } },
  { path: '/admin/make-admin', name: 'AdminMakeAnAdmin', component: AdminMakeAnAdmin, meta: { roles: [Role.Admin] } },
  { path: '/admin/raports/users', name: 'AdminRaportsUsers', component: AdminRaportsUsers, meta: { roles: [Role.Admin] } },
  { path: '/admin/raports/chats', name: 'AdminRaportsChats', component: AdminRaportsChats, meta: { roles: [Role.Admin] } },
  { path: '/admin/raports/player-advertisements', name: 'AdminRaportsPlayerAdvertisements', component: AdminRaportsPlayerAdvertisements, meta: { roles: [Role.Admin] } },
  { path: '/admin/raports/club-offers', name: 'AdminRaportsClubOffers', component: AdminRaportsClubOffers, meta: { roles: [Role.Admin] } },
];

// Tworzenie routera Vue z wykorzystaniem historii przeglądarki
const router = createRouter({
  history: createWebHistory(), // Używa historii HTML5 do obsługi nawigacji bez przeładowania strony
  routes, // Przekazanie zdefiniowanych ścieżek do routera
});

// Middleware autoryzacji – wywoływane przed każdą zmianą trasy
router.beforeEach(async (to, _from, next) => {
  const isAuthenticated = await AccountService.isAuthenticated(); // Sprawdź czy użytkownik jest zalogowany
  const userRole = await AccountService.getRole(); // Pobierz rolę aktualnie zalogowanego użytkownika
  const isTokenValid = await AccountService.isTokenAvailable(); // Sprawdź czy token sesji jest nadal ważny

  // Pobierz wymagane role, jeśli istnieją
  const requiredRoles = to.meta.roles as Role[] | undefined;

  // Jeśli trasa wymaga określonych ról..
  if (requiredRoles?.length) {
    // Jeśli użytkownik nie jest zalogowany przekierowuje go na stronę główną z komunikatem
    if (!isAuthenticated) {
      return next({ path: '/', state: { toastMessage: "You are not authenticated. Please log in." } });
    }

    // Jeśli token sesji jest nieważny wymusza ponowne logowanie
    if (!isTokenValid) {
      return next({ path: '/', state: { toastMessage: "Session expired. Please log in again." } });
    }

    // Jeśli użytkownik nie ma odpowiedniej roli odmawia dostępu
    if (!userRole || !requiredRoles.includes(userRole)) {
      return next({ path: '/', state: { toastMessage: "Access denied. Wrong role." } });
    }
  }
  next(); // Jeśli wszystkie warunki zostały spełnione przejdź do docelowej trasy
});

export default router;