import { useUserProfile } from '@/features/userProfile/hooks/useUserProfile';
import i18n from '@/infrastructure/translations/i18n';
import { useEffect } from 'react';

export const useCurrentLanguage = () => {
  const { data, isLoading, isSuccess } = useUserProfile();

  useEffect(() => {
    if (data && !isLoading && isSuccess) {
      const languageCode = data.language.code;

      i18n.changeLanguage(languageCode);
    }
  }, [data, isLoading, isSuccess]);

  return {
    currentLanguage: data?.language,
    isLoading,
    isSuccess,
  };
};
