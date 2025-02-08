import {
  InvalidateQueryFilters,
  InvalidateOptions,
  useQueryClient,
} from '@tanstack/react-query';

export const useQueryInvalidation = () => {
  const queryClient = useQueryClient();

  const invalidateQuery = (
    filters?: InvalidateQueryFilters,
    options?: InvalidateOptions,
  ) => {
    queryClient.invalidateQueries(filters, options);
  };

  return invalidateQuery;
};
