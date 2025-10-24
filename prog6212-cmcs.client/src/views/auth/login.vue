<script setup lang="ts">
  import * as z from "zod";
  import type { FormSubmitEvent, AuthFormField } from "@nuxt/ui";
  import { ref } from "vue";
  import { ofetch } from "ofetch";
  import { useAuthStore } from "@/stores/auth";

  const toast = useToast();
  const loading = ref(false);
  const fields: AuthFormField[] = [
    {
      name: "email",
      type: "email",
      label: "Email",
      placeholder: "Enter your email",
      required: true,
    },
    {
      name: "password",
      label: "Password",
      type: "password",
      placeholder: "Enter your password",
      required: true,
    },
  ];

  const schema = z.object({
    email: z.email("Invalid email"),
    password: z.string("Password is required").min(8, "Must be at least 8 characters"),
  });

  type Schema = z.output<typeof schema>;
  const text = ref("")
  async function onSubmit(payload: FormSubmitEvent<Schema>) {
    try {
      loading.value = true;

      await useAuthStore().login(
         payload.data.email,
        payload.data.password,
      );
    } finally {
      loading.value = false;
    }
  }
</script>

<template>
  <div class="w-screen h-screen flex flex-col items-center justify-center gap-4 p-4">
    <UPageCard class="w-full max-w-md">

      <UAuthForm
        :schema="schema"
        title="Claims System Login"
        description="Enter your credentials to access the claim system."
        :loading
        :fields="fields"
        @submit="onSubmit"
        :submit="{
          label: 'Login',
          loadingAuto: true
        }"
      />
    </UPageCard>
  </div>
</template>
