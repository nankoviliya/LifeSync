import { Menu } from 'primereact/menu';
import { MenuItem } from 'primereact/menuitem';
import { useMemo, useRef } from 'react';

import { useTheme } from '@/hooks/useTheme';

export const useThemeToggle = () => {
  const { theme, effectiveTheme, setTheme } = useTheme();

  const menuRef = useRef<Menu>(null);

  const themeMenuItems = useMemo<MenuItem[]>(
    () => [
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
    ],
    [theme, setTheme],
  );

  const toggleMenu = (e: React.MouseEvent<HTMLElement>) => {
    menuRef.current?.toggle(e);
  };

  const icon = useMemo(
    () => (effectiveTheme === 'dark' ? 'pi pi-moon' : 'pi pi-sun'),
    [effectiveTheme],
  );

  return {
    menuRef,
    themeMenuItems,
    toggleMenu,
    icon,
  };
};
