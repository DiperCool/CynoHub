import React from 'react';
import { Send, CheckCircle, FileText } from 'lucide-react';
import { LitterStatus } from '@/lib/interfaces/litter';

export function LitterStatusBadge({ status }: { status: LitterStatus }) {
  const getStatusIcon = (s: LitterStatus) => {
    switch (s) {
      case LitterStatus.Draft: return <FileText className="w-4 h-4 text-zinc-400" />;
      case LitterStatus.Submitted: return <Send className="w-4 h-4 text-blue-400" />;
      case LitterStatus.Approved: return <CheckCircle className="w-4 h-4 text-emerald-400" />;
      case LitterStatus.Published: return <CheckCircle className="w-4 h-4 text-purple-400" />;
      default: return null;
    }
  };

  const getStatusBadgeClass = (s: LitterStatus) => {
    switch (s) {
      case LitterStatus.Draft: return 'bg-zinc-500/10 text-zinc-400 border-zinc-500/20';
      case LitterStatus.Submitted: return 'bg-blue-500/10 text-blue-400 border-blue-500/20';
      case LitterStatus.Approved: return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20';
      case LitterStatus.Published: return 'bg-purple-500/10 text-purple-400 border-purple-500/20';
      default: return 'bg-zinc-500/10 text-zinc-400 border-zinc-500/20';
    }
  };

  return (
    <div className={`inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium border ${getStatusBadgeClass(status)}`}>
      <span className="mr-1.5">{getStatusIcon(status)}</span>
      {status}
    </div>
  );
}
