import LitterList from "@/components/LitterList";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";

export default async function Home() {
  const cookieStore = await cookies();
  const breederId = cookieStore.get('breederId')?.value;

  if (!breederId) {
    redirect('/auth');
  }

  return (
    <div className="relative min-h-screen w-full flex flex-col items-center py-12 px-4 sm:px-6 lg:px-8 overflow-hidden">
      {/* Background Gradients */}
      <div className="absolute inset-0 z-0 pointer-events-none">
        <div className="absolute top-0 left-1/4 w-96 h-96 bg-emerald-500/10 rounded-full blur-[128px]" />
        <div className="absolute bottom-0 right-1/4 w-96 h-96 bg-purple-500/10 rounded-full blur-[128px]" />
      </div>

      {/* Main Content */}
      <div className="z-10 w-full flex flex-col items-center">
        <div className="mb-12 text-center animate-in slide-in-from-top-8 fade-in duration-700">
          <div className="inline-flex items-center justify-center px-4 py-1.5 mb-6 text-sm font-medium text-emerald-400 bg-emerald-400/10 rounded-full ring-1 ring-inset ring-emerald-400/20 backdrop-blur-sm">
            <span>Breeder Portal v1.0</span>
          </div>
          <h1 className="text-4xl md:text-6xl font-extrabold tracking-tight text-transparent bg-clip-text bg-linear-to-br from-white to-zinc-500 mb-4">
            Manage Litters <br /> with CynoHub
          </h1>
          <p className="text-zinc-400 max-w-xl mx-auto text-lg md:text-xl">
            A secure and intuitive platform to organize, track, and publish your litters to the world.
          </p>
        </div>

        <LitterList />
      </div>
    </div>
  );
}
