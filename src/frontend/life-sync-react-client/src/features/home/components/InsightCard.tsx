import { Sparkles } from 'lucide-react';

import { useAppTranslations } from '@/hooks/useAppTranslations';

// TODO(afterwork #5.insight): replace static copy with a real computed/served
// insight. No fabricated live numbers here. See afterwork.md.
export const InsightCard = () => {
  const { translate } = useAppTranslations();
  return (
    <div className="flex items-center gap-[18px] rounded-xl border border-border bg-card p-5 text-card-foreground shadow-xs md:col-span-4">
      <div className="flex size-9 shrink-0 items-center justify-center rounded-md bg-primary-soft text-primary-soft-foreground">
        <Sparkles className="size-4" aria-hidden="true" />
      </div>
      <div className="flex-1">
        <div className="text-sm font-medium">
          {translate('dashboard-insight-title', {
            defaultValue: 'Your weekly insights will appear here.',
          })}
        </div>
        <div className="mt-0.5 text-xs text-muted-foreground">
          {translate('dashboard-insight-subtitle', {
            defaultValue: 'Track your spending to unlock personalized tips.',
          })}
        </div>
      </div>
    </div>
  );
};
