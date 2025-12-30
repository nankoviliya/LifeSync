import { NewTransactionButtons } from '@/features/finances/transactions/components/NewTransactionButtons';
import { TransactionItem } from '@/features/finances/transactions/components/TransactionItem';
import { TransactionsFilters } from '@/features/finances/transactions/components/TransactionsFilters';
import { TransactionsSummary } from '@/features/finances/transactions/components/TransactionsSummary';
import { useTransactions } from '@/features/finances/transactions/hooks/useTransactions';
import { useTransactionsFilters } from '@/features/finances/transactions/hooks/useTransactionsFilters';

export const Transactions = () => {
  const { control, watch, handleReset } = useTransactionsFilters();

  const filters = watch();

  const { data, isLoading, isSuccess, refetch } = useTransactions(filters);

  const onFiltersApply = () => {
    refetch();
  };

  return (
    <div className="inline-flex w-full flex-row gap-4">
      <div className="inline-flex">
        <TransactionsFilters
          control={control}
          onFiltersApply={onFiltersApply}
          resetFilters={handleReset}
        />
      </div>
      <div className="inline-flex w-full flex-col gap-4">
        {isSuccess && data && (
          <TransactionsSummary
            expenseSummaryData={data.expenseSummary}
            incomeSummaryData={data.incomeSummary}
          />
        )}
        {data && <NewTransactionButtons />}
        <div className="grid grid-cols-1 gap-4">
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
