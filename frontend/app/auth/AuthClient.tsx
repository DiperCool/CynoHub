'use client';

import { useState, type SubmitEvent } from 'react';
import { KeyRound, ShieldAlert, CheckCircle2, Copy, Check } from 'lucide-react';
import { useQueryClient } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';
import { toast } from 'sonner';
import Cookies from 'js-cookie';

export default function AuthClient({ initialBreederId }: { initialBreederId: string }) {
  const [breederId, setBreederId] = useState(initialBreederId);
  const [inputValue, setInputValue] = useState(initialBreederId);
  const [isCopied, setIsCopied] = useState(false);
  
  const queryClient = useQueryClient();
  const router = useRouter();

  const handleCopy = () => {
    if (process.env.NEXT_PUBLIC_BREEDER_ID) {
      navigator.clipboard.writeText(process.env.NEXT_PUBLIC_BREEDER_ID);
      setIsCopied(true);
      toast.success('Mock ID copied to clipboard');
      setTimeout(() => setIsCopied(false), 2000);
    }
  };

  const handleSave = (e: SubmitEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (inputValue.trim()) {
      const newId = inputValue.trim();
      setBreederId(newId);
      Cookies.set('breederId', newId, { expires: 365, path: '/' });
      // Invalidate queries to fetch new data for the new breederId
      queryClient.invalidateQueries();
      // Redirect back to home/my litters after auth
      router.push('/');
    }
  };

  return (
    <div className="relative min-h-[calc(100vh-4rem)] flex items-center justify-center p-4">
      {/* Background Gradients */}
      <div className="absolute inset-0 z-0 pointer-events-none overflow-hidden flex items-center justify-center">
        <div className="absolute w-160 h-160 bg-emerald-500/10 rounded-full blur-[128px] -top-20 -left-20" />
        <div className="absolute w-160 h-160 bg-purple-500/10 rounded-full blur-[128px] bottom-0 right-0" />
      </div>

      <div className="relative z-10 w-full max-w-md overflow-hidden rounded-2xl bg-zinc-900/80 backdrop-blur-xl border border-zinc-800 shadow-2xl animate-in zoom-in-95 duration-700 mx-auto">
        {/* Card Header Effect */}
        <div className="absolute top-0 left-0 w-full h-1 bg-linear-to-r from-emerald-500 to-purple-500" />
        
        <div className="p-8 sm:p-10">
          <div className="flex justify-center mb-8">
            <div className="p-4 bg-emerald-500/10 rounded-full ring-1 ring-emerald-500/30">
              <KeyRound className="w-10 h-10 text-emerald-400" />
            </div>
          </div>
          
          <div className="text-center mb-10">
            <h2 className="text-3xl font-bold text-white mb-3">
              {breederId ? 'Change Breeder ID' : 'Welcome to CynoHub'}
            </h2>
            <p className="text-zinc-400 text-base">
              Please enter your Breeder ID to access and manage your litters securely.
              {process.env.NEXT_PUBLIC_BREEDER_ID && (
                <span className="flex items-center justify-center gap-2 mt-3 text-sm text-emerald-400/80">
                  <span>Mock ID for testing:</span>
                  <button 
                    type="button"
                    onClick={handleCopy}
                    title="Copy Mock ID"
                    className="group flex items-center gap-1.5 font-mono bg-emerald-500/10 hover:bg-emerald-500/20 px-2 py-1 rounded border border-emerald-500/20 transition-all active:scale-95"
                  >
                    <span>{process.env.NEXT_PUBLIC_BREEDER_ID}</span>
                    {isCopied ? (
                      <Check className="w-3.5 h-3.5 text-emerald-400" />
                    ) : (
                      <Copy className="w-3.5 h-3.5 opacity-70 group-hover:opacity-100 transition-opacity" />
                    )}
                  </button>
                </span>
              )}
            </p>
          </div>

          <form onSubmit={handleSave} className="space-y-6">
            <div>
              <div className="relative group">
                <input
                  type="text"
                  value={inputValue}
                  onChange={(e) => setInputValue(e.target.value)}
                  placeholder="e.g. B-123456"
                  className="w-full bg-zinc-950/50 border border-zinc-700 rounded-xl px-5 py-4 pl-12 text-white placeholder:text-zinc-600 focus:outline-none focus:ring-2 focus:ring-emerald-500/50 focus:border-emerald-500 transition-all shadow-inner text-lg"
                  autoFocus
                />
                <ShieldAlert className="absolute left-4 top-4 w-6 h-6 text-zinc-500 group-focus-within:text-emerald-500 transition-colors" />
              </div>
            </div>

            <button
              type="submit"
              disabled={!inputValue.trim()}
              className="w-full flex items-center justify-center gap-2 px-6 py-4 bg-emerald-500 hover:bg-emerald-400 text-zinc-950 rounded-xl font-bold text-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed hover:shadow-lg hover:shadow-emerald-500/20"
            >
              <span>{breederId ? 'Update ID & Proceed' : 'Access Portal'}</span>
              <CheckCircle2 className="w-6 h-6" />
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
