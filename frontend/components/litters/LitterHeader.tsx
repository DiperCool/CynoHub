import React from 'react';
import { RefreshCw } from 'lucide-react';
import { LitterStatus } from '@/lib/interfaces/litter';

interface LitterHeaderProps {
  statusFilter: string;
  setStatusFilter: (status: string) => void;
  isLoading: boolean;
  onRefresh: () => void;
}

export function LitterHeader({ statusFilter, setStatusFilter, isLoading, onRefresh }: LitterHeaderProps) {
  return (
    <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
      <div>
        <h1 className="text-3xl font-bold tracking-tight text-white">Litters</h1>
        <p className="text-zinc-400 mt-1">Manage your litters and publish them when ready.</p>
      </div>
      <div className="flex items-center gap-3">
        <select 
          className="h-10 rounded-md border border-zinc-800 bg-zinc-900 px-3 py-2 text-sm text-white focus:outline-none focus:ring-2 focus:ring-emerald-500"
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
        >
          <option value="">All Statuses</option>
          <option value={LitterStatus.Draft}>Draft</option>
          <option value={LitterStatus.Submitted}>Submitted</option>
          <option value={LitterStatus.Approved}>Approved</option>
          <option value={LitterStatus.Published}>Published</option>
        </select>
        <button 
          onClick={onRefresh}
          className="inline-flex h-10 items-center justify-center rounded-md border border-zinc-800 bg-zinc-900 px-4 text-sm font-medium text-white transition-colors hover:bg-zinc-800 focus:outline-none focus:ring-2 focus:ring-emerald-500"
        >
          <RefreshCw className={`w-4 h-4 mr-2 ${isLoading ? 'animate-spin' : ''}`} />
          Refresh
        </button>
      </div>
    </div>
  );
}
