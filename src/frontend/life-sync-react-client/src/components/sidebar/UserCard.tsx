import { useUserCard } from '@/components/sidebar/hooks/useUserCard';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

export interface UserCardProps {
  user: { name: string; email: string; initials: string };
}

export const UserCard = ({ user }: UserCardProps) => {
  const { profileLabel, logoutLabel, navigateToUserProfile, handleLogout } =
    useUserCard();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button
          type="button"
          className="flex w-full items-center gap-2.5 rounded-md px-1.5 py-2 text-left transition-colors hover:bg-side-accent-soft"
        >
          <span className="grid size-7 shrink-0 place-items-center rounded-full bg-side-accent-soft text-[11px] font-medium text-side-foreground">
            {user.initials}
          </span>
          <span className="min-w-0 flex-1">
            <span className="block truncate text-[12px] font-medium text-side-foreground">
              {user.name}
            </span>
            <span className="block truncate text-[10px] text-side-muted">
              {user.email}
            </span>
          </span>
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent side="top" align="start" className="w-56">
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
