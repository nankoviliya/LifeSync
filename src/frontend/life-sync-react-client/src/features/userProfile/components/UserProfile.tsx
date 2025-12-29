import { Image } from 'primereact/image';

import { UserProfileDataContainer } from '@/features/userProfile/components/profileData/UserProfileDataContainer';
import { useAuth } from '@/stores/AuthProvider';

import styles from './UserProfile.module.scss';

export const UserProfile = () => {
  const { isLoading, isAuthenticated, user } = useAuth();

  return (
    <div className={styles['user-profile']}>
      {isLoading && <p>Loading...</p>}
      {!isLoading && !user && <p>Unable to find user profile data</p>}
      {isAuthenticated && user && (
        <div className={styles['user-profile__info']}>
          <Image
            className={styles['user-profile__info__profile-icon']}
            src="/default-user-avatar.png"
            alt="user_avatar"
          />
          <UserProfileDataContainer userData={user} />
        </div>
      )}
    </div>
  );
};
