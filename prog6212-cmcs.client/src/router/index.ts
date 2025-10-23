import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      component: () => import("@/views/index.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/login",
      alias: "/login",
      name: "auth",
      component: () => import("@/views/auth/login.vue"),
      meta: { requiresAuth: false },
    },
    { path: "/:pathMatch(.*)*", name: "NotFound", component: () => import("@/views/Error.vue") },
  ],
});

router.beforeResolve(async (to, from, next) => {
  next();
});
export default router;
