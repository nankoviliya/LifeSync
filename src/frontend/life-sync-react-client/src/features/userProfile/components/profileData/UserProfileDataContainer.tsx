import { useState } from 'react';

import { UserProfileDataEditable } from '@/features/userProfile/components/profileData/UserProfileDataEditable';
import { UserProfileDataReadonly } from '@/features/userProfile/components/profileData/UserProfileDataReadonly';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

import styles from './UserProfileDataContainer.module.scss';

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
      className={styles['user-profile-data-container']}
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
