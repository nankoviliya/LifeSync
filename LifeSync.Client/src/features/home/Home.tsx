import styles from './Home.module.scss';
import { ServiceCard } from '@/components/serviceCard/components/ServiceCard';
import { useCurrentLanguage } from '@/features/home/hooks/useCurrentLanguage';
import { applicationServices } from '@/infrastructure/applicationServices/applicationService';

export const Home = () => {
  const { currentLanguage } = useCurrentLanguage();

  return (
    <div className={styles['home']}>
      <div className={styles['home__services-list']}>
        <ServiceCard service={applicationServices.finances} />
      </div>
    </div>
  );
};
