import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useReadQuery } from '@/hooks/api/useReadQuery';

export type ExportFormat = 'json';

const formatMap: Record<ExportFormat, number> = { json: 1 };

export interface IExportAccountResponse {
  encodedData: string;
  contentType: string;
  fileName: string;
}

export const useExportAccountData = (format: ExportFormat) => {
  return useReadQuery<IExportAccountResponse>({
    endpoint: endpointsOptions.exportAccountData.endpoint,
    queryKey: [endpointsOptions.exportAccountData.key],
    config: { params: { format: formatMap[format] } },
    enabled: false,
  });
};
