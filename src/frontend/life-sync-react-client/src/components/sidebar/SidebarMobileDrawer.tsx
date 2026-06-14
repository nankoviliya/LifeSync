import { Menu } from 'lucide-react';
import { useState } from 'react';

import { Sidebar, SidebarProps } from '@/components/sidebar/Sidebar';
import { Button } from '@/components/ui/button';
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetTitle,
  SheetTrigger,
} from '@/components/ui/sheet';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export type SidebarMobileDrawerProps = SidebarProps;

export const SidebarMobileDrawer = (props: SidebarMobileDrawerProps) => {
  const [open, setOpen] = useState(false);
  const { translate } = useAppTranslations();
  const openLabel = translate('sidebar-open-menu', {
    defaultValue: 'Open menu',
  });
  const navTitle = translate('sidebar-nav-title', {
    defaultValue: 'Navigation',
  });
  const navDescription = translate('sidebar-nav-description', {
    defaultValue: 'Application navigation menu',
  });

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button
          type="button"
          variant="ghost"
          size="icon-sm"
          className="md:hidden"
          aria-label={openLabel}
        >
          <Menu className="size-5" />
        </Button>
      </SheetTrigger>
      <SheetContent
        side="left"
        className="w-64 border-side-border bg-side p-0 text-side-foreground sm:max-w-xs"
      >
        <SheetTitle className="sr-only">{navTitle}</SheetTitle>
        <SheetDescription className="sr-only">
          {navDescription}
        </SheetDescription>
        <Sidebar {...props} onNavigate={() => setOpen(false)} />
      </SheetContent>
    </Sheet>
  );
};
