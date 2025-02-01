import { useUserProfile } from '@/features/userProfile/hooks/useUserProfile';
import styles from './UserProfile.module.scss';
import { Image } from 'primereact/image';
import { UserProfileDataContainer } from '@/features/userProfile/components/profileData/UserProfileDataContainer';

export const UserProfile = () => {
  const { isLoading, isSuccess, data } = useUserProfile();

  return (
    <div className={styles['user-profile']}>
      {isLoading && <p>Loading...</p>}
      {!isLoading && !data && <p>Unable to find user profile data</p>}
      {isSuccess && data && (
        <div className={styles['user-profile__info']}>
          <Image
            className={styles['user-profile__info__profile-icon']}
            src="/default-user-avatar.png"
            alt="user_avatar"
          />
          <UserProfileDataContainer userData={data} />
        </div>
      )}
    </div>
  );
};
