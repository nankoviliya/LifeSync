import { Skeleton } from '@/components/ui/skeleton';
import { ExportAccountData } from '@/features/userProfile/components/dataExport/ExportAccountData';
import { ImportAccountData } from '@/features/userProfile/components/dataImport/ImportAccountData';
import { UserProfileDataContainer } from '@/features/userProfile/components/profileData/UserProfileDataContainer';
import { useAuth } from '@/stores/AuthProvider';

export const UserProfileDataContainerSkeleton = () => (
  <div className="flex flex-col gap-2">
    <Skeleton className="h-12 w-12 rounded-full" />
    <Skeleton className="h-4 w-[200px]" />
  </div>
);

export const UserProfile = () => {
  const { isLoading, user } = useAuth();

  return (
    <div className="p-4">
      {isLoading && <UserProfileDataContainerSkeleton />}
      {!isLoading && user && (
        <div className="flex flex-col items-center gap-6 md:flex-row md:items-start">
          <UserProfileDataContainer userData={user} />
          <div className="flex flex-col gap-6">
            <ExportAccountData />
            <ImportAccountData />
          </div>
        </div>
      )}
    </div>
  );
};
