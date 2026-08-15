import { EmptyState } from '@/components/composites/EmptyState';
import { AppShellHeader } from '@/components/layouts/AppShellHeader';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { MoneyStatStrip } from '@/features/finances/components/MoneyStatStrip';
import { MoneyTransactionList } from '@/features/finances/components/MoneyTransactionList';
import { NewTransactionButtons } from '@/features/finances/transactions/components/NewTransactionButtons';
import { TransactionsFilters } from '@/features/finances/transactions/components/TransactionsFilters';
import { useTransactions } from '@/features/finances/transactions/hooks/useTransactions';
import { useTransactionsFilters } from '@/features/finances/transactions/hooks/useTransactionsFilters';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useAuth } from '@/stores/AuthProvider';

export const Finances = () => {
  const { translate } = useAppTranslations();
  const { user } = useAuth();

  const { control, watch, handleReset } = useTransactionsFilters();
  const filters = watch();
  const { data, isLoading, isSuccess, isError, refetch } =
    useTransactions(filters);

  const onFiltersApply = () => {
    refetch();
  };

  const hasTransactions = isSuccess && data && data.transactions.length > 0;

  return (
    <>
      <AppShellHeader
        title={translate('page-money-title', { defaultValue: 'Money' })}
        subtitle={translate('page-money-subtitle', {
          defaultValue: 'month to date',
        })}
        actions={<NewTransactionButtons />}
      />

      {isLoading && (
        <div className="mb-[18px] grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
          {Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-[92px] rounded-xl" />
          ))}
        </div>
      )}
      {isSuccess && data && user && (
        <div className="mb-[18px]">
          <MoneyStatStrip
            incomeSummary={data.incomeSummary}
            expenseSummary={data.expenseSummary}
            balanceAmount={user.balanceAmount}
            balanceCurrency={user.balanceCurrency}
          />
        </div>
      )}

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-[280px_1fr]">
        <div>
          <TransactionsFilters
            control={control}
            onFiltersApply={onFiltersApply}
            resetFilters={handleReset}
          />
        </div>

        <div>
          {isLoading && (
            <div className="rounded-xl border border-border bg-card p-[18px]">
              {Array.from({ length: 6 }).map((_, i) => (
                <Skeleton key={i} className="mb-3 h-10 rounded-md" />
              ))}
            </div>
          )}

          {isError && (
            <div className="rounded-xl border border-destructive/40 bg-card p-6 text-center">
              <p className="text-sm text-destructive">
                {translate('money-load-error', {
                  defaultValue: "Couldn't load your transactions.",
                })}
              </p>
              <Button
                type="button"
                variant="outline"
                className="mt-3"
                onClick={() => refetch()}
              >
                {translate('money-load-retry', { defaultValue: 'Retry' })}
              </Button>
            </div>
          )}

          {isSuccess && !hasTransactions && (
            <EmptyState
              title={translate('money-empty-title', {
                defaultValue: 'No transactions yet',
              })}
              description={translate('money-empty-description', {
                defaultValue: 'Add your first transaction to get started.',
              })}
            />
          )}

          {hasTransactions && (
            <MoneyTransactionList transactions={data.transactions} />
          )}
        </div>
      </div>
    </>
  );
};
