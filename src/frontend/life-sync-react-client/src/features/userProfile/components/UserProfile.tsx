import { ExportAccountData } from '@/features/userProfile/components/dataExport/ExportAccountData';
import { UserDataImport } from '@/features/userProfile/components/dataImport/UserDataImport';
import { UserProfileDataContainer } from '@/features/userProfile/components/profileData/UserProfileDataContainer';
import { useAuth } from '@/stores/AuthProvider';

export const UserProfile = () => {
  const { isLoading, isAuthenticated, user } = useAuth();

  return (
    <div className="p-4">
      {isLoading && <p>Loading...</p>}
      {!isLoading && !user && <p>Unable to find user profile data</p>}
      {isAuthenticated && user && (
        <div className="flex flex-col items-center gap-6 md:flex-row md:items-start">
          <UserProfileDataContainer userData={user} />
          <div className="flex flex-col gap-6">
            <ExportAccountData />
            <UserDataImport />
          </div>
        </div>
      )}
    </div>
  );
};
