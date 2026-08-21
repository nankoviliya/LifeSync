import { cn } from '@/lib/utils';

export interface SettingsSectionProps {
  id: string;
  title: string;
  description: string;
  danger?: boolean;
  children: React.ReactNode;
}

export const SettingsSection = ({
  id,
  title,
  description,
  danger,
  children,
}: SettingsSectionProps) => {
  return (
    <section
      id={id}
      className={cn(
        'scroll-mt-6 rounded-xl border bg-card p-[22px] text-card-foreground shadow-xs',
        danger ? 'border-destructive/40' : 'border-border',
      )}
    >
      <div
        className={cn(
          'text-[15px] font-semibold tracking-[-0.01em]',
          danger && 'text-destructive',
        )}
      >
        {title}
      </div>
      <div className="mt-1 text-[12.5px] text-muted-foreground">
        {description}
      </div>
      <div className="mt-1.5">{children}</div>
    </section>
  );
};
