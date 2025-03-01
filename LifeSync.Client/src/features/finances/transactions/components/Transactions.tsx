import { NewTransactionButtons } from '@/features/finances/transactions/components/NewTransactionButtons';
import { TransactionItem } from '@/features/finances/transactions/components/TransactionItem';
import { TransactionsSummary } from '@/features/finances/transactions/components/TransactionsSummary';
import { useTransactions } from '@/features/finances/transactions/hooks/useTransactions';

import styles from './Transactions.module.scss';

export const Transactions = () => {
  const { data, isLoading, isSuccess } = useTransactions();

  return (
    <div className={styles['transactions']}>
      {data && (
        <TransactionsSummary
          expenseSummaryData={data.expenseSummary}
          incomeSummaryData={data.incomeSummary}
        />
      )}
      {data && <NewTransactionButtons />}
      <div className={styles['transactions__list']}>
        {isLoading && <p>Loading...</p>}
        {!isLoading && !data && <p>No records</p>}
        {isSuccess &&
          data &&
          data.transactions.map((i) => {
            return <TransactionItem key={i.id} transactionData={i} />;
          })}
      </div>
    </div>
  );
};
