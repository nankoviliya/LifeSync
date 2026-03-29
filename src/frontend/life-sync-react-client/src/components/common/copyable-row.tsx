import { Check, Copy } from 'lucide-react';
import { useState } from 'react';

interface IProps {
  label: string;
  value: string;
}

export const CopyableRow = ({ label, value }: IProps) => {
  const [copied, setCopied] = useState(false);

  const handleCopy = () => {
    navigator.clipboard.writeText(value);
    setCopied(true);
    setTimeout(() => setCopied(false), 1500);
  };

  return (
    <details className="flex flex-col gap-1 text-sm">
      <summary className="flex justify-between items-center gap-2 cursor-pointer list-none">
        <span className="text-muted-foreground shrink-0">{label}</span>
        <span className="font-medium text-right truncate max-w-[160px] hover:text-primary">
          {value}
        </span>
      </summary>
      <div className="flex items-center gap-2 text-xs bg-muted rounded px-2 py-1">
        <span className="break-all flex-1 text-right">{value}</span>
        <button
          type="button"
          onClick={handleCopy}
          className="shrink-0 text-muted-foreground hover:text-primary"
        >
          {copied ? (
            <Check className="h-3 w-3" />
          ) : (
            <Copy className="h-3 w-3" />
          )}
        </button>
      </div>
    </details>
  );
};
