import { Link } from 'react-router-dom';

import { TransactionRow } from '@/components/composites/TransactionRow';
import { routePaths } from '@/config/routing/routePaths';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import {
  IExpenseTransactionGetModel,
  IIncomeTransactionGetModel,
} from '@/hooks/useTransactions';
import { TransactionType } from '@/types/transactionTypes';
import { formatSignedCurrency } from '@/utils/formatCurrency';

export interface RecentTransactionsCardProps {
  transactions: Array<IExpenseTransactionGetModel | IIncomeTransactionGetModel>;
}

export const RecentTransactionsCard = ({
  transactions,
}: RecentTransactionsCardProps) => {
  const { translate } = useAppTranslations();
  const recent = transactions.slice(0, 5);

  return (
    <div className="rounded-xl border border-border bg-card p-5 text-card-foreground shadow-xs md:col-span-4 md:row-span-2">
      <div className="mb-2 flex items-baseline justify-between">
        <div className="text-[13px] font-semibold">
          {translate('dashboard-recent-title', {
            defaultValue: 'Recent transactions',
          })}
        </div>
        <Link
          to={routePaths.finances.path}
          className="text-[11.5px] text-primary hover:underline"
        >
          {translate('dashboard-recent-all', { defaultValue: 'All →' })}
        </Link>
      </div>
      {recent.length === 0 ? (
        <div className="py-6 text-center text-[12.5px] text-muted-foreground">
          {translate('dashboard-recent-empty', {
            defaultValue: 'No transactions this month yet.',
          })}
        </div>
      ) : (
        recent.map((tx, i) => {
          const isExpense = tx.transactionType === TransactionType.Expense;
          const category = isExpense
            ? (tx as IExpenseTransactionGetModel).expenseType
            : translate('money-tab-income', { defaultValue: 'Income' });
          return (
            <TransactionRow
              key={tx.id}
              first={i === 0}
              title={tx.description}
              meta={tx.date}
              category={category}
              amount={formatSignedCurrency(
                isExpense ? -tx.amount : tx.amount,
                tx.currency,
              )}
              positive={!isExpense}
            />
          );
        })
      )}
    </div>
  );
};
