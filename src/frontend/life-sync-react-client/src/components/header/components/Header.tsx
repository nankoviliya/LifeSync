import { useNavigate } from 'react-router-dom';

import { HeaderUnauthenticatedButtons } from '@/components/header/components/HeaderUnauthenticatedButtons';
import { ThemeToggle } from '@/components/header/components/ThemeToggle';
import { UserAvatar } from '@/components/header/components/UserAvatar';
import { useHeader } from '@/components/header/hooks/useHeader';
import { routePaths } from '@/config/routing/routePaths';

import styles from './Header.module.scss';

export const Header = () => {
  const navigate = useNavigate();
  const { isUserAuthenticated } = useHeader();

  const handleKeyDown = (e: React.KeyboardEvent<HTMLDivElement>) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      navigate(routePaths.home.path);
    }
  };

  return (
    <header className={styles['header']}>
      <div
        className={styles['header__logo']}
        onClick={() => {
          navigate(routePaths.home.path);
        }}
        role="button"
        tabIndex={0}
        onKeyDown={handleKeyDown}
      >
        <img src="app.ico" alt="Life Sync logo" />
        <span>Life Sync</span>
      </div>
      <div className={styles['header__actions']}>
        <ThemeToggle />
        {isUserAuthenticated && <UserAvatar />}
        {!isUserAuthenticated && <HeaderUnauthenticatedButtons />}
      </div>
    </header>
  );
};
