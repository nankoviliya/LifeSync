import { Button } from 'primereact/button';
import { Calendar } from 'primereact/calendar';
import { Dropdown } from 'primereact/dropdown';
import { InputNumber } from 'primereact/inputnumber';
import { InputText } from 'primereact/inputtext';
import { classNames } from 'primereact/utils';
import { Control, Controller } from 'react-hook-form';

import {
  ExpenseType,
  INewExpenseTransactionRequest,
} from '@/features/finances/transactions/models/newExpenseTransactionRequest';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { parseCalendarDate } from '@/utils/utilities';

import styles from './NewExpenseTransaction.module.scss';

export interface INewExpenseTransactionProps {
  control: Control<INewExpenseTransactionRequest>;
  handleSubmit: (e?: React.BaseSyntheticEvent) => Promise<void>;
}

export const NewExpenseTransaction = ({
  control,
  handleSubmit,
}: INewExpenseTransactionProps) => {
  const { translate } = useAppTranslations();

  const expenseTypeOptions = Object.values(ExpenseType).map((type) => ({
    label: type,
    value: type,
  }));

  return (
    <form className={styles['form']} onSubmit={handleSubmit}>
      <Controller
        name={'amount'}
        control={control}
        rules={{ required: 'Amount is required.' }}
        render={({ field, fieldState }) => (
          <>
            <label htmlFor="amount">
              {translate('new-transaction-input-amount-label')}
            </label>
            <InputNumber
              id={field.name}
              ref={field.ref}
              value={field.value}
              onBlur={field.onBlur}
              onValueChange={(e) => field.onChange(e)}
              className={classNames({ 'p-invalid': fieldState.invalid })}
            />
          </>
        )}
      />
      <Controller
        name={'currency'}
        control={control}
        rules={{ required: 'Currency is required.' }}
        render={({ field, fieldState }) => (
          <>
            <label htmlFor="currency">
              {translate('new-transaction-input-currency-label')}
            </label>
            <InputText
              id={field.name}
              {...field}
              autoFocus
              className={classNames({ 'p-invalid': fieldState.invalid })}
            />
          </>
        )}
      />
      <Controller
        name={'date'}
        control={control}
        rules={{ required: 'Date is required.' }}
        render={({ field, fieldState }) => (
          <>
            <label htmlFor="currency">
              {translate('new-transaction-input-date-label')}
            </label>
            <Calendar
              id={field.name}
              {...field}
              className={classNames({ 'p-invalid': fieldState.invalid })}
              onChange={(e) => {
                const utcDate = parseCalendarDate(e.value);
                field.onChange(utcDate);
              }}
            />
          </>
        )}
      />
      <Controller
        name={'description'}
        control={control}
        rules={{ required: 'Description is required.' }}
        render={({ field, fieldState }) => (
          <>
            <label htmlFor="currency">
              {translate('new-transaction-input-description-label')}
            </label>
            <InputText
              id={field.name}
              {...field}
              autoFocus
              className={classNames({ 'p-invalid': fieldState.invalid })}
            />
          </>
        )}
      />
      <Controller
        name={'expenseType'}
        control={control}
        rules={{ required: 'Expense type is required.' }}
        render={({ field, fieldState }) => (
          <>
            <label htmlFor="expenseType">
              {translate('new-expense-transaction-input-expense-type-label')}
            </label>
            <Dropdown
              id={field.name}
              {...field}
              options={expenseTypeOptions}
              className={classNames({ 'p-invalid': fieldState.invalid })}
            />
          </>
        )}
      />

      <Button label="Submit" type="submit" />
    </form>
  );
};
