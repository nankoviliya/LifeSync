import { classNames } from 'primereact/utils';

import { appConstants } from '@/config/constants/appConstants';
import {
  IExpenseTransactionGetModel,
  IIncomeTransactionGetModel,
  TransactionType,
} from '@/features/finances/transactions/models/transactionsGetModel';

import styles from './TransactionItem.module.scss';

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
      className={classNames(styles['transaction-item'], {
        [styles['transaction-item--income']]:
          transactionType === TransactionType.Income,
        [styles['transaction-item--expense']]:
          transactionType === TransactionType.Expense,
      })}
    >
      <div className={styles['transaction-item__description']}>
        <span>{description}</span>
        <i>{expenseType ?? appConstants.emptySpanFallback}</i>
      </div>
      <div className={styles['transaction-item__main-info']}>
        <strong>
          {transactionSign} {amount} {currency}
        </strong>
        <span>{date}</span>
      </div>
    </div>
  );
};
