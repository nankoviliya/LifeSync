import { Controller } from 'react-hook-form';

import { Button } from '@/components/buttons/Button';
import { DatePicker } from '@/components/ui/date-picker';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { useNewExpenseTransaction } from '@/features/finances/transactions/hooks/useNewExpenseTransaction';
import { ExpenseType } from '@/features/finances/transactions/models/transactionsGetModel';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export interface INewExpenseTransactionProps {
  closeForm: () => void;
}

export const NewExpenseTransaction = ({
  closeForm,
}: INewExpenseTransactionProps) => {
  const { translate } = useAppTranslations();

  const { control, isSubmitting, handleSubmit } =
    useNewExpenseTransaction(closeForm);

  const expenseTypeOptions = Object.values(ExpenseType).map((type) => ({
    label: type,
    value: type,
  }));

  return (
    <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
      <Controller
        name={'amount'}
        control={control}
        rules={{ required: 'Amount is required.' }}
        render={({ field, fieldState }) => (
          <>
            <label htmlFor="amount">
              {translate('new-transaction-input-amount-label')}
            </label>
            <Input
              id={field.name}
              type="number"
              value={field.value ?? ''}
              onChange={(e) =>
                field.onChange(e.target.value ? Number(e.target.value) : null)
              }
              onBlur={field.onBlur}
              ref={field.ref}
              aria-invalid={fieldState.invalid}
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
            <Input
              id={field.name}
              {...field}
              autoFocus
              aria-invalid={fieldState.invalid}
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
            <label htmlFor="date">
              {translate('new-transaction-input-date-label')}
            </label>
            <DatePicker
              value={field.value}
              onChange={field.onChange}
              aria-invalid={fieldState.invalid}
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
            <label htmlFor="description">
              {translate('new-transaction-input-description-label')}
            </label>
            <Input
              id={field.name}
              {...field}
              aria-invalid={fieldState.invalid}
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
            <Select
              value={field.value}
              onValueChange={field.onChange}
            >
              <SelectTrigger
                className="w-full"
                aria-invalid={fieldState.invalid}
              >
                <SelectValue placeholder="Select expense type" />
              </SelectTrigger>
              <SelectContent>
                {expenseTypeOptions.map((opt) => (
                  <SelectItem key={opt.value} value={opt.value}>
                    {opt.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </>
        )}
      />

      <Button label="Submit" type="submit" loading={isSubmitting} />
    </form>
  );
};
