import { cn } from '@/lib/utils';

export interface StatCardProps {
  /** already-translated label, rendered mono + uppercase */
  label: string;
  /** already-formatted value (use formatCurrency) */
  value: string;
  /** optional already-formatted delta line; omit when not derivable */
  delta?: string;
  deltaTone?: 'success' | 'destructive' | 'default';
}

const deltaToneClasses: Record<
  NonNullable<StatCardProps['deltaTone']>,
  string
> = {
  success: 'text-success',
  destructive: 'text-destructive',
  default: 'text-muted-foreground',
};

export const StatCard = ({
  label,
  value,
  delta,
  deltaTone = 'success',
}: StatCardProps) => {
  return (
    <div className="rounded-xl border border-border bg-card p-[18px] text-card-foreground shadow-xs">
      <div className="font-mono text-[11px] uppercase tracking-[0.06em] text-muted-foreground">
        {label}
      </div>
      <div className="mt-1.5 text-[26px] font-medium tracking-[-0.025em] tabular-nums">
        {value}
      </div>
      {delta && (
        <div className={cn('mt-1 text-[11.5px]', deltaToneClasses[deltaTone])}>
          {delta}
        </div>
      )}
    </div>
  );
};
