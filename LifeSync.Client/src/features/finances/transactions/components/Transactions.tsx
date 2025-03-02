import { NewTransactionButtons } from '@/features/finances/transactions/components/NewTransactionButtons';
import { TransactionItem } from '@/features/finances/transactions/components/TransactionItem';
import { TransactionsFilters } from '@/features/finances/transactions/components/TransactionsFilters';
import { TransactionsSummary } from '@/features/finances/transactions/components/TransactionsSummary';
import { useTransactions } from '@/features/finances/transactions/hooks/useTransactions';
import { useTransactionsFilters } from '@/features/finances/transactions/hooks/useTransactionsFilters';

import styles from './Transactions.module.scss';

export const Transactions = () => {
  const { control, watch, handleReset } = useTransactionsFilters();

  const filters = watch();

  const { data, isLoading, isSuccess, refetch } = useTransactions(filters);

  const onFiltersApply = () => {
    refetch();
  };

  return (
    <div className={styles['transactions']}>
      <div className={styles['transactions__filters']}>
        <TransactionsFilters
          control={control}
          onFiltersApply={onFiltersApply}
          resetFilters={handleReset}
        />
      </div>
      <div className={styles['transactions__main-content']}>
        {isSuccess && data && (
          <TransactionsSummary
            expenseSummaryData={data.expenseSummary}
            incomeSummaryData={data.incomeSummary}
          />
        )}
        {data && <NewTransactionButtons />}
        <div className={styles['transactions__main-content__list']}>
          {isLoading && <p>Loading...</p>}
          {!isLoading && !data && <p>No records</p>}
          {isSuccess &&
            data &&
            data.transactions.map((i) => {
              return <TransactionItem key={i.id} transactionData={i} />;
            })}
        </div>
      </div>
    </div>
  );
};
