import { UserProfileIcon } from '@/features/userProfile/components/profileData/UserProfileIcon';
import { IProfileColor } from '@/features/userProfile/utils/profileColors';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';
import { formatCurrency } from '@/utils/formatCurrency';

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
      <div className="rounded-lg bg-primary-soft px-4 py-3">
        <p className="mb-0.5 font-mono text-[11px] uppercase tracking-[0.08em] text-primary-soft-foreground">
          {translate('profile-current-balance-label')}
        </p>
        <p className="text-2xl font-semibold tracking-[-0.02em] tabular-nums text-primary-soft-foreground">
          {formatCurrency(balanceAmount, balanceCurrency)}
        </p>
      </div>
    </div>
  );
};
