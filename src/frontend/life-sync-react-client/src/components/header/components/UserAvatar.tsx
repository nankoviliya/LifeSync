import { User } from 'lucide-react';

import { useUserAvatar } from '@/components/header/hooks/useUserAvatar';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

export const UserAvatar = () => {
  const { profileLabel, logoutLabel, navigateToUserProfile, handleLogout } =
    useUserAvatar();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="icon" className="rounded-full">
          <User className="h-5 w-5" />
          <span className="sr-only">User avatar</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuItem onClick={navigateToUserProfile}>
          {profileLabel}
        </DropdownMenuItem>
        <DropdownMenuItem onClick={handleLogout}>
          {logoutLabel}
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
