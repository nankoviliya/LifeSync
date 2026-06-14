import { Loader2 } from 'lucide-react';

import { cn } from '@/lib/utils';

export type SpinnerProps = React.ComponentProps<'svg'>;

export const Spinner = ({ className, ...props }: SpinnerProps) => {
  return (
    <Loader2
      role="status"
      aria-label="Loading"
      className={cn('size-6 animate-spin text-muted-foreground', className)}
      {...props}
    />
  );
};
