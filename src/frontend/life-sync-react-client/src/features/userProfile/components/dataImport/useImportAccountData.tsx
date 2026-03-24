import { useMutation } from '@tanstack/react-query';

import { endpoints } from '@/config/endpoints/endpoints';
import { post } from '@/lib/apiClient';

export const useImportAccountData = (onSuccess?: () => void) => {
  const mutation = useMutation({
    mutationFn: async ({ file, format }: { file: File; format: string }) => {
      const formData = new FormData();
      formData.append('file', file);
      formData.append('format', format);
      return post<any, FormData>(endpoints.account.importAccountData, formData);
    },
    onSuccess: () => {
      onSuccess?.();
    },
    onError: () => {
      console.log('Import error');
    },
  });

  return {
    importData: mutation.mutate,
    isImporting: mutation.isPending,
  };
};
