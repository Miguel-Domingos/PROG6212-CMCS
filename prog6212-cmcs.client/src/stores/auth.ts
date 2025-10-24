// stores/auth.ts
import { defineStore } from "pinia";
import { useApi } from "@/composables/useApi";
import router from "@/router";

interface User {
  userId: number;
  name: string;
  email: string;
  role: string;
}

interface LoginResponse {
  message: string;
  token: string;
  user: User;
}

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: "" as string,
    user: null as User | null,
  }),

  actions: {
    async login(email: string, password: string) {
      const api = useApi();
      console.log("Attempting login with", {
        email, password
      })
        const response = await api<LoginResponse>("/auth/login", {
          method: "POST",
          body: { email, password },
        });

        this.token = response.token;
        this.user = response.user;

        useToast().add({
          title: "Welcome back!",
          description: `Logged in as ${response.user.role}`,
          color: "success",
        });

        router.push("/");
   
    },

    logout() {
      this.$reset();
      useToast().add({
        title: "Logged out",
        description: "You have been logged out successfully.",
      });
      router.push("/login");
    },
  },

  persist: true,
});
