import { Menu } from 'primereact/menu';
import { MenuItem } from 'primereact/menuitem';
import { useRef } from 'react';

import { useTheme } from '@/hooks/useTheme';
import { Theme } from '@/types/theme';

export const useThemeToggle = () => {
  const { theme, effectiveTheme, setTheme } = useTheme();

  const menuRef = useRef<Menu>(null);

  const themeMenuItems: MenuItem[] = [
    {
      label: 'Light',
      icon: 'pi pi-sun',
      className: theme === 'light' ? 'selected-theme' : '',
      command: () => setTheme('light'),
    },
    {
      label: 'Dark',
      icon: 'pi pi-moon',
      className: theme === 'dark' ? 'selected-theme' : '',
      command: () => setTheme('dark'),
    },
    {
      label: 'System',
      icon: 'pi pi-desktop',
      className: theme === 'system' ? 'selected-theme' : '',
      command: () => setTheme('system'),
    },
  ];

  const toggleMenu = (e: React.MouseEvent<HTMLElement>) => {
    menuRef.current?.toggle(e);
  };

  const getIcon = (): string => {
    return effectiveTheme === 'dark' ? 'pi pi-moon' : 'pi pi-sun';
  };

  const getAriaLabel = (): string => {
    const themeLabels: Record<Theme, string> = {
      light: 'Light theme selected',
      dark: 'Dark theme selected',
      system: 'System theme selected',
    };
    return themeLabels[theme];
  };

  return {
    menuRef,
    themeMenuItems,
    toggleMenu,
    icon: getIcon(),
    ariaLabel: getAriaLabel(),
  };
};
