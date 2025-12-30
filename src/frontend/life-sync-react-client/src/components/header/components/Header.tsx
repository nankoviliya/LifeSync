import { useNavigate } from 'react-router-dom';

import { HeaderUnauthenticatedButtons } from '@/components/header/components/HeaderUnauthenticatedButtons';
import { ThemeToggle } from '@/components/header/components/ThemeToggle';
import { UserAvatar } from '@/components/header/components/UserAvatar';
import { useHeader } from '@/components/header/hooks/useHeader';
import { routePaths } from '@/config/routing/routePaths';

export const Header = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useHeader();

  const handleKeyDown = (e: React.KeyboardEvent<HTMLDivElement>) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      navigate(routePaths.home.path);
    }
  };

  return (
    <header className="mb-4 flex h-12 w-full select-none items-center justify-between px-8 pt-4">
      <div
        className="inline-flex cursor-pointer items-center gap-4"
        onClick={() => {
          navigate(routePaths.home.path);
        }}
        role="button"
        tabIndex={0}
        onKeyDown={handleKeyDown}
      >
        <img
          src="app.ico"
          alt="Life Sync logo"
          className="h-12 w-12 rounded-full object-cover"
        />
        <span className="font-['Montserrat',sans-serif] text-2xl text-foreground">
          Life Sync
        </span>
      </div>
      <div className="flex items-center gap-2">
        <ThemeToggle />
        {isAuthenticated && <UserAvatar />}
        {!isAuthenticated && <HeaderUnauthenticatedButtons />}
      </div>
    </header>
  );
};
