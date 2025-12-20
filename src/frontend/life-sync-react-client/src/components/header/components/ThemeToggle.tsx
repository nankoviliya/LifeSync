import { Button } from 'primereact/button';
import { Menu } from 'primereact/menu';

import { useThemeToggle } from '@/components/header/hooks/useThemeToggle';

export const ThemeToggle = () => {
  const { menuRef, themeMenuItems, toggleMenu, icon } = useThemeToggle();

  return (
    <div>
      <Button
        icon={icon}
        onClick={toggleMenu}
        rounded
        text
        aria-label="Theme settings"
        aria-haspopup="menu"
        aria-controls="theme-menu"
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
