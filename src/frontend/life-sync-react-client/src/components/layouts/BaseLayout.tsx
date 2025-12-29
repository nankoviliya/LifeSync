import { PropsWithChildren } from 'react';

import { Footer } from '@/components/footer/components/Footer';
import { Header } from '@/components/header/components/Header';

import styles from './BaseLayout.module.scss';

export const BaseLayout = ({ children }: PropsWithChildren) => {
  return (
    <div className={styles['base-layout']}>
      <Header />
      <div className={styles['base-layout__content']}>{children}</div>
      <Footer />
    </div>
  );
};
