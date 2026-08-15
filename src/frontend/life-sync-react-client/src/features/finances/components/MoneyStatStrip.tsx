import { StatCard } from '@/components/composites/StatCard';
import {
  IExpenseSummaryData,
  IIncomeSummaryData,
} from '@/features/finances/transactions/models/transactionsGetModel';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { formatCurrency } from '@/utils/formatCurrency';

export interface MoneyStatStripProps {
  incomeSummary: IIncomeSummaryData;
  expenseSummary: IExpenseSummaryData;
  balanceAmount: number;
  balanceCurrency: string;
}

export const MoneyStatStrip = ({
  incomeSummary,
  expenseSummary,
  balanceAmount,
  balanceCurrency,
}: MoneyStatStripProps) => {
  const { translate } = useAppTranslations();

  const { totalIncome, currency: incomeCurrency } = incomeSummary;
  const {
    totalSpent,
    totalSpentOnSavings,
    currency: expenseCurrency,
  } = expenseSummary;

  // Savings rate is the only honestly-derivable delta (savings ÷ income).
  // Period-over-period deltas need backend history (deferred — afterwork).
  // TODO: move this savings-rate calculation to the backend and expose it on
  // the summary payload — financial figures should be computed server-side, not
  // in the presentation layer, so the number is authoritative and consistent
  // across clients.
  const savingsRate =
    totalIncome > 0
      ? `${Math.round((totalSpentOnSavings / totalIncome) * 100)}% ${translate(
          'money-stat-savings-rate-suffix',
          { defaultValue: 'rate' },
        )}`
      : undefined;

  return (
    <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
      <StatCard
        label={translate('money-stat-income', { defaultValue: 'Income' })}
        value={formatCurrency(totalIncome, incomeCurrency, { cents: false })}
      />
      <StatCard
        label={translate('money-stat-spent', { defaultValue: 'Spent' })}
        value={formatCurrency(totalSpent, expenseCurrency)}
      />
      <StatCard
        label={translate('money-stat-saved', { defaultValue: 'Saved' })}
        value={formatCurrency(totalSpentOnSavings, expenseCurrency, {
          cents: false,
        })}
        delta={savingsRate}
      />
      <StatCard
        label={translate('money-stat-balance', { defaultValue: 'Balance' })}
        value={formatCurrency(balanceAmount, balanceCurrency)}
      />
    </div>
  );
};
