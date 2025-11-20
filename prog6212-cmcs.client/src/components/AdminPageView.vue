<script setup lang="ts">
  import { ref, reactive, onMounted, computed } from "vue";
  import { useApi } from "@/composables/useApi";
  import type { TableColumn } from "@nuxt/ui";


  const api = useApi();
  const toast = useToast();
  const loading = ref(false)
  const users = ref([]);
  const roles = ref([]);
  const modalOpen = ref(false);

  const form = reactive({
    name: "",
    email: "",
    password: "",
    roleId: null as number | null
  });

  // --- LOAD INITIAL DATA ---
  async function loadUsers() {
    users.value = await api("/users");

  }

  async function loadRoles() {
    roles.value = await api("/roles");

  }

  onMounted(async () => {
    try {
      loading.value = true
      await Promise.all([loadUsers(), loadRoles()]);
    } finally {
      loading.value = false
    }
  });


  const columns: TableColumn<any>[] = [
    {
      accessorKey: "name",
      header: "Name",
    },
    {
      accessorKey: "email",
      header: "Email",
    },
    {
      accessorKey: "role",
      header: "Role",
    },
    {
      accessorKey: "actions",
      header: "Actions",
    },
  ];

  const formattedUsers = computed(() =>
    users.value
      ? users.value.map((user) => ({
        name: user.name,
        email: user.email,
        role: user.role.roleName,
      }))
      : []
  );
</script>

<template>
  <UContainer class="space-y-6">
    <div class="w-full flex justify-between items-center">
      <UButton color="neutral" variant="soft" :loading label="update list" @click="loadUsers()" />
      <CreateUserModal @success="loadUsers" />
    </div>

    <UTable :loading :columns :data="formattedUsers" />
</UContainer>
</template>
