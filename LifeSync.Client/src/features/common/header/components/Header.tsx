import { routePaths } from '@/infrastructure/routing/routePaths';
import { Link, useNavigate } from 'react-router-dom';
import styles from './Header.module.scss';
import { useHeader } from '@/features/common/header/hooks/useHeader';
import { UserAvatar } from '@/features/common/header/components/UserAvatar';
import { HeaderUnauthenticatedButtons } from '@/features/common/header/components/HeaderUnauthenticatedButtons';

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
