<script setup lang="ts">
import * as z from "zod";
import { reactive, ref, computed, onMounted } from "vue";
import { useApi } from "@/composables/useApi";


const emits = defineEmits(["success"]);
const api = useApi();
const toast = useToast();
const open = ref(false);
const roles = ref([]);

async function loadRoles() {
  roles.value = await api("/roles");
}

onMounted(() => {
  loadRoles();
});

const roleItems = computed(() =>
  roles.value.map((r: any) => ({
    label: r.roleName,
    value: r.roleId,
  }))
);

const schema = z.object({
  name: z.string().min(1, "Name required"),
  email: z.string().email("Invalid email"),
  password: z.string().min(4, "Min 4 characters"),
  roleId: z.number().min(1, "Select a role"),
  bankDetails: z.string().optional(),
});

type Schema = z.output<typeof schema>;

const state = reactive<Schema>({
  name: "",
  email: "",
  password: "",
  roleId: null,
  bankDetails: ""
});

async function onSubmit() {
  try {
    await api("/admin/users", {
      method: "POST",
      body: state,
    });

    toast.add({
      title: "Create User",
      description: "User created successfully",
      color: "success",
    });

    // reset form
    state.name = "";
    state.email = "";
    state.password = "";
    state.roleId = 0;
    state.bankDetails = ""

    open.value = false;

    emits("success");
  } catch (e) {
    toast.add({
      title: "Create User",
      description: "Failed to create user",
      color: "error",
    });
  }
}
</script>

<template>
  <UModal v-model:open="open" title="Create User">
    <UButton color="primary"
             icon="i-lucide-plus"
             label="New User"
             variant="solid" />

    <template #body>
      <UForm :schema="schema"
             :state="state"
             class="space-y-4"
             @submit="onSubmit">
        <UFormField label="Name" name="name">
          <UInput v-model="state.name" class="w-full" />
        </UFormField>

        <UFormField label="Email" name="email">
          <UInput v-model="state.email" type="email" class="w-full" />
        </UFormField>

        <UFormField label="Password" name="password">
          <UInput v-model="state.password" type="password" class="w-full" />
        </UFormField>

        <UFormField label="Role" name="roleId">
          <USelect v-model="state.roleId"
                   :items="roleItems"
                   placeholder="Select a role"
                   class="w-full" />
        </UFormField>

        <UFormField v-if="state.roleId == '4'" label="Bank Details" name="bankDetails">
          <UInput v-model="state.bankDetails" class="w-full" />
        </UFormField>

        <!-- Actions -->
        <div class="flex justify-end gap-2 mt-4">
          <UButton label="Cancel"
                   color="neutral"
                   variant="subtle"
                   @click="open = false" />
          <UButton label="Create"
                   color="primary"
                   variant="solid"
                   type="submit"
                   loading-auto />
        </div>
      </UForm>
    </template>
  </UModal>
</template>
