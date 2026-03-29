import { Avatar, AvatarFallback } from '@radix-ui/react-avatar';

import { IProfileColor } from '@/features/userProfile/utils/profileColors';
import { cn } from '@/lib/utils';

export interface IUserProfileIconProps {
  initials: string;
  color: IProfileColor;
}

export const UserProfileIcon = ({ initials, color }: IUserProfileIconProps) => {
  const { bg, text } = color;

  return (
    <Avatar className="h-14 w-14 flex rounded-full overflow-hidden">
      <AvatarFallback
        className={cn(
          'w-full h-full flex items-center justify-center rounded-full text-xl font-bold',
          bg,
          text,
        )}
      >
        {initials}
      </AvatarFallback>
    </Avatar>
  );
};
