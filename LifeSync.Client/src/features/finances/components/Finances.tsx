import { routePaths } from '@/infrastructure/routing/routePaths';
import { Link } from 'react-router-dom';
import styles from './Finances.module.scss';
import { ServiceCard } from '@/components/serviceCard/components/ServiceCard';
import { applicationServices } from '@/infrastructure/applicationServices/applicationService';

export const Finances = () => {
  return (
    <div className={styles['finances']}>
      <ServiceCard service={applicationServices.incomeTransactions} />
      <ServiceCard service={applicationServices.expenseTransactions} />
    </div>
  );
};
