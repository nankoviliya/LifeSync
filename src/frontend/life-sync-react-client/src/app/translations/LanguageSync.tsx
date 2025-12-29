import { useState, useEffect, PropsWithChildren } from 'react';

import { SkeletonLoader } from '@/components/loaders/SkeletonLoader';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useAuth } from '@/stores/AuthProvider';

// We use this Guard component to verify that user-related language
// is loaded and we don't see this language-switch on UI
export const LanguageSync = ({ children }: PropsWithChildren) => {
  const { user, isLoading } = useAuth();
  const { i18n } = useAppTranslations();
  const [ready, setReady] = useState(false);

  useEffect(() => {
    if (isLoading) return;

    setReady(false);

    const targetLang = user?.language.code ?? 'en';

    if (i18n.language === targetLang) {
      setReady(true);
    } else {
      i18n.changeLanguage(targetLang).then(() => setReady(true));
    }
  }, [user, isLoading, i18n]);

  if (!ready) return <SkeletonLoader />;

  return <>{children}</>;
};
