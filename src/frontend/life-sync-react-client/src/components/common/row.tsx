import { ReactNode } from 'react';

interface IProps {
  label: string;
  children: ReactNode;
}

export const Row = ({ label, children }: IProps) => (
  <div className="flex justify-between items-center gap-2 text-sm">
    <span className="text-muted-foreground shrink-0">{label}</span>
    {children}
  </div>
);
