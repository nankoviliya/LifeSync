import { appLanguages } from '@/types/appLanguages';

export function getLanguageTranslation(
  languageCode: string,
  translate: (key: string) => string,
): string {
  const entry = Object.entries(appLanguages).find(
    ([, code]) => code === languageCode,
  );

  if (!entry) return `Unsupported language code: ${languageCode}`;

  return translate(`application-language-${entry[0]}`);
}
