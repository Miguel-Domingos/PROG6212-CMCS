import { createRouter, createWebHistory } from "vue-router";
import RouterGuard from "@/middleware/RouterGuard.global";
import NotFound from "@/views/Error.vue";



const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "app",
      component: () => import("@/views/index.vue"),
      meta: { requireAuth: true },
    },
    {
      path: "/login",
      name: "auth:login",
      component: () => import("@/views/auth/login.vue"),
      meta: { requireAuth: false },
    },
    { path: "/:pathMatch(.*)*", name: "NotFound", component: NotFound },
  ],
});

router.beforeResolve((to, from, next) => RouterGuard(to, from, next));
export default router;
