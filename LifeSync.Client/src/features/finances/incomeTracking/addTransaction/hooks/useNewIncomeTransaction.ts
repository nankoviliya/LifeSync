import { useState } from 'react';
import { useForm } from 'react-hook-form';

import { useNewIncomeTransactionMutation } from '@/features/finances/incomeTracking/addTransaction/hooks/useNewIncomeTransactionMutation';
import { INewIncomeTransactionRequest } from '@/features/finances/incomeTracking/addTransaction/models/newIncomeTransactionRequest';

export const useNewIncomeTransaction = () => {
  const [isModalVisible, setIsModalVisible] = useState<boolean>(false);

  const { control, handleSubmit } = useForm<INewIncomeTransactionRequest>({
    defaultValues: {
      amount: 0,
      currency: '',
      date: undefined,
      description: '',
    },
  });

  const { onSubmit } = useNewIncomeTransactionMutation(() => {
    setIsModalVisible(false);
  });

  return {
    control,
    handleSubmit: handleSubmit(onSubmit),
    isModalVisible,
    setIsModalVisible,
  };
};
