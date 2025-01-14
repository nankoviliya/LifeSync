import { Outlet } from 'react-router-dom';
import styles from './App.module.scss';
import { Header } from '@/features/common/header/components/Header';
import { Footer } from '@/features/common/footer/components/Footer';

export const App = () => {
  return (
    <div className={styles['app']}>
      <Header />
      <div className={styles['app__content']}>
        <Outlet />
      </div>
      <Footer />
    </div>
  );
};
