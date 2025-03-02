import { TransactionsSummaryItem } from '@/features/finances/transactions/components/TransactionsSummaryItem';
import {
  IExpenseSummaryData,
  IIncomeSummaryData,
} from '@/features/finances/transactions/models/transactionsGetModel';
import { useAppTranslations } from '@/hooks/useAppTranslations';

import styles from './TransactionsSummary.module.scss';

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
      className: styles['transaction-summary__section--total-needs'],
    },
    {
      label: 'expense-summary-total-spent-wants-label',
      value: totalSpentOnWants,
      className: styles['transaction-summary__section--total-wants'],
    },
    {
      label: 'expense-summary-total-spent-savings-label',
      value: totalSpentOnSavings,
      className: styles['transaction-summary__section--total-savings'],
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
    <div className={styles['transaction-summary']}>
      <div className={styles['transaction-summary__section']}>
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
      <div className={styles['transaction-summary__section']}>
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
