interface ITransactionsSummaryItemProps {
  label: string;
  value: number;
  currency: string;
  className?: string;
}

export const TransactionsSummaryItem = ({
  label,
  value,
  currency,
  className = '',
}: ITransactionsSummaryItemProps) => (
  <p>
    {label}:{' '}
    <strong className={className}>
      {value} {currency}
    </strong>
  </p>
);
