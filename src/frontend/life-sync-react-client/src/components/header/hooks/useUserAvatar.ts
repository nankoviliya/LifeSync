import { useNavigate } from 'react-router-dom';

import { routePaths } from '@/config/routing/routePaths';
import { useLogout } from '@/hooks/auth/useLogout';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const useUserAvatar = () => {
  const navigate = useNavigate();
  const { translate } = useAppTranslations();

  const { logout } = useLogout();

  const navigateToUserProfile = () => {
    navigate(routePaths.userProfile.path);
  };

  const handleLogout = () => {
    logout();
  };

  return {
    profileLabel: translate('profile-button-name'),
    logoutLabel: translate('logout-button-name'),
    navigateToUserProfile,
    handleLogout,
  };
};
