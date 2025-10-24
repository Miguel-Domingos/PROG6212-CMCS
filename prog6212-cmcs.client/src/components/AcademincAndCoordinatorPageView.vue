<script setup lang="ts">
  import { ref, onMounted, computed } from "vue";
  import { useApi } from "@/composables/useApi";

  interface Claim {
    claimId: number;
    lecturerName: string;
    claimDate: string;
    hoursWorked: number;
    totalAmount: number;
    status: number; // 0 = Pending, 1 = Approved, 2 = Rejected
    notes: string;
  }

  const api = useApi();
  const claims = ref<Claim[]>([]);

  // Estatísticas
  const totalClaims = computed(() => claims.value.length);
  const pendingClaims = computed(() => claims.value.filter(c => c.status === 0));
  const approvedClaims = computed(() => claims.value.filter(c => c.status === 1));
  const pendingAmount = computed(() =>
    pendingClaims.value.reduce((sum, c) => sum + c.totalAmount, 0)
  );

  // Cards
  const cards = ref([
    { title: "Total Claims", icon: "lucide:folder-open", value: "0" },
    { title: "Pending Claims", icon: "lucide:clock-4", value: "0" },
    { title: "Pending Amount", icon: "lucide:circle-dollar-sign", value: "$0.00" },
    { title: "Approved Claims", icon: "lucide:check-circle", value: "0" },
  ]);

  function updateCards() {
    cards.value[0].value = totalClaims.value.toString();
    cards.value[1].value = pendingClaims.value.length.toString();
    cards.value[2].value = `$${pendingAmount.value.toFixed(2)}`;
    cards.value[3].value = approvedClaims.value.length.toString();
  }

  async function fetchClaims() {
    try {
      const data = await api("/claims"); // rota que retorna todas
      claims.value = data;
      updateCards();
    } catch (err) {
      console.error("Error fetching claims:", err);
    }
  }

  onMounted(fetchClaims);

  // Ações
  async function approveClaim(claimId: number) {
    await api(`/claims/${claimId}/approve`, { method: "POST" });
    await fetchClaims();
  }

  async function rejectClaim(claimId: number) {
    await api(`/claims/${claimId}/reject`, { method: "POST" });
    await fetchClaims();
  }

  // Tabs com slots
  const items = [
    { label: "Pending Claims", icon: "i-lucide-hourglass", slot: "pending" },
    { label: "All Claims", icon: "i-lucide-list", slot: "all" },
  ];
</script>

<template>
  <UContainer>
    <!-- Cards -->
    <div class="grid grid-cols-4 gap-4 mb-10">
      <UCard v-for="card in cards" :key="card.title">
        <template #header>
          <div class="flex justify-between items-center">
            {{ card.title }}
            <UIcon :name="card.icon" class="size-5" />
          </div>
        </template>
        <span class="text-2xl font-semibold">{{ card.value }}</span>
      </UCard>
    </div>

    <!-- Tabs -->
    <UTabs :items="items" variant="link">
      <!-- Aba: Pending Claims -->
      <template #pending>
        <UCard>
          <h2 class="font-semibold text-xl mb-4">Pending Claims</h2>
          <div v-if="pendingClaims.length > 0" class="flex flex-col gap-4">
            <UCard v-for="claim in pendingClaims"
                   :key="claim.claimId">
              <div class="flex justify-between items-center">
 

                <div class="flex flex-col gap-1">
                  <span>{{claim.title}}</span>
                  <span>{{claim.notes}}</span>
                  <span>Hours Worked: {{claim.hoursWorked}}</span>
                  <span>submitted: {{new Date(claim.claimDate).toLocaleDateString()}}</span>
                </div>

                <div class="flex flex-col gap-2 items-end">
                  <span class="font-medium">${{ claim.totalAmount.toFixed(2) }}</span>
                  <UBadge variant="soft" color="neutral">Pending</UBadge>

                  <div class="flex gap-2 mt-2">
                    <UButton color="success" @click="approveClaim(claim.claimId)">
                      Approve
                    </UButton>
                    <UButton color="error" @click="rejectClaim(claim.claimId)">
                      Reject
                    </UButton>
                  </div>
                </div>
              </div>
            </UCard>
          </div>
          <span v-else>No Pending Claim</span>
        </UCard>
      </template>

      <!-- Aba: All Claims -->
      <template #all>
        <UCard>
          <h2 class="font-semibold text-xl mb-4">All Claims</h2>
          <div v-if="claims.length > 0" class="flex flex-col gap-4">
            <UCard v-for="claim in claims"
                   :key="claim.claimId">
              <div class="flex justify-between items-center">
                <div class="flex flex-col gap-1">
                  <span class="font-semibold">{{ claim.lecturerName }}</span>
                  <span class="text-sm text-gray-500">
                    Submitted: {{ new Date(claim.claimDate).toLocaleDateString() }}
                  </span>
                  <span class="text-sm text-gray-500">Hours: {{ claim.hoursWorked }}</span>
                  <span class="text-sm text-gray-500">Notes: {{ claim.notes }}</span>
                </div>

                <div class="flex flex-col gap-2 items-end">
                  <span class="font-medium">${{ claim.totalAmount.toFixed(2) }}</span>
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
