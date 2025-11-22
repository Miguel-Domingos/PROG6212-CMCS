<script setup lang="ts">
  import { ref, watch, computed, defineEmits } from 'vue'
  import { useApi } from '@/composables/useApi'
  import { useAuthStore } from "@/stores/auth";

  const open = ref(false)
  const claimTitle = ref('')
  const notes = ref('')
  const hoursWorked = ref<number>(1)
  const hourlyRate = ref<number>(10)
  const files = ref<File[]>([])
  const loading = ref(false)
  const uploadedFile = ref<File | null>(null)
  const emits = defineEmits(['created'])

  const MAX_FILE_SIZE = 2 * 1024 * 1024 // 2MB

  const canSubmit = computed(()=> !!claimTitle.value && !!notes.value && !!hourlyRate.value
  && !!hoursWorked.value)
  const api = useApi()
  const {user} = useAuthStore()

  async function submitClaim() {
    try {
      const claim = await api('/claims', {
        method: 'POST',
        body: {
          title: claimTitle.value,
          notes: notes.value,
          hoursWorked: hoursWorked.value,
          hourlyRate: hourlyRate.value,
          LecturerId: user.lecturerProfile.lecturerId,
        }
      })

      const claimId = claim.ClaimId || claim.claimId

      if (uploadedFile.value) {
        const formData = new FormData();
        formData.append("file", uploadedFile.value);

        await api(`/SupportingDocuments/${claimId}`, {
          method: 'POST',
          body: formData,
          headers: {
            "Content-Type": "multipart/form-data"
          }
        });
      }

      emits('created')
      closeModal()
      useToast().add({
        title: "Claim Submitted",
        description: "Your expense claim has been submitted successfully.",
        color: "success",
      })

    } catch (err) {
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  function closeModal() {
    open.value = false
    claimTitle.value = ''
    notes.value = ''
    hourlyRate.value = null
    hoursWorked.value = null
    files.value = []
    uploadedFile.value = null
  }



  watch(uploadedFile, ()=>{{
    if(uploadedFile.value) {
      console.log("File ready for upload:", uploadedFile.value)
       if (uploadedFile.value.size > MAX_FILE_SIZE) {
        uploadedFile.value = null
        useToast().add({
          title: "File too large",
          description: "The file size exceeds 2MB limit.",
          color: "error",
        })
      }
    }
  }})
</script>

<template>
  <UModal v-model:open="open" title="Submit new Claim"
          description="Fill in the details for your expense claim">
    <UButton color="neutral" variant="solid" icon="i-lucide-plus" label="New Claim" size="lg" />

    <template #body>
      <div class="w-full flex flex-col gap-4">
        <UFormField label="Claim Title">
          <UInput v-model="claimTitle" class="w-full" />
        </UFormField>

        <UFormField label="Notes">
          <UTextarea v-model="notes" class="w-full" />
        </UFormField>

        <UFormField label="Worked Hours">
          <UInputNumber v-model="hoursWorked" :min="1" :max="5000" orientation="vertical" class="w-full" />
        </UFormField>

        <UFormField label="Hours Rate">
          <UInputNumber v-model="hourlyRate" :min="1" :max="5000" orientation="vertical" class="w-full" />
        </UFormField>

        <UFormField label="Documents">
          <UFileUpload accept=".pdf,.docx,.doc,.xlsx"
                       layout="list"
                       label="Drop your file here"
                       description="PDF, DOC, DOCX or XLSX (max. 2MB)"
                       class="w-full min-h-20"
                       v-model="uploadedFile" />
        </UFormField>

        <div class="w-full flex gap-2">
          <UButton :disabled="!canSubmit" :loading="loading" @click="submitClaim" color="neutral" variant="solid" label="Submit Claim" class="w-full flex justify-center text-center" />
          <UButton @click="closeModal" color="neutral" variant="outline" label="Cancel" />
        </div>
      </div>
    </template>
  </UModal>
</template>
