import { Download } from 'lucide-react';
import { useState } from 'react';

import { Button } from '@/components/buttons/Button';
import { Badge } from '@/components/ui/badge';
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from '@/components/ui/card';
import {
  ExportFormat,
  useExportAccountData,
} from '@/features/userProfile/components/dataExport/useExportAccountData';
import { useAppTranslations } from '@/hooks/useAppTranslations';

const EXPORT_FORMATS: ExportFormat[] = ['json'];

export const ExportAccountData = () => {
  const { translate } = useAppTranslations();
  const [format, setFormat] = useState<ExportFormat>('json');

  const { refetch, isFetching } = useExportAccountData(format);

  const handleExport = async () => {
    const { data } = await refetch();
    if (!data) return;
    const { encodedData, contentType, fileName } = data;
    const blob = new Blob([atob(encodedData)], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
    URL.revokeObjectURL(url);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-base">{translate('export-title')}</CardTitle>
        <CardDescription>{translate('export-description')}</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-4">
        <div className="flex flex-col gap-1">
          <span className="text-xs text-muted-foreground">
            {translate('export-format-label')}
          </span>
          <div className="flex gap-2">
            {EXPORT_FORMATS.map((fmt) => (
              <Badge
                key={fmt}
                variant={format === fmt ? 'default' : 'outline'}
                className="cursor-pointer uppercase"
                onClick={() => setFormat(fmt)}
              >
                {fmt}
              </Badge>
            ))}
          </div>
        </div>

        <Button
          type="button"
          loading={isFetching}
          label={translate('export-button')}
          icon={<Download className="h-4 w-4" />}
          className="w-full"
          onClick={handleExport}
        />
      </CardContent>
    </Card>
  );
};
