import { ArrowDown, ArrowUp } from 'lucide-react';

import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';

export interface TransactionRowProps {
  /** transaction description */
  title: string;
  /** secondary line, e.g. formatted date */
  meta: string;
  /** optional category badge text (e.g. expenseType) */
  category?: string;
  /** already-formatted, signed amount (use formatSignedCurrency) */
  amount: string;
  /** income → true (success styling, down arrow); expense → false (muted, up arrow) */
  positive: boolean;
  /** suppress the top border for the first row in a list */
  first?: boolean;
}

export const TransactionRow = ({
  title,
  meta,
  category,
  amount,
  positive,
  first,
}: TransactionRowProps) => {
  return (
    <div
      className={cn(
        'grid grid-cols-[30px_1fr_auto_auto] items-center gap-3.5 py-[11px]',
        !first && 'border-t border-border',
      )}
    >
      <div
        className={cn(
          'flex size-[30px] items-center justify-center rounded-md',
          positive
            ? 'bg-success-soft text-success'
            : 'bg-secondary text-muted-foreground',
        )}
      >
        {positive ? (
          <ArrowDown className="size-3.5" aria-hidden="true" />
        ) : (
          <ArrowUp className="size-3.5" aria-hidden="true" />
        )}
      </div>
      <div className="min-w-0">
        <div className="truncate text-[13.5px] font-medium">{title}</div>
        <div className="mt-0.5 text-[11.5px] text-muted-foreground">{meta}</div>
      </div>
      <div>
        {category && (
          <Badge variant="secondary" className="font-normal">
            {category}
          </Badge>
        )}
      </div>
      <div
        className={cn(
          'text-right text-sm font-semibold tabular-nums',
          positive ? 'text-success' : 'text-foreground',
        )}
      >
        {amount}
      </div>
    </div>
  );
};
