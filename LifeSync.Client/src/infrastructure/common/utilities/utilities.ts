import { appConstants } from '@/infrastructure/common/models/appConstants';
import { Nullable } from 'primereact/ts-helpers';

export function parseCalendarDate(localDate: Nullable<Date>): Date | null {
  if (!localDate) {
    return null;
  }

  try {
    const utcDate = new Date(
      Date.UTC(
        localDate.getFullYear(),
        localDate.getMonth(),
        localDate.getDate(),
        0,
        0,
        0,
      ),
    );

    return utcDate;
  } catch (error) {
    console.error(`Error parsing calendar date: ${error}`);

    return null;
  }
}

export const replaceIdPlaceholderWithId = (id: string, textToEdit: string) => {
  return textToEdit.replace(appConstants.idPlaceholder, id);
};
