import { ServiceCard } from '@/components/serviceCard/components/ServiceCard';
import { applicationServices } from '@/config/applicationServices/applicationService';

export const Finances = () => {
  return (
    <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
      <ServiceCard service={applicationServices.financeTransactions} />
    </div>
  );
};
