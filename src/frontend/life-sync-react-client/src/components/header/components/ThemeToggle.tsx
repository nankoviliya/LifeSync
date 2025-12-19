import { Button } from 'primereact/button';

import { useTheme } from '@/hooks/useTheme';

export const ThemeToggle = () => {
  const { isDarkMode, toggleTheme } = useTheme();

  return (
    <Button
      icon={isDarkMode ? 'pi pi-sun' : 'pi pi-moon'}
      onClick={toggleTheme}
      rounded
      text
      aria-label={isDarkMode ? 'Switch to light mode' : 'Switch to dark mode'}
    />
  );
};
