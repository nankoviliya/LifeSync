import { Menu } from 'primereact/menu';
import { MenuItem } from 'primereact/menuitem';
import { useRef } from 'react';
import { useNavigate } from 'react-router-dom';

import { routePaths } from '@/config/routing/routePaths';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useAuth } from '@/hooks/useAuthentication';

export const useUserAvatar = () => {
  const navigate = useNavigate();
  const { translate } = useAppTranslations();

  const { logout } = useAuth();

  const navigateToUserProfile = () => {
    navigate(routePaths.userProfile.path);
  };

  const avatarMenuRef = useRef<Menu>(null);

  const handleLogout = async () => {
    await logout();
    navigate(routePaths.login.path);
  };

  const avatarMenuItems: MenuItem[] = [
    {
      label: translate('profile-button-name'),
      command: navigateToUserProfile,
    },
    {
      label: translate('logout-button-name'),
      command: handleLogout,
    },
  ];

  const toggleAvatarMenu = (e: React.MouseEvent<HTMLElement>) => {
    avatarMenuRef.current?.toggle(e);
  };

  return {
    avatarMenuRef,
    avatarMenuItems,
    toggleAvatarMenu,
  };
};
