import { useAppTranslations } from '@/hooks/useAppTranslations';

export interface SoonTileProps {
  /** already-translated tool label, e.g. "Fitness" */
  label: string;
  /** already-translated one-line teaser */
  teaser: string;
  className?: string;
}

export const SoonTile = ({ label, teaser, className }: SoonTileProps) => {
  const { translate } = useAppTranslations();
  return (
    <div
      className={`flex flex-col gap-3 rounded-xl border border-dashed border-border-strong bg-card p-5 opacity-85 ${
        className ?? ''
      }`}
    >
      <div className="font-mono text-[10.5px] uppercase tracking-[0.1em] text-muted-foreground">
        {label}
      </div>
      <span className="inline-block w-fit rounded bg-secondary px-1.5 py-0.5 font-mono text-[10px] uppercase tracking-[0.08em] text-muted-foreground-hi">
        {translate('nav-soon-badge', { defaultValue: 'SOON' })}
      </span>
      <div className="text-[13px] font-medium leading-[1.35]">{teaser}</div>
    </div>
  );
};
