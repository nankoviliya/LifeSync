import { useNavigate } from 'react-router-dom';

import { HeaderUnauthenticatedButtons } from '@/components/header/components/HeaderUnauthenticatedButtons';
import { UserAvatar } from '@/components/header/components/UserAvatar';
import { useHeader } from '@/components/header/hooks/useHeader';
import { routePaths } from '@/config/routing/routePaths';

import styles from './Header.module.scss';

export const Header = () => {
  const navigate = useNavigate();
  const { isUserAuthenticated } = useHeader();

  return (
    <header className={styles['header']}>
      <div
        className={styles['header__logo']}
        onClick={() => {
          navigate(routePaths.home.path);
        }}
      >
        <img src="app.ico" />
        <span>Life Sync</span>
      </div>
      <div>
        {isUserAuthenticated && <UserAvatar />}
        {!isUserAuthenticated && <HeaderUnauthenticatedButtons />}
      </div>
    </header>
  );
};
