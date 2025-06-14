import { ServiceCard } from '@/components/serviceCard/components/ServiceCard';
import { applicationServices } from '@/config/applicationServices/applicationService';

import styles from './Finances.module.scss';

export const Finances = () => {
  return (
    <div className={styles['finances']}>
      <ServiceCard service={applicationServices.financeTransactions} />
    </div>
  );
};
