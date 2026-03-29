import { Card, CardContent } from '@/components/ui/card';
import { UserProfileDataBody } from '@/features/userProfile/components/profileData/UserProfileDataBody';
import { UserProfileDataHeader } from '@/features/userProfile/components/profileData/UserProfileDataHeader';
import { getProfileColor } from '@/features/userProfile/utils/profileColors';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

interface IProps {
  userData: IUserProfileDataModel;
}

export const UserProfileDataContainer = ({ userData }: IProps) => {
  const { firstName, lastName } = userData;

  const initials =
    firstName && lastName
      ? `${firstName[0]}${lastName[0]}`.toUpperCase()
      : '..';

  const color = getProfileColor(initials);

  return (
    <Card className="w-[260px] self-stretch">
      <CardContent className="flex flex-col gap-4 pt-5 h-full">
        <UserProfileDataHeader
          initials={initials}
          color={color}
          userData={userData}
        />
        <UserProfileDataBody userData={userData} />
      </CardContent>
    </Card>
  );
};
