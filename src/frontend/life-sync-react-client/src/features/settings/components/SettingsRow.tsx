export interface SettingsRowProps {
  title: string;
  description: string;
  control: React.ReactNode;
  first?: boolean;
}

export const SettingsRow = ({
  title,
  description,
  control,
  first,
}: SettingsRowProps) => {
  return (
    <div
      className={`flex items-center justify-between gap-4 py-3.5 ${
        first ? '' : 'border-t border-border'
      }`}
    >
      <div className="min-w-0">
        <div className="text-[13px] font-medium">{title}</div>
        <div className="mt-0.5 text-xs text-muted-foreground">
          {description}
        </div>
      </div>
      <div className="shrink-0">{control}</div>
    </div>
  );
};
