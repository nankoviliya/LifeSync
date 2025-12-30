import { Sheet, SheetContent } from '@/components/ui/sheet';
import { cn } from '@/lib/utils';

export interface ISidebarContainerProps {
  isOpen: boolean;
  position: 'left' | 'right';
  children: React.ReactNode;
  onClose?: () => void;
  className?: string;
}

export const SidebarContainer = ({
  isOpen,
  position,
  onClose,
  children,
  className,
}: ISidebarContainerProps) => {
  const handleOpenChange = (open: boolean) => {
    if (!open) {
      onClose?.();
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={handleOpenChange}>
      <SheetContent side={position} className={cn(className)}>
        {children}
      </SheetContent>
    </Sheet>
  );
};
