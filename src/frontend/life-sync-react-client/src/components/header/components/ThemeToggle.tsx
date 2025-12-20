import { Button } from 'primereact/button';
import { Menu } from 'primereact/menu';

import { useThemeToggle } from '@/components/header/hooks/useThemeToggle';

export const ThemeToggle = () => {
  const { menuRef, themeMenuItems, toggleMenu, icon, ariaLabel } =
    useThemeToggle();

  return (
    <div>
      <Button
        icon={icon}
        onClick={toggleMenu}
        rounded
        text
        aria-label={ariaLabel}
      />
      <Menu
        model={themeMenuItems}
        popup
        ref={menuRef}
        id="theme-menu"
        popupAlignment="left"
      />
    </div>
  );
};
