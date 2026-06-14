export interface TopbarProps {
  title: string;
  subtitle?: string;
  actions?: React.ReactNode;
  /** Slot rendered before the title block — used for the mobile hamburger trigger. */
  leading?: React.ReactNode;
}

export const Topbar = ({ title, subtitle, actions, leading }: TopbarProps) => {
  return (
    <div className="mb-5 flex flex-wrap items-end justify-between gap-3 md:mb-6">
      <div className="flex min-w-0 items-center gap-2.5">
        {leading}
        <div className="min-w-0">
          {subtitle && (
            <div className="mb-1 font-mono text-[11px] uppercase tracking-[0.08em] text-muted-foreground md:mb-1.5 md:text-[11.5px]">
              {subtitle}
            </div>
          )}
          <h1 className="m-0 truncate text-[22px] font-semibold tracking-[-0.025em] md:text-[28px]">
            {title}
          </h1>
        </div>
      </div>
      {actions && (
        <div className="flex flex-wrap items-center gap-2">{actions}</div>
      )}
    </div>
  );
};
