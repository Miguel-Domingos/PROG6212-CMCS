import { useAuthStore } from "@/stores/auth";
import type { NavigationGuardWithThis } from "vue-router";

const RouterGuard: NavigationGuardWithThis<void> = (to, from, next) => {
  const token = useAuthStore().token;

 
  if (!to.meta.requireAuth && token) {
    next({ name: "app" });
  } else if (to.meta.requireAuth && !token) {
    next({ name: "auth:login" });
  } else {
    next();
  }
};

export default RouterGuard;
