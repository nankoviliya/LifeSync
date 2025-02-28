interface SummaryItemProps {
  label: string;
  value: number;
  currency: string;
  className?: string;
}

export const ExpenseSummaryItem = ({
  label,
  value,
  currency,
  className = '',
}: SummaryItemProps) => (
  <p>
    {label}: <span className={className}>{value}</span> {currency}
  </p>
);
