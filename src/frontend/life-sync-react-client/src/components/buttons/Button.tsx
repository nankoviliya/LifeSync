import { forwardRef } from 'react';

import { Button as ShadcnButton } from '@/components/ui/button';
import { Spinner } from '@/components/ui/spinner';
import { cn } from '@/lib/utils';

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  label?: string;
  loading?: boolean;
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
      if (severity === 'success') return 'default';
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
        className={cn(
          severity === 'success' &&
            'bg-emerald-600 text-white hover:bg-emerald-700',
          rounded && 'rounded-full',
          className,
        )}
        asChild={asChild}
        {...props}
      >
        {loading && <Spinner data-icon="inline-start" />}
        {!loading && icon}
        {label || children}
      </ShadcnButton>
    );
  },
);

Button.displayName = 'Button';
