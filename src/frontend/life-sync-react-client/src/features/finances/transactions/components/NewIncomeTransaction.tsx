import { Calendar } from 'primereact/calendar';
import { InputNumber } from 'primereact/inputnumber';
import { InputText } from 'primereact/inputtext';
import { classNames } from 'primereact/utils';
import { Controller } from 'react-hook-form';

import { Button } from '@/components/buttons/Button';
import { useNewIncomeTransaction } from '@/features/finances/transactions/hooks/useNewIncomeTransaction';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { parseCalendarDate } from '@/utils/dateUtilities';

import styles from './NewIncomeTransaction.module.scss';

export interface INewExpenseTransactionProps {
  closeForm: () => void;
}

export const NewIncomeTransaction = ({
  closeForm,
}: INewExpenseTransactionProps) => {
  const { translate } = useAppTranslations();

  const { control, isSubmitting, handleSubmit } =
    useNewIncomeTransaction(closeForm);
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

      <Button label="Submit" type="submit" loading={isSubmitting} />
    </form>
  );
};
