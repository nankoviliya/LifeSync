import { useState } from 'react';

export const useNewTransactionButtons = () => {
  const [isIncomeFormVisible, setIsIncomeFormVisible] =
    useState<boolean>(false);

  const [isExpenseFormVisible, setIsExpenseFormVisible] =
    useState<boolean>(false);

  return {
    isIncomeFormVisible,
    setIsIncomeFormVisible,
    isExpenseFormVisible,
    setIsExpenseFormVisible,
  };
};
