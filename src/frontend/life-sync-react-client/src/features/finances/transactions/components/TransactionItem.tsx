import { appConstants } from '@/config/constants/appConstants';
import {
  IExpenseTransactionGetModel,
  IIncomeTransactionGetModel,
  TransactionType,
} from '@/features/finances/transactions/models/transactionsGetModel';
import { cn } from '@/lib/utils';

export interface ITransactionItemProps {
  transactionData: IExpenseTransactionGetModel | IIncomeTransactionGetModel;
}

export const TransactionItem = ({ transactionData }: ITransactionItemProps) => {
  const { transactionType, description, amount, currency, date } =
    transactionData;

  const { expenseType } = transactionData as IExpenseTransactionGetModel;

  const transactionSign =
    transactionType === TransactionType.Expense ? '-' : '+';

  return (
    <div
      className={cn(
        'inline-flex items-center justify-between border-l border-border px-2',
        transactionType === TransactionType.Income &&
          'border-r-[0.5rem] border-r-emerald-600',
        transactionType === TransactionType.Expense &&
          'border-r-[0.5rem] border-r-red-500',
      )}
    >
      <div className="inline-flex flex-col gap-2">
        <span>{description}</span>
        <i className="text-muted-foreground">
          {expenseType ?? appConstants.emptySpanFallback}
        </i>
      </div>
      <div className="inline-flex flex-col gap-2 text-right">
        <strong>
          {transactionSign} {amount} {currency}
        </strong>
        <span className="text-sm text-muted-foreground">{date}</span>
      </div>
    </div>
  );
};
