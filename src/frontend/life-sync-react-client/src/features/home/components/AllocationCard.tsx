import { Donut } from '@/features/home/components/charts/Donut';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { IExpenseSummaryData } from '@/hooks/useTransactions';
import { formatCurrency } from '@/utils/formatCurrency';

export interface AllocationCardProps {
  expenseSummary: IExpenseSummaryData;
}

export const AllocationCard = ({ expenseSummary }: AllocationCardProps) => {
  const { translate } = useAppTranslations();
  const {
    totalSpent,
    totalSpentOnNeeds,
    totalSpentOnWants,
    totalSpentOnSavings,
    currency,
  } = expenseSummary;

  const legend: ReadonlyArray<[string, number, string]> = [
    [
      translate('dashboard-alloc-needs', { defaultValue: 'Needs' }),
      totalSpentOnNeeds,
      'var(--primary)',
    ],
    [
      translate('dashboard-alloc-wants', { defaultValue: 'Wants' }),
      totalSpentOnWants,
      'var(--chart-wants)',
    ],
    [
      translate('dashboard-alloc-saved', { defaultValue: 'Saved' }),
      totalSpentOnSavings,
      'var(--chart-saved)',
    ],
  ];

  return (
    <div className="rounded-xl border border-border bg-card p-[22px] text-card-foreground shadow-xs md:col-span-2 md:row-span-2">
      <div className="text-[13px] font-semibold">
        {translate('dashboard-alloc-title', { defaultValue: 'Where it went' })}
      </div>
      <div className="mt-1 text-[11.5px] text-muted-foreground">
        {formatCurrency(totalSpent, currency)}
      </div>
      <div className="my-2 flex justify-center">
        <Donut
          size={120}
          stroke={16}
          segments={legend.map(([, value, color]) => ({ value, color }))}
        />
      </div>
      <div className="mt-2.5">
        {legend.map(([label, value, color]) => (
          <div key={label} className="mb-1.5 flex items-center gap-2">
            <span
              className="size-2 rounded-[2px]"
              style={{ background: color }}
            />
            <span className="flex-1 text-xs">{label}</span>
            <span className="text-[11.5px] font-medium tabular-nums">
              {formatCurrency(value, currency, { cents: false })}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
};
