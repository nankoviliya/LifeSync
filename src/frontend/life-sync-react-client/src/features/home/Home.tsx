import { ServiceCard } from '@/components/serviceCard/components/ServiceCard';
import { applicationServices } from '@/config/applicationServices/applicationService';

import styles from './Home.module.scss';

export const Home = () => {
  return (
    <div className={styles['home']}>
      <div className={styles['home__services-list']}>
        <ServiceCard service={applicationServices.finances} />
      </div>
    </div>
  );
};
