import { TransactionsSummaryItem } from '@/features/finances/transactions/components/TransactionsSummaryItem';
import {
  IExpenseSummaryData,
  IIncomeSummaryData,
} from '@/features/finances/transactions/models/transactionsGetModel';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export interface ITransactionsSummaryProps {
  expenseSummaryData: IExpenseSummaryData;
  incomeSummaryData: IIncomeSummaryData;
}

export const TransactionsSummary = ({
  expenseSummaryData,
  incomeSummaryData,
}: ITransactionsSummaryProps) => {
  const { translate } = useAppTranslations();

  const {
    totalSpent,
    totalSpentOnNeeds,
    totalSpentOnSavings,
    totalSpentOnWants,
    currency: expenseCurrency,
  } = expenseSummaryData;

  const { totalIncome, currency: incomeCurrency } = incomeSummaryData;

  const expenseSummaryItems = [
    {
      label: 'expense-summary-total-spent-label',
      value: totalSpent,
      className: '',
    },
    {
      label: 'expense-summary-total-spent-needs-label',
      value: totalSpentOnNeeds,
      className: 'text-emerald-600',
    },
    {
      label: 'expense-summary-total-spent-wants-label',
      value: totalSpentOnWants,
      className: 'text-red-500',
    },
    {
      label: 'expense-summary-total-spent-savings-label',
      value: totalSpentOnSavings,
      className: 'text-primary',
    },
  ];

  const incomeSummaryItems = [
    {
      label: 'income-summary-total-income-label',
      value: totalIncome,
      className: '',
    },
  ];

  return (
    <div className="inline-flex flex-col rounded-md px-4 py-2 text-sm shadow-md">
      <div className="inline-flex flex-row gap-4">
        {expenseSummaryItems.map(({ label, value, className }) => (
          <TransactionsSummaryItem
            key={label}
            label={translate(label)}
            value={value}
            currency={expenseCurrency}
            className={className}
          />
        ))}
      </div>
      <div className="inline-flex flex-row gap-4">
        {incomeSummaryItems.map(({ label, value, className }) => (
          <TransactionsSummaryItem
            key={label}
            label={translate(label)}
            value={value}
            currency={incomeCurrency}
            className={className}
          />
        ))}
      </div>
    </div>
  );
};
