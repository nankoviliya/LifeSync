import {
  Activity,
  BookOpen,
  LayoutDashboard,
  Repeat,
  Target,
  Wallet,
} from 'lucide-react';

import { SearchInput } from '@/components/sidebar/SearchInput';
import { ToolNavItem } from '@/components/sidebar/ToolNavItem';
import { UserCard } from '@/components/sidebar/UserCard';
import { routePaths } from '@/config/routing/routePaths';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export interface SidebarProps {
  activeId: string;
  user: { name: string; email: string; initials: string };
  /** Called whenever a nav link is clicked — used by the mobile drawer to auto-close. */
  onNavigate?: () => void;
}

interface ToolDef {
  id: string;
  labelKey: string;
  defaultLabel: string;
  to?: string;
  icon: React.ReactNode;
  ready: boolean;
}

const TOOLS: ReadonlyArray<ToolDef> = [
  {
    id: 'dashboard',
    labelKey: 'nav-dashboard',
    defaultLabel: 'Dashboard',
    to: routePaths.home.path,
    icon: <LayoutDashboard size={16} />,
    ready: true,
  },
  {
    id: 'money',
    labelKey: 'nav-money',
    defaultLabel: 'Money',
    to: routePaths.finances.path,
    icon: <Wallet size={16} />,
    ready: true,
  },
  {
    id: 'fitness',
    labelKey: 'nav-fitness',
    defaultLabel: 'Fitness',
    icon: <Activity size={16} />,
    ready: false,
  },
  {
    id: 'journal',
    labelKey: 'nav-journal',
    defaultLabel: 'Journal',
    icon: <BookOpen size={16} />,
    ready: false,
  },
  {
    id: 'habits',
    labelKey: 'nav-habits',
    defaultLabel: 'Habits',
    icon: <Repeat size={16} />,
    ready: false,
  },
  {
    id: 'goals',
    labelKey: 'nav-goals',
    defaultLabel: 'Goals',
    icon: <Target size={16} />,
    ready: false,
  },
];

export const Sidebar = ({ activeId, user, onNavigate }: SidebarProps) => {
  const { translate } = useAppTranslations();
  const toolsLabel = translate('sidebar-tools-label', {
    defaultValue: 'Tools',
  });

  // No outer <aside> / sizing here — the consumer (desktop AppShell column or
  // mobile SidebarMobileDrawer Sheet) is responsible for width/height/positioning.
  // This component just owns the visual content + dark surface tokens.
  return (
    <div className="flex h-full w-full flex-col bg-side text-side-foreground">
      <div className="flex items-center gap-2.5 border-b border-side-border px-5 py-4">
        <div className="grid size-7 place-items-center rounded-md bg-primary font-mono text-sm font-bold text-primary-foreground">
          L
        </div>
        <div className="text-[15px] font-semibold tracking-tight">LifeSync</div>
      </div>

      <div className="px-5 pb-1.5 pt-3.5 font-mono text-[10px] uppercase tracking-[0.12em] text-side-muted">
        {toolsLabel}
      </div>
      <nav className="flex flex-col">
        {TOOLS.map((tool) => (
          <ToolNavItem
            key={tool.id}
            icon={tool.icon}
            label={translate(tool.labelKey, {
              defaultValue: tool.defaultLabel,
            })}
            to={tool.to}
            active={tool.id === activeId}
            ready={tool.ready}
            onClick={onNavigate}
          />
        ))}
      </nav>

      <div className="mt-auto border-t border-side-border p-3.5">
        <SearchInput />
        <div className="mt-2.5">
          <UserCard user={user} />
        </div>
      </div>
    </div>
  );
};
