import { useState } from 'react';

import { TransactionRow } from '@/components/composites/TransactionRow';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import {
  IExpenseTransactionGetModel,
  IIncomeTransactionGetModel,
} from '@/hooks/useTransactions';
import { ExpenseType, TransactionType } from '@/types/transactionTypes';
import { formatSignedCurrency } from '@/utils/formatCurrency';

type AnyTransaction = IExpenseTransactionGetModel | IIncomeTransactionGetModel;

export interface MoneyTransactionListProps {
  transactions: AnyTransaction[];
}

type TabKey = 'all' | 'income' | 'expenses' | 'savings';

const filterByTab = (txns: AnyTransaction[], tab: TabKey): AnyTransaction[] => {
  switch (tab) {
    case 'income':
      return txns.filter((t) => t.transactionType === TransactionType.Income);
    case 'expenses':
      return txns.filter((t) => t.transactionType === TransactionType.Expense);
    case 'savings':
      return txns.filter(
        (t) =>
          t.transactionType === TransactionType.Expense &&
          (t as IExpenseTransactionGetModel).expenseType ===
            ExpenseType.Savings,
      );
    case 'all':
    default:
      return txns;
  }
};

export const MoneyTransactionList = ({
  transactions,
}: MoneyTransactionListProps) => {
  const { translate } = useAppTranslations();
  const [tab, setTab] = useState<TabKey>('all');

  const visible = filterByTab(transactions, tab);

  return (
    <div>
      <div className="mb-2.5 flex flex-wrap items-center justify-between gap-2">
        <Tabs value={tab} onValueChange={(v) => setTab(v as TabKey)}>
          <TabsList>
            <TabsTrigger value="all">
              {translate('money-tab-all', { defaultValue: 'All' })}
            </TabsTrigger>
            <TabsTrigger value="income">
              {translate('money-tab-income', { defaultValue: 'Income' })}
            </TabsTrigger>
            <TabsTrigger value="expenses">
              {translate('money-tab-expenses', { defaultValue: 'Expenses' })}
            </TabsTrigger>
            <TabsTrigger value="savings">
              {translate('money-tab-savings', { defaultValue: 'Savings' })}
            </TabsTrigger>
          </TabsList>
        </Tabs>
        <div className="text-xs text-muted-foreground">
          {visible.length}{' '}
          {translate('money-transactions-count-suffix', {
            defaultValue: 'transactions',
          })}
        </div>
      </div>

      <div className="rounded-xl border border-border bg-card p-[18px] text-card-foreground shadow-xs">
        {visible.map((tx, i) => {
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
        })}
      </div>
    </div>
  );
};
