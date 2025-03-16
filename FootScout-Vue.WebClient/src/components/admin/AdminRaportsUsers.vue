<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import UserService from '../../services/api/UserService';
import { TimeService } from '../../services/time/TimeService';
import BarChartComponent from '../layout/BarChartComponent.vue';
import '../../styles/admin/AdminRaportsUsers.css';

const toast = useToast();
const router = useRouter();

const userCount = ref<number>(0);
const userCreationData = ref<{ date: string; count: number }[]>([]);
const currentMonth = ref<Date>(new Date());

onMounted(async () => {
  try {
    const _users = await UserService.getUsers();

    if (_users.length > 0) {
      const firstUserDate = new Date(Math.min(..._users.map(user => new Date(user.creationDate).getTime()))).toISOString().split('T')[0];
      const todayDate = new Date().toISOString().split('T')[0];

      const dateRange = TimeService.generateDateRange(firstUserDate, todayDate);
      const creationCounts: Record<string, number> = {};
      _users.forEach(user => {
        const creationDate = new Date(user.creationDate).toISOString().split('T')[0];
        creationCounts[creationDate] = (creationCounts[creationDate] || 0) + 1;
      });

      userCreationData.value = dateRange.map(date => ({
        date,
        count: creationCounts[date] || 0,
      })).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

      userCount.value = _users.length;
    }
  }
  catch (error) {
    console.error('Failed to fetch user data:', error);
    toast.error('Failed to load user data.');
  }
});

const changeMonth = (direction: number) => {
  currentMonth.value = new Date(currentMonth.value.getFullYear(), currentMonth.value.getMonth() + direction, 1);
};


const usersFilteredData = computed(() => {
  return userCreationData.value.filter(item => item.date.startsWith(TimeService.formatDateToMonth(currentMonth.value)));
});

const chartData = computed(() => ({
  labels: usersFilteredData.value.map(item => item.date),
  datasets: [
    {
      label: 'User Creation Count',
      backgroundColor: '#8884d8',
      data: usersFilteredData.value.map(item => item.count),
    },
  ],
}));

const exportDataToCSV = async () => {
  await UserService.exportUsersToCsv();
};

const navigateToUsers = () => {
  router.push('/admin/users');
};
</script>

<template>
  <div class="AdminRaportsUsers">
    <h1><i class="bi bi-people-fill"></i> Users - Reports & Stats</h1>
    <h3>Users count: <strong>{{ userCount }}</strong></h3>

    <button class="btn btn-primary me-2" @click="navigateToUsers">
      <i class="bi bi-info-circle"></i> Show Users
    </button>
    <button class="btn btn-success" @click="exportDataToCSV">
      <i class="bi bi-download"></i> Export to CSV
    </button>

    <h5>Creation dates for {{ TimeService.formatDateToMonth(currentMonth) }}</h5>
    <div class="histogram-container">
      <BarChartComponent :chart-data="chartData" />
    </div>

    <button class="btn btn-dark me-2" @click="changeMonth(-1)">
      <i class="bi bi-arrow-left"></i>
    </button>
    <button class="btn btn-dark" @click="changeMonth(1)">
      <i class="bi bi-arrow-right"></i>
    </button>
  </div>
</template>