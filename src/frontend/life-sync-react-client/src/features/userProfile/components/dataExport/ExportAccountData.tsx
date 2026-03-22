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
import { useAppTranslations } from '@/hooks/useAppTranslations';

type ExportFormat = 'json';

const EXPORT_FORMATS: ExportFormat[] = ['json'];

export const ExportAccountData = () => {
  const { translate } = useAppTranslations();
  const [format, setFormat] = useState<ExportFormat>('json');

  const handleExport = () => {
    // TODO: trigger export with selected format
    console.log('Exporting as', format);
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
          label={translate('export-button')}
          icon={<Download className="h-4 w-4" />}
          className="w-full"
          onClick={handleExport}
        />
      </CardContent>
    </Card>
  );
};
