import { NavLink } from 'react-router-dom';

import { useAppTranslations } from '@/hooks/useAppTranslations';
import { cn } from '@/lib/utils';

export interface ToolNavItemProps {
  icon: React.ReactNode;
  label: string;
  to?: string;
  active?: boolean;
  ready: boolean;
}

const baseClasses =
  'flex items-center gap-2.5 px-5 py-2 text-[13px] text-side-foreground/80 transition-colors hover:bg-side-accent-soft hover:text-side-foreground';
const activeClasses = 'bg-side-accent-soft text-side-foreground font-medium';
const disabledClasses =
  'flex items-center gap-2.5 px-5 py-2 text-[13px] text-side-muted cursor-not-allowed select-none';

export const ToolNavItem = ({
  icon,
  label,
  to,
  active,
  ready,
}: ToolNavItemProps) => {
  const { translate } = useAppTranslations();
  const soonLabel = translate('nav-soon-badge', { defaultValue: 'SOON' });

  if (!ready || !to) {
    return (
      <div className={disabledClasses} aria-disabled="true">
        <span className="grid size-4 place-items-center opacity-70">
          {icon}
        </span>
        <span className="flex-1">{label}</span>
        <span className="rounded border border-side-border px-1.5 py-px font-mono text-[9px] tracking-wider text-side-muted">
          {soonLabel}
        </span>
      </div>
    );
  }

  return (
    <NavLink to={to} className={cn(baseClasses, active && activeClasses)} end>
      <span className="grid size-4 place-items-center">{icon}</span>
      <span>{label}</span>
    </NavLink>
  );
};
