import { Check, RefreshCw } from 'lucide-react';
import { Control, Controller } from 'react-hook-form';

import { Button } from '@/components/buttons/Button';
import { DatePicker } from '@/components/ui/date-picker';
import { Input } from '@/components/ui/input';
import { MultiSelect } from '@/components/ui/multi-select';
import { ITransactionsFiltersModel } from '@/features/finances/transactions/models/transactionsFiltersModel';
import {
  ExpenseType,
  TransactionType,
} from '@/features/finances/transactions/models/transactionsGetModel';
import { useAppTranslations } from '@/hooks/useAppTranslations';

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
    <div className="flex flex-col gap-4 p-4">
      <h2 className="text-lg font-semibold">{translate('filters-label')}</h2>

      <Controller
        name={'description'}
        control={control}
        render={({ field }) => (
          <div className="flex flex-col gap-1.5">
            <label htmlFor="description" className="text-sm font-medium">
              {translate('finances-filters-description-label')}
            </label>
            <Input
              id={field.name}
              value={field.value ?? ''}
              onChange={field.onChange}
              onBlur={field.onBlur}
              name={field.name}
              ref={field.ref}
              autoFocus
            />
          </div>
        )}
      />
      <Controller
        name={'startDate'}
        control={control}
        render={({ field }) => (
          <div className="flex flex-col gap-1.5">
            <label htmlFor="startDate" className="text-sm font-medium">
              {translate('finances-filters-start-date-label')}
            </label>
            <DatePicker value={field.value} onChange={field.onChange} />
          </div>
        )}
      />
      <Controller
        name={'endDate'}
        control={control}
        render={({ field }) => (
          <div className="flex flex-col gap-1.5">
            <label htmlFor="endDate" className="text-sm font-medium">
              {translate('finances-filters-end-date-label')}
            </label>
            <DatePicker value={field.value} onChange={field.onChange} />
          </div>
        )}
      />
      <Controller
        name={'transactionTypes'}
        control={control}
        render={({ field }) => (
          <div className="flex flex-col gap-1.5">
            <label htmlFor="transactionTypes" className="text-sm font-medium">
              {translate('finances-filters-transaction-types-label')}
            </label>
            <MultiSelect
              id={field.name}
              name={field.name}
              value={field.value ?? []}
              onChange={field.onChange}
              onBlur={field.onBlur}
              options={transactionTypeOptions}
            />
          </div>
        )}
      />
      <Controller
        name={'expenseTypes'}
        control={control}
        render={({ field }) => (
          <div className="flex flex-col gap-1.5">
            <label htmlFor="expenseTypes" className="text-sm font-medium">
              {translate('finances-filters-expense-types-label')}
            </label>
            <MultiSelect
              id={field.name}
              name={field.name}
              value={field.value ?? []}
              onChange={field.onChange}
              onBlur={field.onBlur}
              options={expenseTypeOptions}
            />
          </div>
        )}
      />

      <div className="mt-4 flex gap-2">
        <Button
          label={translate('filters-apply-button-label')}
          type="button"
          onClick={onFiltersApply}
          icon={<Check className="h-4 w-4" />}
          severity="success"
          rounded
        />
        <Button
          label={translate('filters-reset-button-label')}
          type="button"
          onClick={onFiltersReset}
          icon={<RefreshCw className="h-4 w-4" />}
          severity="secondary"
          rounded
        />
      </div>
    </div>
  );
};
