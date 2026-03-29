import { Check, Copy } from 'lucide-react';
import { useState } from 'react';

interface IProps {
  label: string;
  value: string;
}

export const CopyableRow = ({ label, value }: IProps) => {
  const [expanded, setExpanded] = useState(false);
  const [copied, setCopied] = useState(false);

  const handleCopy = () => {
    navigator.clipboard.writeText(value);
    setCopied(true);
    setTimeout(() => setCopied(false), 1500);
  };

  return (
    <div className="flex flex-col gap-1 text-sm">
      <div className="flex justify-between items-center gap-2">
        <span className="text-muted-foreground shrink-0">{label}</span>
        <span
          className="font-medium text-right truncate max-w-[160px] cursor-pointer hover:text-primary"
          onClick={() => setExpanded(!expanded)}
        >
          {value}
        </span>
      </div>
      {expanded && (
        <div className="flex items-center gap-2 text-xs bg-muted rounded px-2 py-1">
          <span className="break-all flex-1 text-right">{value}</span>
          <button
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
      )}
    </div>
  );
};
