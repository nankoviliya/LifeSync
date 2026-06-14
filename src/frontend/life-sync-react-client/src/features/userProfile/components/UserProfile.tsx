import { AppShellHeader } from '@/components/layouts/AppShellHeader';
import { Skeleton } from '@/components/ui/skeleton';
import { ExportAccountData } from '@/features/userProfile/components/dataExport/ExportAccountData';
import { ImportAccountData } from '@/features/userProfile/components/dataImport/ImportAccountData';
import { UserProfileDataContainer } from '@/features/userProfile/components/profileData/UserProfileDataContainer';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useAuth } from '@/stores/AuthProvider';

export const UserProfileDataContainerSkeleton = () => (
  <div className="flex flex-col gap-2">
    <Skeleton className="h-12 w-12 rounded-full" />
    <Skeleton className="h-4 w-[200px]" />
  </div>
);

export const UserProfile = () => {
  const { isLoading, user } = useAuth();
  const { translate } = useAppTranslations();

  return (
    <>
      <AppShellHeader
        title={translate('page-profile-title', { defaultValue: 'Profile' })}
      />
      <div className="p-4">
        {isLoading && <UserProfileDataContainerSkeleton />}
        {!isLoading && user && (
          <div className="flex flex-col items-center gap-6 md:flex-row md:items-stretch">
            <UserProfileDataContainer userData={user} />
            <div className="flex flex-col gap-6 flex-1">
              <ExportAccountData />
              <ImportAccountData />
            </div>
          </div>
        )}
      </div>
    </>
  );
};
