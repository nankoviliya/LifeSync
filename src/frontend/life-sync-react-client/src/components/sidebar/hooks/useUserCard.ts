import { useNavigate } from 'react-router-dom';

import { routePaths } from '@/config/routing/routePaths';
import { useLogout } from '@/hooks/auth/useLogout';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const useUserCard = () => {
  const navigate = useNavigate();
  const { translate } = useAppTranslations();
  const { logout } = useLogout();

  const navigateToUserProfile = () => {
    navigate(routePaths.userProfile.path);
  };

  const navigateToSettings = () => {
    navigate(routePaths.settings.path);
  };

  const handleLogout = () => {
    logout();
  };

  return {
    profileLabel: translate('profile-button-name', { defaultValue: 'Profile' }),
    settingsLabel: translate('settings-button-name', {
      defaultValue: 'Settings',
    }),
    logoutLabel: translate('logout-button-name', { defaultValue: 'Logout' }),
    navigateToUserProfile,
    navigateToSettings,
    handleLogout,
  };
};
