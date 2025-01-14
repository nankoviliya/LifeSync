import { initReactI18next } from 'react-i18next';
import i18n from 'i18next';
import HttpBackend, { HttpBackendOptions } from 'i18next-http-backend';
import { appLanguages } from '@/infrastructure/translations/data/appLanguages';

i18n
  .use(initReactI18next)
  .use(HttpBackend)
  .init<HttpBackendOptions>({
    lng: appLanguages.english,
    fallbackLng: appLanguages.english,
    supportedLngs: Object.values(appLanguages),
    ns: ['serviceNames'],
    backend: {
      // Path to load translation JSON files stored locally
      loadPath: '/locales/{{lng}}/{{ns}}.json',
    },
    load: 'currentOnly',
    debug: false,
    keySeparator: false,
    interpolation: {
      escapeValue: false,
    },
  });

export default i18n;
