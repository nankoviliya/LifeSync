import { UserProfileIcon } from '@/features/userProfile/components/profileData/UserProfileIcon';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

import { UserProfileData } from '@/features/userProfile/components/profileData/UserProfileData';

interface IProps {
  userData: IUserProfileDataModel;
}

export const UserProfileDataContainer = ({ userData }: IProps) => {
  const { firstName, lastName } = userData;

  return (
    <div className="w-[260px] rounded-xl border bg-card p-5 shadow-sm flex flex-col gap-4">
      <UserProfileIcon userFirstName={firstName} userLastName={lastName} />
      <UserProfileData userData={userData} />
    </div>
  );
};
