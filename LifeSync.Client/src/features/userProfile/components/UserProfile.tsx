import { useUserProfile } from '@/features/userProfile/hooks/useUserProfile';
import styles from './UserProfile.module.scss';
import { Image } from 'primereact/image';

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
          <div className={styles['user-profile__info__profile-data']}>
            <span>User ID: {data.userId}</span>
            <span>Username: {data.userName}</span>
            <span>Email: {data.email}</span>
            <span>First Name: {data.firstName}</span>
            <span>Last Name: {data.lastName}</span>
            <span>
              Balance: {data.balanceAmount} {data.balanceCurrency}
            </span>
          </div>
        </div>
      )}
    </div>
  );
};
