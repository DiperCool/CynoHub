import axios from 'axios';

export const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7052',
});

apiClient.interceptors.request.use((config) => {
  const breederId = process.env.NEXT_PUBLIC_BREEDER_ID;
  if (breederId) {
    config.headers['X-Breeder-Id'] = breederId;
  }
  return config;
});