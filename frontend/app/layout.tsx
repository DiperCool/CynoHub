import type { Metadata } from "next";
import { Inter } from "next/font/google";
import { Toaster } from 'sonner';
import ClientProviders from '@/lib/ClientProviders';
import Navbar from '@/components/Navbar';
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "CynoHub - Breeder Portal",
  description: "Manage your litters easily.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={inter.className}>
      <body className="bg-zinc-950 text-zinc-50 min-h-screen antialiased selection:bg-emerald-500/30">
        <ClientProviders>
          <Navbar />
          <main className="relative flex min-h-[calc(100vh-4rem)] flex-col">
            {children}
          </main>
          <Toaster theme="dark" position="bottom-right" />
        </ClientProviders>
      </body>
    </html>
  );
}
