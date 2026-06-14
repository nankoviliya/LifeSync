import { createContext, useContext, useEffect } from 'react';

interface ShellHeaderState {
  title: string;
  subtitle?: string;
  actions?: React.ReactNode;
}

interface AppShellHeaderContextValue {
  setHeader: (state: ShellHeaderState | null) => void;
}

export const AppShellHeaderContext =
  createContext<AppShellHeaderContextValue | null>(null);

export type AppShellHeaderProps = ShellHeaderState;

export const AppShellHeader = ({
  title,
  subtitle,
  actions,
}: AppShellHeaderProps) => {
  const ctx = useContext(AppShellHeaderContext);
  if (!ctx) {
    throw new Error('AppShellHeader must be rendered inside <AppShell>');
  }

  useEffect(() => {
    ctx.setHeader({ title, subtitle, actions });
    return () => ctx.setHeader(null);
  }, [ctx, title, subtitle, actions]);

  return null;
};
