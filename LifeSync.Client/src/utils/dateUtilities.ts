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

export function getCurrentMonthFirstDayDate(): Date {
  const todayDate = new Date();

  return new Date(todayDate.getFullYear(), todayDate.getMonth(), 1);
}

export function getCurrentMonthLastDayDate(): Date {
  const todayDate = new Date();

  return new Date(todayDate.getFullYear(), todayDate.getMonth() + 1, 0);
}
