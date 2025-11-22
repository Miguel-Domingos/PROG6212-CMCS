<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import * as signalR from "@microsoft/signalr";

import ClaimFormModal from '@/components/ClaimFormModal.vue';
import { useApi } from '@/composables/useApi';
import { useAuthStore } from "@/stores/auth.ts";

interface Claim {
  claimId: number;
  claimDate: string;
  hoursWorked: number;
  totalAmount: number;
  status: number;
  notes: string;
  documents: { fileName: string; filePath: string }[];
}

const api = useApi();
const claims = ref<Claim[]>([]);

const totalAmount = computed(() => claims.value.reduce((sum, c) => sum + c.totalAmount, 0));
const pendingClaims = computed(() => claims.value.filter(c => c.status === 0).length);
const approvedClaims = computed(() => claims.value.filter(c => c.status === 1).length);

const cards = ref([
  { title: 'Total Claims Amount', icon: 'lucide:circle-dollar-sign', value: '$0.00' },
  { title: 'Pending Claims', icon: 'lucide:clock-4', value: '0' },
  { title: 'Approved Claims', icon: 'lucide:paperclip', value: '0' },
]);

const items = [{ label: 'My Claims', icon: 'i-lucide-user' },]

function updateCards() {
  cards.value[0]!.value = `$${totalAmount.value.toFixed(2)}`;
  cards.value[1]!.value = pendingClaims.value.toString();
  cards.value[2]!.value = approvedClaims.value.toString();
}

async function fetchClaims() {
  const data = await api('/claims/mine');
  claims.value = data;
  updateCards();
}

async function downloadFile(docId: number, fileName: string) {
  try {
    const response = await api(`/SupportingDocuments/download/${docId}`);

    const blob = new Blob([response]);
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);

  } catch (error) {
    console.error('Erro no download:', error);
    alert('Erro ao baixar arquivo');
  }
}


onMounted(async () => {
  await fetchClaims();

  const auth = useAuthStore();

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5039/hubs/notifications", {
      accessTokenFactory: () => auth.token || ""
    })
    .withAutomaticReconnect()
    .build();

  connection.on("claimUpdated", async (notification) => {
    console.log("🔔 Notification received:", notification);

    useToast().add({
      title: "Claim status Updated",
      description: notification.message,
    });
    await fetchClaims();
  });

  try {
    await connection.start();
  } catch (err) {
    console.error("Error connecting to SignalR:", err);
  }
});
</script>

<template>
  <UContainer>
    <div class="grid grid-cols-3 gap-4 mb-10">
      <UCard v-for="card in cards">
        <template #header>
          <div class="flex justify-between items-center">
            {{card.title}}

            <UIcon :name="card.icon" class="size-5" />
          </div>
        </template>

        <span class="text-2xl font-semibold">{{card.value}}</span>

      </UCard>
    </div>

    <UTabs :items="items" variant="link">
      <template #content="{ item }">
        <UCard>
          <div class="w-full flex justify-between items-center">
            <div>
              <h1 class="font-bold text-2xl">Claims History</h1>
              <p class="text-sm text-gray-500">View and manage your submitted claims</p>
            </div>

            <ClaimFormModal @created="fetchClaims" />
          </div>

          <div v-if="claims.length > 0" class="flex flex-col w-full gap-2 mt-4">
            <UCard v-for="claim in claims">
              <div class="w-full flex justify-between" items-center>
                <div class="flex flex-col gap-1">
                  <span>{{claim.title}}</span>
                  <span>{{claim.notes}}</span>
                  <span>Hours Worked: {{claim.hoursWorked}}</span>
                  <span>submitted: {{new Date(claim.claimDate).toLocaleDateString()}}</span>
                  <UButton v-if="claim.documents.length > 0" size="xs" label="Download document"
                           @click="downloadFile(claim.documents[0]!.documentId, claim.documents[0]!.fileName)"
                           class="w-fit"/>
                </div>

                <div class="flex flex-col gap-2 items-end">
                  <span>{{claim.totalAmount}}</span>
                  <UBadge v-if="claim.status === 0" variant="soft" color="neutral">Pending</UBadge>
                  <UBadge v-else-if="claim.status === 1" color="neutral">Approved</UBadge>
                  <UBadge v-else color="error">Rejected</UBadge>
                </div>
              </div>
            </UCard>
          </div>

          <span v-else>No Claims</span>
        </UCard>
      </template>
    </UTabs>
  </UContainer>
</template>
