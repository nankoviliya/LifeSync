import { useAppTranslations } from '@/hooks/useAppTranslations';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

export interface IUserProfileDataReadonlyProps {
  userData: IUserProfileDataModel;
}

export const UserProfileDataReadonly = ({
  userData,
}: IUserProfileDataReadonlyProps) => {
  const { translate } = useAppTranslations();

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
    <div className="inline-flex flex-col gap-4">
      <span>
        {translate('profile-user-id-label')}: {userId}
      </span>
      <span>
        {translate('profile-username-label')}: {userName}
      </span>
      <span>
        {translate('profile-email-label')}: {email}
      </span>
      <span>
        {translate('profile-first-name-label')}: {firstName}
      </span>
      <span>
        {translate('profile-last-name-label')}: {lastName}
      </span>
      <span>
        {translate('profile-balance-label')}: {balanceAmount} {balanceCurrency}
      </span>
      <span>
        {translate('profile-language-label')}: {language.name}
      </span>
    </div>
  );
};
