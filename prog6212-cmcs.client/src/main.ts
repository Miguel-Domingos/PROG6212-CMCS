import "./assets/main.css";
import { createApp } from "vue";
import ui from "@nuxt/ui/vue-plugin";
import App from "./App.vue";
import router from "./router";
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'




const app = createApp(App);

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)

app.use(router);
app.use(pinia);
app.use(ui);

app.mount("#app");
