import { useState } from 'react';

import { UserProfileDataEditable } from '@/features/userProfile/components/profileData/UserProfileDataEditable';
import { UserProfileDataReadonly } from '@/features/userProfile/components/profileData/UserProfileDataReadonly';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

export interface IUserProfileDataContainerProps {
  userData: IUserProfileDataModel;
}

export const UserProfileDataContainer = ({
  userData,
}: IUserProfileDataContainerProps) => {
  const [isEditing, setIsEditing] = useState<boolean>(false);

  const enableEditMode = () => {
    setIsEditing(true);
  };

  const disableEditMode = () => {
    setIsEditing(false);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLDivElement>) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      enableEditMode();
    }
  };

  return (
    <div
      className="inline-flex cursor-text flex-col rounded border border-transparent p-4 transition-colors hover:border-border"
      onClick={enableEditMode}
      role="button"
      tabIndex={0}
      onKeyDown={handleKeyDown}
    >
      {isEditing ? (
        <UserProfileDataEditable
          userData={userData}
          disableEditMode={disableEditMode}
        />
      ) : (
        <UserProfileDataReadonly userData={userData} />
      )}
    </div>
  );
};
