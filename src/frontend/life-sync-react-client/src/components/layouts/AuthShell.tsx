import { Outlet } from 'react-router-dom';

import { useAppTranslations } from '@/hooks/useAppTranslations';

export const AuthShell = () => {
  const { translate } = useAppTranslations();
  const credits = translate('auth-shell-credits', {
    defaultValue: '© 2026 nankoviliya · portfolio · github',
  });

  return (
    <div className="flex h-dvh w-full overflow-hidden bg-background text-foreground">
      {/* Brand panel */}
      <div className="relative hidden w-[45%] flex-col justify-between overflow-hidden bg-side px-11 py-9 text-side-foreground md:flex">
        <div className="z-[2] flex items-center gap-2.5">
          <div className="grid size-7 place-items-center rounded-md bg-primary font-mono text-sm font-bold text-primary-foreground">
            L
          </div>
          <div className="text-[15px] font-semibold tracking-tight">
            LifeSync
          </div>
        </div>
        <div className="z-[2] font-mono text-[11.5px] text-side-muted">
          {credits}
        </div>
        {/* Decorative radial gradient — pointer-events disabled */}
        <div
          aria-hidden="true"
          className="pointer-events-none absolute right-[-120px] top-[140px] size-[340px] rounded-full"
          style={{
            background:
              'radial-gradient(circle, color-mix(in srgb, var(--primary) 13%, transparent) 0%, transparent 70%)',
          }}
        />
      </div>

      {/* Form panel */}
      <div className="flex flex-1 flex-col items-center justify-center px-4 py-8 md:p-10">
        {/* Mobile-only logo (brand panel is hidden below md) */}
        <div className="mb-8 flex items-center gap-2.5 md:hidden">
          <div className="grid size-7 place-items-center rounded-md bg-primary font-mono text-sm font-bold text-primary-foreground">
            L
          </div>
          <div className="text-[15px] font-semibold tracking-tight">
            LifeSync
          </div>
        </div>
        <div className="w-full max-w-[380px]">
          <Outlet />
        </div>
      </div>
    </div>
  );
};
