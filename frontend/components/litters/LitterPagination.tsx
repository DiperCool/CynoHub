import React from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { PaginationMeta } from '@/lib/interfaces/litter';
import { LITTERS_DEFAULT_PAGE } from '@/lib/constants';

interface LitterPaginationProps {
  meta: PaginationMeta;
  setPage: (page: number) => void;
  isFetching: boolean;
}

export function LitterPagination({ meta, setPage, isFetching }: LitterPaginationProps) {
  if (meta.totalPages <= LITTERS_DEFAULT_PAGE) return null;

  return (
    <div className="flex items-center justify-between px-6 py-4 border-t border-zinc-800 bg-zinc-900/50">
      <div className="text-sm text-zinc-400">
        Showing <span className="font-medium text-white">{(meta.pageNumber - 1) * meta.pageSize + 1}</span> to <span className="font-medium text-white">{Math.min(meta.pageNumber * meta.pageSize, meta.totalCount)}</span> of <span className="font-medium text-white">{meta.totalCount}</span> results
      </div>
      <div className="flex items-center gap-2">
        <button
          onClick={() => setPage(Math.max(LITTERS_DEFAULT_PAGE, meta.pageNumber - 1))}
          disabled={!meta.hasPreviousPage || isFetching}
          className="inline-flex h-8 w-8 items-center justify-center rounded-md border border-zinc-800 bg-zinc-900 text-zinc-400 hover:text-white hover:bg-zinc-800 disabled:opacity-50 disabled:pointer-events-none transition-colors"
        >
          <ChevronLeft className="w-4 h-4" />
        </button>
        <div className="text-sm font-medium text-white">
          Page {meta.pageNumber} of {meta.totalPages}
        </div>
        <button
          onClick={() => setPage(Math.min(meta.totalPages, meta.pageNumber + 1))}
          disabled={!meta.hasNextPage || isFetching}
          className="inline-flex h-8 w-8 items-center justify-center rounded-md border border-zinc-800 bg-zinc-900 text-zinc-400 hover:text-white hover:bg-zinc-800 disabled:opacity-50 disabled:pointer-events-none transition-colors"
        >
          <ChevronRight className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
}
