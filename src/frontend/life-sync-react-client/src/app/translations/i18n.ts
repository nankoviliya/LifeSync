import i18n from 'i18next';
import HttpBackend, { HttpBackendOptions } from 'i18next-http-backend';

import { appLanguages } from '@/app/translations/data/appLanguages';
import { environment } from '@/config/currentEnvironment';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';

i18n.use(HttpBackend).init<HttpBackendOptions>({
  lng: appLanguages.english,
  fallbackLng: appLanguages.english,
  supportedLngs: Object.values(appLanguages),
  backend: {
    loadPath: `${environment.xApiUrl}${endpointsOptions.getTranslations.endpoint}?languageCode={{lng}}`,
  },
  load: 'currentOnly',
  debug: false,
  keySeparator: false,
  interpolation: {
    escapeValue: false,
  },
});

export default i18n;
