import { queryClient } from '@/infrastructure/api/queryClient/queryClient';
import {
  InvalidateQueryFilters,
  InvalidateOptions,
} from '@tanstack/react-query';

export const useQueryInvalidation = () => {
  const invalidateQuery = (
    filters?: InvalidateQueryFilters,
    options?: InvalidateOptions,
  ) => {
    queryClient.invalidateQueries(filters, options);
  };

  return invalidateQuery;
};
