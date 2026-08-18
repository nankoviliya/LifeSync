export interface SettingsSubNavItem {
  id: string;
  label: string;
}

export interface SettingsSubNavProps {
  items: SettingsSubNavItem[];
  activeId: string;
}

export const SettingsSubNav = ({ items, activeId }: SettingsSubNavProps) => {
  return (
    <nav className="flex flex-row flex-wrap gap-1 md:sticky md:top-6 md:flex-col md:self-start">
      {items.map((item) => (
        <a
          key={item.id}
          href={`#${item.id}`}
          className={`rounded-md px-3 py-2 text-[13px] transition-colors ${
            item.id === activeId
              ? 'bg-secondary font-medium text-foreground'
              : 'text-muted-foreground hover:text-foreground'
          }`}
        >
          {item.label}
        </a>
      ))}
    </nav>
  );
};
