import { Inbox } from 'lucide-react';

export interface EmptyStateProps {
  title: string;
  description: string;
  /** optional action node, e.g. a Button */
  action?: React.ReactNode;
}

export const EmptyState = ({ title, description, action }: EmptyStateProps) => {
  return (
    <div className="flex flex-col items-center rounded-xl border border-border bg-card p-8 text-center text-card-foreground shadow-xs">
      <div className="mb-3.5 flex size-12 items-center justify-center rounded-lg bg-primary-soft text-primary-soft-foreground">
        <Inbox className="size-5" aria-hidden="true" />
      </div>
      <div className="text-[15px] font-semibold tracking-[-0.01em]">
        {title}
      </div>
      <div className="mx-auto mt-1.5 max-w-[280px] text-[12.5px] text-muted-foreground">
        {description}
      </div>
      {action && <div className="mt-4">{action}</div>}
    </div>
  );
};
