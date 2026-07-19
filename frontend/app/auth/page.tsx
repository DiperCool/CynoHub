import { cookies } from 'next/headers';
import AuthClient from './AuthClient';

export default async function AuthPage() {
  const cookieStore = await cookies();
  const breederId = cookieStore.get('breederId')?.value ?? '';
  
  return <AuthClient initialBreederId={breederId} />;
}
