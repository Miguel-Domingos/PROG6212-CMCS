<script setup lang="ts">
  import { useAuthStore } from '@/stores/auth';
  import AcademicAndCoordinatorPageView from '@/components/AcademincAndCoordinatorPageView.vue';
  import LecturerPageView from '@/components/LecturerPageView.vue';

  const { user, logout } = useAuthStore();

</script>

<template>
  <div class="w-full h-full">
    <UHeader>
      <template #title>
       {{user.role}} Dashboard
      </template>

      

      <template #right>
        <div class="flex items-center gap-2">

        <div class="flex items-center gap-1">
          <UIcon name="i-lucide-user" class="size-5" />
          {{user.name}} ({{user.role}})
        </div>

        <UColorModeButton />

        <UButton @click="logout" icon="i-lucide-log-out" color="neutral" variant="solid">logout</UButton>
        </div>
      </template>

    </UHeader>

    <UMain class="mt-4">

      <AcademicAndCoordinatorPageView v-if="user.role === 'AcademicManager' || user.role === 'ProgrammeCoordinator'" />
      <LecturerPageView v-else-if="user.role === 'Lecturer'" />
      <div v-else>
        <p class="text-center text-lg">Unauthorized Role: {{user.role}}</p>
      </div>

    </UMain>
    </div>
</template>
