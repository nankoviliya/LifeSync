import styles from './Home.module.scss';
import { ServiceCard } from '@/features/common/serviceCard/components/ServiceCard';
import { applicationServices } from '@/infrastructure/applicationServices/applicationService';

export const Home = () => {
  return (
    <div className={styles['home']}>
      <div className={styles['home__services-list']}>
        <ServiceCard service={applicationServices.finances} />
      </div>
    </div>
  );
};
