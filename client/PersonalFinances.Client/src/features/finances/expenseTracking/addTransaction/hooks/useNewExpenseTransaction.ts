import { useNewExpenseTransactionMutation } from '@/features/finances/expenseTracking/addTransaction/hooks/useNewExpenseTransactionMutation';
import { INewExpenseTransactionRequest } from '@/features/finances/expenseTracking/addTransaction/models/newExpenseTransactionRequest';
import { useState } from 'react';
import { useForm } from 'react-hook-form';

export const useNewExpenseTransaction = () => {
  const [isModalVisible, setIsModalVisible] = useState<boolean>(false);

  const { control, handleSubmit } = useForm<INewExpenseTransactionRequest>({
    defaultValues: {
      amount: 0,
      currency: '',
      date: undefined,
      description: '',
      expenseType: undefined,
    },
  });

  const { onSubmit } = useNewExpenseTransactionMutation(() => {
    setIsModalVisible(false);
  });

  return {
    control,
    handleSubmit: handleSubmit(onSubmit),
    isModalVisible,
    setIsModalVisible,
  };
};
