import { useQuery } from '@tanstack/react-query';
import { littersApi } from '@/lib/api/litters';
import { queryKeys } from '@/lib/api/queryKeys';
import { LITTERS_PAGE_SIZE } from '@/lib/constants';

export function useLitters(page: number, statusFilter?: string) {
  return useQuery({
    queryKey: queryKeys.litters.list(page, statusFilter),
    queryFn: () => littersApi.getLitters(page, LITTERS_PAGE_SIZE, statusFilter || undefined),
  });
}
