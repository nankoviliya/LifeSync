import { ExpenseSummaryItem } from '@/features/finances/expenseTracking/components/summary/ExpenseSummaryItem';
import { IExpenseSummaryData } from '@/features/finances/expenseTracking/models/expenseTransactionModel';
import { useAppTranslations } from '@/hooks/useAppTranslations';

import styles from './ExpenseSummary.module.scss';

export interface IExpenseSummaryProps {
  expenseSummaryData: IExpenseSummaryData;
  transactionsCount: number;
}

export const ExpenseSummary = ({
  expenseSummaryData,
  transactionsCount,
}: IExpenseSummaryProps) => {
  const { translate } = useAppTranslations();

  const {
    totalSpent,
    totalSpentOnNeeds,
    totalSpentOnSavings,
    totalSpentOnWants,
    currency,
  } = expenseSummaryData;

  const summaryItems = [
    {
      label: 'expense-summary-total-spent-label',
      value: totalSpent,
      className: '',
    },
    {
      label: 'expense-summary-total-spent-needs-label',
      value: totalSpentOnNeeds,
      className: styles['expense-summary--total-needs'],
    },
    {
      label: 'expense-summary-total-spent-wants-label',
      value: totalSpentOnWants,
      className: styles['expense-summary--total-wants'],
    },
    {
      label: 'expense-summary-total-spent-savings-label',
      value: totalSpentOnSavings,
      className: styles['expense-summary--total-savings'],
    },
  ];

  return (
    <div className={styles['expense-summary']}>
      <p>
        {translate('expense-summary-transactions-count-label')}:{' '}
        {transactionsCount}
      </p>

      {summaryItems.map(({ label, value, className }) => (
        <ExpenseSummaryItem
          key={label}
          label={translate(label)}
          value={value}
          currency={currency}
          className={className}
        />
      ))}
    </div>
  );
};
