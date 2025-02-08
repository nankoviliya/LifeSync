import { useEffect } from 'react';

import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useUserProfile } from '@/hooks/useUserProfile';

export const useCurrentLanguage = () => {
  const { data, isLoading, isSuccess } = useUserProfile();

  const { i18n } = useAppTranslations();

  useEffect(() => {
    if (data && !isLoading && isSuccess) {
      const languageCode = data.language.code;

      i18n.changeLanguage(languageCode);
    }
  }, [data, isLoading, isSuccess, i18n]);

  return {
    currentLanguage: data?.language,
    isLoading,
    isSuccess,
  };
};
