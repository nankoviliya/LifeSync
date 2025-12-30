import { Loader2 } from 'lucide-react';
import { forwardRef } from 'react';

import { Button as ShadcnButton } from '@/components/ui/button';
import { cn } from '@/lib/utils';

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  label?: string;
  loading?: boolean;
  loadingIcon?: string;
  icon?: React.ReactNode;
  severity?: 'secondary' | 'success' | 'danger';
  outlined?: boolean;
  text?: boolean;
  rounded?: boolean;
  asChild?: boolean;
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      label,
      loading,
      loadingIcon,
      icon,
      severity,
      outlined,
      text,
      rounded,
      children,
      className,
      disabled,
      asChild,
      ...props
    },
    ref,
  ) => {
    const getVariant = () => {
      if (text) return 'ghost';
      if (outlined) return 'outline';
      if (severity === 'secondary') return 'secondary';
      if (severity === 'danger') return 'destructive';
      return 'default';
    };

    const getSize = () => {
      if (icon && !label && !children) return 'icon';
      return 'default';
    };

    return (
      <ShadcnButton
        ref={ref}
        variant={getVariant()}
        size={getSize()}
        disabled={loading || disabled}
        className={cn(rounded && 'rounded-full', className)}
        asChild={asChild}
        {...props}
      >
        {loading && <Loader2 className="h-4 w-4 animate-spin" />}
        {!loading && icon}
        {label || children}
      </ShadcnButton>
    );
  },
);

Button.displayName = 'Button';
