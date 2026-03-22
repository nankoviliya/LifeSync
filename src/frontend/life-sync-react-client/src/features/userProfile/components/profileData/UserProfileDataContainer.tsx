import { Card, CardContent } from '@/components/ui/card';
import { UserProfileData } from '@/features/userProfile/components/profileData/UserProfileData';
import { UserProfileIcon } from '@/features/userProfile/components/profileData/UserProfileIcon';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

interface IProps {
  userData: IUserProfileDataModel;
}

export const UserProfileDataContainer = ({ userData }: IProps) => {
  const { firstName, lastName } = userData;

  return (
    <Card className="w-[260px]">
      <CardContent className="flex flex-col gap-4 pt-5">
        <UserProfileIcon userFirstName={firstName} userLastName={lastName} />
        <UserProfileData userData={userData} />
      </CardContent>
    </Card>
  );
};
