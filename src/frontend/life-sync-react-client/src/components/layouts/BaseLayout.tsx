import { Footer } from '@/components/footer/components/Footer';
import { Header } from '@/components/header/components/Header';

import styles from './BaseLayout.module.scss';

type BaseLayoutProps = {
  children: React.ReactNode;
};

export const BaseLayout = ({ children }: BaseLayoutProps) => {
  return (
    <div className={styles['base-layout']}>
      <Header />
      <div className={styles['base-layout__content']}>{children}</div>
      <Footer />
    </div>
  );
};
