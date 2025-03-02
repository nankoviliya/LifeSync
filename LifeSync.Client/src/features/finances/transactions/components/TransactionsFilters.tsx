import { Button } from 'primereact/button';
import { Calendar } from 'primereact/calendar';
import { InputText } from 'primereact/inputtext';
import { MultiSelect } from 'primereact/multiselect';
import { Control, Controller } from 'react-hook-form';

import { ITransactionsFiltersModel } from '@/features/finances/transactions/models/transactionsFiltersModel';
import {
  ExpenseType,
  TransactionType,
} from '@/features/finances/transactions/models/transactionsGetModel';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { parseCalendarDate } from '@/utils/utilities';

import styles from './TransactionsFilters.module.scss';

export interface ITransactionsFiltersProps {
  control: Control<ITransactionsFiltersModel>;
  onFiltersApply: () => void;
  resetFilters: () => Promise<void>;
}

export const TransactionsFilters = ({
  control,
  onFiltersApply,
  resetFilters,
}: ITransactionsFiltersProps) => {
  const { translate } = useAppTranslations();

  const transactionTypeOptions = Object.values(TransactionType).map((type) => ({
    label: type,
    value: type,
  }));

  const expenseTypeOptions = Object.values(ExpenseType).map((type) => ({
    label: type,
    value: type,
  }));

  const onFiltersReset = async () => {
    await resetFilters();
    onFiltersApply();
  };

  return (
    <div className={styles['transactions-filters']}>
      <h2>{translate('filters-label')}</h2>

      <Controller
        name={'description'}
        control={control}
        render={({ field }) => (
          <>
            <label htmlFor="description">
              {translate('finances-filters-description-label')}
            </label>
            <InputText id={field.name} {...field} autoFocus />
          </>
        )}
      />
      <Controller
        name={'startDate'}
        control={control}
        render={({ field }) => (
          <>
            <label htmlFor="startDate">
              {translate('finances-filters-start-date-label')}
            </label>
            <Calendar
              id={field.name}
              {...field}
              onChange={(e) => {
                const utcDate = parseCalendarDate(e.value);
                field.onChange(utcDate);
              }}
            />
          </>
        )}
      />
      <Controller
        name={'endDate'}
        control={control}
        render={({ field }) => (
          <>
            <label htmlFor="endDate">
              {translate('finances-filters-end-date-label')}
            </label>
            <Calendar
              id={field.name}
              {...field}
              onChange={(e) => {
                const utcDate = parseCalendarDate(e.value);
                field.onChange(utcDate);
              }}
            />
          </>
        )}
      />
      <Controller
        name={'transactionTypes'}
        control={control}
        render={({ field }) => (
          <>
            <label htmlFor="transactionTypes">
              {translate('finances-filters-transaction-types-label')}
            </label>
            <MultiSelect
              id={field.name}
              {...field}
              options={transactionTypeOptions}
              autoFocus
            />
          </>
        )}
      />
      <Controller
        name={'expenseTypes'}
        control={control}
        render={({ field }) => (
          <>
            <label htmlFor="expenseTypes">
              {translate('finances-filters-expense-types-label')}
            </label>
            <MultiSelect
              id={field.name}
              {...field}
              options={expenseTypeOptions}
              autoFocus
            />
          </>
        )}
      />

      <div className={styles['transactions-filters__control-buttons']}>
        <Button
          label={translate('filters-apply-button-label')}
          type="button"
          onClick={onFiltersApply}
          icon="pi pi-check"
          severity="success"
          rounded
        />
        <Button
          label={translate('filters-reset-button-label')}
          type="button"
          onClick={onFiltersReset}
          icon="pi pi-refresh"
          severity="secondary"
          rounded
        />
      </div>
    </div>
  );
};
