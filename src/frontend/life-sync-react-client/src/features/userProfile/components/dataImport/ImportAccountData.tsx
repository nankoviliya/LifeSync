import { Upload } from 'lucide-react';
import { useRef, useState } from 'react';

import { Button } from '@/components/buttons/Button';
import { Badge } from '@/components/ui/badge';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { useAppTranslations } from '@/hooks/useAppTranslations';

import { useImportAccountData } from './useImportAccountData';

type ImportFormat = 'json';
const IMPORT_FORMATS: ImportFormat[] = ['json'];

export const ImportAccountData = () => {
  const { translate } = useAppTranslations();
  const [format, setFormat] = useState<ImportFormat>('json');
  const [file, setFile] = useState<File | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const { importData, isImporting } = useImportAccountData(() => setFile(null));

  const acceptedExtensions = IMPORT_FORMATS.map((f) => `.${f}`).join(',');

  const handleFile = (f: File) => {
    const ext = f.name.split('.').pop()?.toLowerCase() as ImportFormat;
    if (IMPORT_FORMATS.includes(ext)) {
      setFormat(ext);
      setFile(f);
    }
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragging(false);
    const dropped = e.dataTransfer.files[0];
    if (dropped) handleFile(dropped);
  };

  const handleImport = () => {
    if (!file) return;
    importData({ file, format });
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-base">{translate('import-title')}</CardTitle>
        <CardDescription>{translate('import-description')}</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-4">
        <div className="flex flex-col gap-1">
          <span className="text-xs text-muted-foreground">
            {translate('import-format-label')}
          </span>
          <div className="flex gap-2">
            {IMPORT_FORMATS.map((fmt) => (
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
        <div
          role="region"
          aria-label={translate('import-drop-label')}
          className={`flex flex-col items-center justify-center gap-3 rounded-lg border border-dashed p-6 text-center transition-colors ${
            isDragging
              ? 'border-primary bg-muted'
              : 'border-muted-foreground/30'
          }`}
          onDragOver={(e) => {
            e.preventDefault();
            setIsDragging(true);
          }}
          onDragLeave={() => setIsDragging(false)}
          onDrop={handleDrop}
        >
          {file ? (
            <span className="text-sm font-medium">{file.name}</span>
          ) : (
            <span className="text-sm text-muted-foreground">
              {translate('import-drop-label')}
            </span>
          )}
          <input
            ref={inputRef}
            type="file"
            accept={acceptedExtensions}
            className="hidden"
            onChange={(e) => {
              if (e.target.files?.[0]) handleFile(e.target.files[0]);
            }}
          />
          <Button
            type="button"
            label={translate('import-browse-button')}
            outlined
            onClick={() => inputRef.current?.click()}
          />
        </div>
        <Button
          type="button"
          label={translate('import-button')}
          icon={<Upload className="h-4 w-4" />}
          className="w-full"
          disabled={!file || isImporting}
          onClick={handleImport}
        />
      </CardContent>
    </Card>
  );
};
