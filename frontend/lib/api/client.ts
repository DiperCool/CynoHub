import axios from 'axios';

import Cookies from 'js-cookie';

export const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7052',
});

apiClient.interceptors.request.use((config) => {
  let breederId: string | undefined = undefined;

  if (typeof window !== 'undefined') {
    breederId = Cookies.get('breederId');
  }

  if (breederId) {
    config.headers['X-Breeder-Id'] = breederId;
  }
  return config;
});