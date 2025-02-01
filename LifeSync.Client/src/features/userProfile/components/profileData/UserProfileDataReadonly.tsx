import { IUserProfileDataModel } from '@/features/userProfile/models/userProfileDataModel';
import styles from './UserProfileDataReadonly.module.scss';

export interface IUserProfileDataReadonlyProps {
  userData: IUserProfileDataModel;
}

export const UserProfileDataReadonly = ({
  userData,
}: IUserProfileDataReadonlyProps) => {
  const {
    userId,
    userName,
    email,
    firstName,
    lastName,
    balanceAmount,
    balanceCurrency,
    language,
  } = userData;

  return (
    <div className={styles['user-profile-data']}>
      <span>User ID: {userId}</span>
      <span>Username: {userName}</span>
      <span>Email: {email}</span>
      <span>First Name: {firstName}</span>
      <span>Last Name: {lastName}</span>
      <span>
        Balance: {balanceAmount} {balanceCurrency}
      </span>
      <span>Language: {language.name}</span>
    </div>
  );
};
