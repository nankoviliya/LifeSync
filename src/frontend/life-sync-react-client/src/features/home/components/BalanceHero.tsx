import { Spark } from '@/features/home/components/charts/Spark';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import {
  IExpenseSummaryData,
  IIncomeSummaryData,
} from '@/hooks/useTransactions';
import { formatCurrency } from '@/utils/formatCurrency';

export interface BalanceHeroProps {
  balanceAmount: number;
  balanceCurrency: string;
  incomeSummary: IIncomeSummaryData;
  expenseSummary: IExpenseSummaryData;
}

export const BalanceHero = ({
  balanceAmount,
  balanceCurrency,
  incomeSummary,
  expenseSummary,
}: BalanceHeroProps) => {
  const { translate } = useAppTranslations();

  const miniStats: ReadonlyArray<[string, string]> = [
    [
      translate('dashboard-hero-income', { defaultValue: 'INCOME' }),
      formatCurrency(incomeSummary.totalIncome, incomeSummary.currency, {
        cents: false,
      }),
    ],
    [
      translate('dashboard-hero-spent', { defaultValue: 'SPENT' }),
      formatCurrency(expenseSummary.totalSpent, expenseSummary.currency),
    ],
    [
      translate('dashboard-hero-saved', { defaultValue: 'SAVED' }),
      formatCurrency(
        expenseSummary.totalSpentOnSavings,
        expenseSummary.currency,
        { cents: false },
      ),
    ],
  ];

  return (
    <div className="flex flex-col justify-between gap-6 rounded-xl bg-side p-[26px] text-side-foreground md:col-span-3 md:row-span-2">
      <div>
        <div className="font-mono text-[11.5px] uppercase tracking-[0.1em] text-side-muted">
          {translate('dashboard-hero-balance-label', {
            defaultValue: 'Balance',
          })}{' '}
          · {balanceCurrency}
        </div>
        <div className="mt-2.5 text-[54px] font-medium leading-none tracking-[-0.035em] tabular-nums">
          {formatCurrency(balanceAmount, balanceCurrency)}
        </div>
      </div>
      <div>
        <div className="text-primary">
          <Spark height={60} />
        </div>
        <div className="mt-4 flex justify-between">
          {miniStats.map(([k, v]) => (
            <div key={k}>
              <div className="font-mono text-[10.5px] tracking-[0.08em] text-side-muted">
                {k}
              </div>
              <div className="mt-1 text-lg font-medium tabular-nums">{v}</div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
