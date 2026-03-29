import { UserProfileIcon } from '@/features/userProfile/components/profileData/UserProfileIcon';
import { IProfileColor } from '@/features/userProfile/utils/profileColors';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { cn } from '@/lib/utils';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

interface UserProfileDataHeaderProps {
  initials: string;
  color: IProfileColor;
  userData: IUserProfileDataModel;
}

export const UserProfileDataHeader = ({
  initials,
  color,
  userData,
}: UserProfileDataHeaderProps) => {
  const { translate } = useAppTranslations();

  const { firstName, lastName, email, balanceAmount, balanceCurrency } =
    userData;

  return (
    <div className="flex flex-col gap-1">
      <UserProfileIcon initials={initials} color={color} />
      <p className="text-base font-medium mt-2">
        {firstName} {lastName}
      </p>
      <p className="text-sm text-muted-foreground">{email}</p>
      <div className={cn('rounded-lg px-4 py-3', color.bgLight)}>
        <p className={cn('text-xs mb-0.5', color.textLight)}>
          {translate('profile-current-balance-label')}
        </p>
        <p className={cn('text-xl font-medium', color.textLight)}>
          {balanceAmount} {balanceCurrency}
        </p>
      </div>
    </div>
  );
};
