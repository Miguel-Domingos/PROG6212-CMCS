import { useAuthStore } from "@/stores/auth";

export function useNotification(onMessage: (msg: any) => void) {
  const userStore = useAuthStore();



  async function start() {
    try {
      await connection.start();
      console.log("📡 Connected to SignalR Hub");
    } catch (err) {
      console.error("❌ Failed to connect:", err);
      setTimeout(start, 3000);
    }
  }

  start();

  return { connection };
}
