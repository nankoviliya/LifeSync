import { AppShellHeader } from '@/components/layouts/AppShellHeader';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { AllocationCard } from '@/features/home/components/AllocationCard';
import { BalanceHero } from '@/features/home/components/BalanceHero';
import { InsightCard } from '@/features/home/components/InsightCard';
import { RecentTransactionsCard } from '@/features/home/components/RecentTransactionsCard';
import { SoonTile } from '@/features/home/components/SoonTile';
import { useDashboardData } from '@/features/home/hooks/useDashboardData';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const Home = () => {
  const { translate } = useAppTranslations();
  const { user, data, isLoading, isSuccess, isError, refetch } =
    useDashboardData();

  return (
    <>
      <AppShellHeader
        title={translate('page-dashboard-title', { defaultValue: 'Dashboard' })}
        subtitle={translate('page-dashboard-subtitle', {
          defaultValue: 'Today',
        })}
      />

      {isLoading && (
        <div className="grid grid-cols-1 gap-3.5 md:grid-cols-6">
          <Skeleton className="h-64 rounded-xl md:col-span-3 md:row-span-2" />
          <Skeleton className="h-64 rounded-xl md:col-span-2 md:row-span-2" />
          <Skeleton className="h-64 rounded-xl md:col-span-1 md:row-span-2" />
          <Skeleton className="h-48 rounded-xl md:col-span-4 md:row-span-2" />
          <Skeleton className="h-48 rounded-xl md:col-span-2" />
        </div>
      )}

      {isError && (
        <div className="rounded-xl border border-destructive/40 bg-card p-6 text-center">
          <p className="text-sm text-destructive">
            {translate('dashboard-load-error', {
              defaultValue: "Couldn't load your dashboard.",
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

      {isSuccess && data && user && (
        <div className="grid grid-cols-1 gap-3.5 md:auto-rows-[minmax(120px,auto)] md:grid-cols-6">
          <BalanceHero
            balanceAmount={user.balanceAmount}
            balanceCurrency={user.balanceCurrency}
            incomeSummary={data.incomeSummary}
            expenseSummary={data.expenseSummary}
          />
          <AllocationCard expenseSummary={data.expenseSummary} />
          <SoonTile
            className="md:col-span-1 md:row-span-2"
            label={translate('nav-fitness', { defaultValue: 'Fitness' })}
            teaser={translate('dashboard-fitness-teaser', {
              defaultValue: 'Steps, workouts, sleep in one place.',
            })}
          />
          <RecentTransactionsCard transactions={data.transactions} />
          <SoonTile
            className="md:col-span-2"
            label={translate('nav-journal', { defaultValue: 'Journal' })}
            teaser={translate('dashboard-journal-teaser', {
              defaultValue: 'A quiet place to think.',
            })}
          />
          <InsightCard />
        </div>
      )}
    </>
  );
};
