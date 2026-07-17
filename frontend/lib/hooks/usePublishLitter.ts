import { useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { toast } from 'sonner';
import { littersApi } from '@/lib/api/litters';
import { queryKeys } from '@/lib/api/queryKeys';

export function usePublishLitter() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => littersApi.publishLitter(id),
    onSuccess: () => {
      toast.success('Litter published successfully!');
      void queryClient.invalidateQueries({ queryKey: queryKeys.litters.all });
    },
    onError: (err: Error) => {
      const error = err as AxiosError<{ detail?: string }>;
      toast.error('Failed to publish litter', {
        description: error.response?.data?.detail || error.message,
      });
    },
  });
}
