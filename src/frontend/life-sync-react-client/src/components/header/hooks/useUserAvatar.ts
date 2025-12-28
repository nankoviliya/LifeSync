import { Menu } from 'primereact/menu';
import { MenuItem } from 'primereact/menuitem';
import { useRef } from 'react';
import { useNavigate } from 'react-router-dom';

import { routePaths } from '@/config/routing/routePaths';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useLogout } from '@/hooks/useLogout';

export const useUserAvatar = () => {
  const navigate = useNavigate();
  const { translate } = useAppTranslations();

  const { logout } = useLogout();

  const navigateToUserProfile = () => {
    navigate(routePaths.userProfile.path);
  };

  const avatarMenuRef = useRef<Menu>(null);

  const handleLogout = () => {
    logout();
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
