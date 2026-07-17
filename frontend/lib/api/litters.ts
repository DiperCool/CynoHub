import { apiClient } from './client';
import { LitterDtoPagedResult } from '@/lib/interfaces';
import { LITTERS_DEFAULT_PAGE, LITTERS_PAGE_SIZE } from '../constants';

export const littersApi = {
  getLitters: async (pageNumber: number = LITTERS_DEFAULT_PAGE, pageSize: number = LITTERS_PAGE_SIZE, status?: string) => {
    const params = new URLSearchParams();
    params.append('PageNumber', pageNumber.toString());
    params.append('PageSize', pageSize.toString());
    if (status) {
      params.append('status', status);
    }
    const response = await apiClient.get<LitterDtoPagedResult>(`/api/litters?${params.toString()}`);
    return response.data;
  },
  publishLitter: async (litterId: string) => {
    const response = await apiClient.post(`/api/litters/${litterId}/publish`);
    return response.data;
  }
};
