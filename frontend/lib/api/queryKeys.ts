export const queryKeys = {
  litters: {
    all: ['litters'] as const,
    list: (page: number, status?: string) => ['litters', page, status] as const,
  },
} as const;
