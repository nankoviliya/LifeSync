import { Avatar, AvatarFallback } from '@radix-ui/react-avatar';

import { cn } from '@/lib/utils';

export interface IUserProfileIconProps {
  userFirstName: string;
  userLastName: string;
}

const COLORS = [
  { bg: 'bg-red-500', text: 'text-red-100' },
  { bg: 'bg-blue-500', text: 'text-blue-100' },
  { bg: 'bg-green-500', text: 'text-green-100' },
  { bg: 'bg-yellow-400', text: 'text-yellow-100' },
  { bg: 'bg-purple-500', text: 'text-purple-100' },
  { bg: 'bg-pink-500', text: 'text-pink-100' },
  { bg: 'bg-indigo-500', text: 'text-indigo-100' },
];

function getColor(initials: string) {
  const hash = initials.charCodeAt(0) + initials.charCodeAt(1);
  return COLORS[hash % COLORS.length];
}

export const UserProfileIcon = ({
  userFirstName,
  userLastName,
}: IUserProfileIconProps) => {
  const initials =
    userFirstName && userLastName
      ? `${userFirstName[0]}${userLastName[0]}`.toUpperCase()
      : '..';

  const { bg, text } = getColor(initials);

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
