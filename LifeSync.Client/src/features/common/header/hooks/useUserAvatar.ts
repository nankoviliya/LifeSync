import { useAuth } from '@/infrastructure/authentication/hooks/useAuthentication';
import { routePaths } from '@/infrastructure/routing/routePaths';
import { Menu } from 'primereact/menu';
import { MenuItem } from 'primereact/menuitem';
import { useRef } from 'react';
import { useNavigate } from 'react-router-dom';

export const useUserAvatar = () => {
  const navigate = useNavigate();

  const { logout } = useAuth();

  const navigateToUserProfile = () => {
    navigate(routePaths.userProfile.path);
  };

  const avatarMenuRef = useRef<Menu>(null);

  const avatarMenuItems: MenuItem[] = [
    {
      label: 'Profile',
      command: navigateToUserProfile,
    },
    {
      label: 'Logout',
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
