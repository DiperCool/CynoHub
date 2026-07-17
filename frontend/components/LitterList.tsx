'use client';

import React, { useState } from 'react';
import { useLitters } from '@/lib/hooks/useLitters';
import { usePublishLitter } from '@/lib/hooks/usePublishLitter';
import { LITTERS_DEFAULT_PAGE } from '@/lib/constants';

import { LitterHeader } from './litters/LitterHeader';
import { LitterTable } from './litters/LitterTable';
import { LitterPagination } from './litters/LitterPagination';

export default function LitterList() {
  const [page, setPage] = useState(LITTERS_DEFAULT_PAGE);
  const [statusFilter, setStatusFilter] = useState<string>('');

  const { data, isLoading, refetch, isFetching } = useLitters(page, statusFilter);
  const publishMutation = usePublishLitter();

  const litters = data?.data ?? [];
  const meta = data?.pagination;

  return (
    <div className="w-full max-w-6xl mx-auto p-6 space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <LitterHeader
        statusFilter={statusFilter}
        setStatusFilter={(val) => {
          setStatusFilter(val);
          setPage(LITTERS_DEFAULT_PAGE);
        }}
        isLoading={isLoading || isFetching}
        onRefresh={() => refetch()}
      />

      <div className="rounded-xl border border-zinc-800 bg-zinc-900/50 backdrop-blur-xl overflow-hidden shadow-2xl">
        <LitterTable
          litters={litters}
          isLoading={isLoading}
          onPublish={(id) => publishMutation.mutate(id)}
          isPublishing={publishMutation.isPending}
          publishingId={publishMutation.variables ?? null}
        />

        {meta && (
          <LitterPagination
            meta={meta}
            setPage={setPage}
            isFetching={isFetching}
          />
        )}
      </div>
    </div>
  );
}
