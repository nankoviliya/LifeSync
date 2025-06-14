import { Sidebar } from 'primereact/sidebar';
import { classNames } from 'primereact/utils';
import { useCallback, useEffect, useState } from 'react';

import styles from './SidebarContainer.module.scss';

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
  const [isVisible, setIsVisible] = useState<boolean>(isOpen);

  useEffect(() => {
    setIsVisible(isOpen);
  }, [isOpen]);

  const onCloseBase = useCallback(() => {
    setIsVisible(false);
    onClose?.();
  }, [onClose]);

  return (
    <div
      className={classNames(styles['sidebar-container'], {
        className,
      })}
    >
      <Sidebar visible={isVisible} onHide={onCloseBase} position={position}>
        {children}
      </Sidebar>
    </div>
  );
};
