import { useAuth } from '@/infrastructure/authentication/hooks/useAuthentication';
import { routePaths } from '@/infrastructure/routing/routePaths';
import { useAppTranslations } from '@/infrastructure/translations/hooks/useAppTranslations';
import { Menu } from 'primereact/menu';
import { MenuItem } from 'primereact/menuitem';
import { useRef } from 'react';
import { useNavigate } from 'react-router-dom';

export const useUserAvatar = () => {
  const navigate = useNavigate();
  const { translate } = useAppTranslations();

  const { logout } = useAuth();

  const navigateToUserProfile = () => {
    navigate(routePaths.userProfile.path);
  };

  const avatarMenuRef = useRef<Menu>(null);

  const avatarMenuItems: MenuItem[] = [
    {
      label: translate('profile-button-name'),
      command: navigateToUserProfile,
    },
    {
      label: translate('logout-button-name'),
      command: logout,
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
