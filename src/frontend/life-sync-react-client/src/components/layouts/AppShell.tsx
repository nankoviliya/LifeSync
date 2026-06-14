import { useMemo, useState } from 'react';
import { Outlet, useLocation } from 'react-router-dom';

import { AppShellHeaderContext } from '@/components/layouts/AppShellHeader';
import { Sidebar } from '@/components/sidebar/Sidebar';
import { SidebarMobileDrawer } from '@/components/sidebar/SidebarMobileDrawer';
import { ThemeSegmentedToggle } from '@/components/topbar/ThemeSegmentedToggle';
import { Topbar } from '@/components/topbar/Topbar';
import { routePaths } from '@/config/routing/routePaths';
import { useAuth } from '@/stores/AuthProvider';

interface ShellHeaderState {
  title: string;
  subtitle?: string;
  actions?: React.ReactNode;
}

const FALLBACK_HEADER: ShellHeaderState = { title: '' };

const PATH_TO_TOOL_ID: ReadonlyArray<{ pathPrefix: string; id: string }> = [
  { pathPrefix: routePaths.finances.path, id: 'money' },
  { pathPrefix: routePaths.home.path, id: 'dashboard' },
  { pathPrefix: routePaths.userProfile.path, id: 'dashboard' }, // profile is in user card; keep dashboard active
];

const resolveActiveId = (pathname: string): string => {
  // Most-specific prefix wins (finances/transactions before finances).
  const sortedByLength = [...PATH_TO_TOOL_ID].sort(
    (a, b) => b.pathPrefix.length - a.pathPrefix.length,
  );
  const match = sortedByLength.find(
    ({ pathPrefix }) =>
      pathname === pathPrefix || pathname.startsWith(`${pathPrefix}/`),
  );
  return match?.id ?? 'dashboard';
};

const buildInitials = (firstName?: string, lastName?: string): string => {
  const f = firstName?.trim()?.[0] ?? '';
  const l = lastName?.trim()?.[0] ?? '';
  return (f + l).toUpperCase() || '?';
};

export const AppShell = () => {
  const location = useLocation();
  const { user } = useAuth();

  const [headerState, setHeaderState] = useState<ShellHeaderState | null>(null);

  const ctxValue = useMemo(() => ({ setHeader: setHeaderState }), []);

  const activeId = resolveActiveId(location.pathname);

  const userInfo = {
    name: user
      ? `${user.firstName ?? ''} ${user.lastName ?? ''}`.trim() ||
        (user.email ?? '')
      : '',
    email: user?.email ?? '',
    initials: buildInitials(user?.firstName, user?.lastName),
  };

  const header = headerState ?? FALLBACK_HEADER;

  return (
    <AppShellHeaderContext.Provider value={ctxValue}>
      <div className="flex h-dvh w-full overflow-hidden bg-background text-foreground">
        {/* Desktop sidebar — hidden below md, visible at md+ */}
        <aside className="hidden h-dvh w-56 shrink-0 border-r border-side-border bg-side md:flex">
          <Sidebar activeId={activeId} user={userInfo} />
        </aside>
        <main className="flex-1 overflow-auto px-4 pb-6 pt-5 md:px-9 md:pb-9 md:pt-7">
          <Topbar
            title={header.title}
            subtitle={header.subtitle}
            leading={
              <SidebarMobileDrawer activeId={activeId} user={userInfo} />
            }
            actions={
              <>
                <ThemeSegmentedToggle />
                {header.actions}
              </>
            }
          />
          <Outlet />
        </main>
      </div>
    </AppShellHeaderContext.Provider>
  );
};
