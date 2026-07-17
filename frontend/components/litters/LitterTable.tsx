import React from 'react';
import { format } from 'date-fns';
import { Loader2, Plus } from 'lucide-react';
import { LitterDto, LitterStatus } from '@/lib/interfaces/litter';
import { DATE_FORMAT } from '@/lib/constants';
import { LitterStatusBadge } from './LitterStatusBadge';

interface LitterTableProps {
  litters: LitterDto[];
  isLoading: boolean;
  onPublish: (id: string) => void;
  isPublishing: boolean;
  publishingId: string | null;
}

export function LitterTable({ litters, isLoading, onPublish, isPublishing, publishingId }: LitterTableProps) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-sm text-left">
        <thead className="text-xs text-zinc-400 uppercase bg-zinc-900 border-b border-zinc-800">
          <tr>
            <th scope="col" className="px-6 py-4 font-medium">Litter ID</th>
            <th scope="col" className="px-6 py-4 font-medium">Status</th>
            <th scope="col" className="px-6 py-4 font-medium">Created At</th>
            <th scope="col" className="px-6 py-4 font-medium text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          {isLoading ? (
            <tr>
              <td colSpan={4} className="px-6 py-12 text-center">
                <Loader2 className="w-6 h-6 text-emerald-500 animate-spin mx-auto" />
                <p className="text-zinc-400 mt-2">Loading litters...</p>
              </td>
            </tr>
          ) : litters.length === 0 ? (
            <tr>
              <td colSpan={4} className="px-6 py-12 text-center">
                <p className="text-zinc-400">No litters found.</p>
              </td>
            </tr>
          ) : (
            litters.map((litter) => (
              <tr key={litter.id} className="border-b border-zinc-800/50 hover:bg-zinc-800/20 transition-colors group">
                <td className="px-6 py-4 font-mono text-xs text-zinc-300">
                  {litter.id}
                </td>
                <td className="px-6 py-4">
                  <LitterStatusBadge status={litter.status} />
                </td>
                <td className="px-6 py-4 text-zinc-400">
                  {format(new Date(litter.createdAt), DATE_FORMAT)}
                </td>
                <td className="px-6 py-4 text-right">
                  {litter.status === LitterStatus.Approved && (
                    <button
                      onClick={() => onPublish(litter.id)}
                      disabled={isPublishing && publishingId === litter.id}
                      className="inline-flex h-8 items-center justify-center rounded-md bg-emerald-600/10 border border-emerald-500/20 px-3 text-xs font-medium text-emerald-400 transition-colors hover:bg-emerald-600/20 hover:text-emerald-300 focus:outline-none focus:ring-2 focus:ring-emerald-500 disabled:opacity-50 disabled:pointer-events-none"
                    >
                      {isPublishing && publishingId === litter.id ? (
                        <Loader2 className="w-3 h-3 mr-1.5 animate-spin" />
                      ) : (
                        <Plus className="w-3 h-3 mr-1.5" />
                      )}
                      Publish
                    </button>
                  )}
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
