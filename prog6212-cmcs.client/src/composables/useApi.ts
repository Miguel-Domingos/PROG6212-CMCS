import { ofetch } from "ofetch";
import { useAuthStore } from "@/stores/auth";


export function useApi() {
  const auth = useAuthStore();
  const toast = useToast();

  const api = ofetch.create({
    baseURL: "http://localhost:5039/api",

    async onRequest({ options }) {
      if (auth.token) {
        options.headers = {
          ...options.headers,
          Authorization: `Bearer ${auth.token}`,
        };
      }
    },

    async onResponseError({ response }) {
      const message =
        response?._data?.message || response?._data || "Unexpected error.";

      toast.add({
        title: "API Error",
        description: message,
        color: "error",
      });


      if (response?.status === 401 && !response?.url.endsWith('/login')) auth.logout();
    },
  });

  return api;
}
