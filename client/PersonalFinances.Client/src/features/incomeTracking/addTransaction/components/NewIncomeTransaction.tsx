import { Control, Controller } from 'react-hook-form';
import styles from './NewIncomeTransaction.module.scss';
import { InputNumber } from 'primereact/inputnumber';
import { classNames } from 'primereact/utils';
import { InputText } from 'primereact/inputtext';
import { Calendar } from 'primereact/calendar';
import { Button } from 'primereact/button';
import { parseCalendarDate } from '@/infrastructure/common/utilities/utilities';
import { INewIncomeTransactionRequest } from '@/features/incomeTracking/addTransaction/models/newIncomeTransactionRequest';

export interface INewExpenseTransactionProps {
  control: Control<INewIncomeTransactionRequest>;
  handleSubmit: (e?: React.BaseSyntheticEvent) => Promise<void>;
}

export const NewIncomeTransaction = ({
  control,
  handleSubmit,
}: INewExpenseTransactionProps) => {
  return (
    <form className={styles['form']} onSubmit={handleSubmit}>
      <Controller
        name={'amount'}
        control={control}
        rules={{ required: 'Amount is required.' }}
        render={({ field, fieldState }) => (
          <>
            <label htmlFor="amount">Amount</label>
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
            <label htmlFor="currency">Currency</label>
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
            <label htmlFor="currency">Date</label>
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
            <label htmlFor="currency">Description</label>
            <InputText
              id={field.name}
              {...field}
              autoFocus
              className={classNames({ 'p-invalid': fieldState.invalid })}
            />
          </>
        )}
      />

      <Button label="Submit" type="submit" />
    </form>
  );
};
