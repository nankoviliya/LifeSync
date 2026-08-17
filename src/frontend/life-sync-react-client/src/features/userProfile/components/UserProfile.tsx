import { Pencil } from 'lucide-react';
import { useState } from 'react';

import { AppShellHeader } from '@/components/layouts/AppShellHeader';
import { Button } from '@/components/ui/button';
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
  const [isEditing, setIsEditing] = useState(false);

  return (
    <>
      <AppShellHeader
        title={translate('page-profile-title', { defaultValue: 'Profile' })}
        subtitle={translate('page-profile-subtitle', {
          defaultValue: 'Account',
        })}
        actions={
          !isEditing ? (
            <Button
              type="button"
              variant="secondary"
              onClick={() => setIsEditing(true)}
            >
              <Pencil className="h-4 w-4" />
              {translate('profile-edit-button', {
                defaultValue: 'Edit profile',
              })}
            </Button>
          ) : undefined
        }
      />
      <div className="p-1 md:p-2">
        {isLoading && <UserProfileDataContainerSkeleton />}
        {!isLoading && user && (
          <div className="flex flex-col gap-4 md:flex-row md:items-stretch">
            <UserProfileDataContainer
              userData={user}
              isEditing={isEditing}
              onEditingChange={setIsEditing}
            />
            <div className="flex flex-1 flex-col gap-4">
              <ExportAccountData />
              <ImportAccountData />
            </div>
          </div>
        )}
      </div>
    </>
  );
};
