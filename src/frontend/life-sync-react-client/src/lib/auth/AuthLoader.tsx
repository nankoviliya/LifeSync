import { useSuspenseQuery } from '@tanstack/react-query';
import type { AxiosError } from 'axios';

import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { get } from '@/lib/apiClient';
import type { IUserProfileDataModel } from '@/types/userProfileDataModel';

interface AuthLoaderProps {
  children: React.ReactNode;
}

export const AuthLoader = ({ children }: AuthLoaderProps) => {
  const { data: user } = useSuspenseQuery<IUserProfileDataModel, AxiosError>({
    queryKey: [endpointsOptions.getUserAccountData.key],
    queryFn: () =>
      get<IUserProfileDataModel>(endpointsOptions.getUserAccountData.endpoint),
    staleTime: 86_400_000,
  });

  const { i18n } = useAppTranslations();

  const languageCode = user.language.code;

  if (i18n.language !== languageCode) {
    throw i18n.changeLanguage(languageCode);
  }

  return <>{children}</>;
};
