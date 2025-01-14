import { useTranslation } from 'react-i18next';

export const useAppTranslations = () => {
  const { t, i18n } = useTranslation();

  return { translate: t, i18n };
};
