import { ServiceCard } from '@/components/serviceCard/components/ServiceCard';
import { applicationServices } from '@/config/applicationServices/applicationService';

export const Home = () => {
  return (
    <div className="flex gap-4">
      <div className="m-8 grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        <ServiceCard service={applicationServices.finances} />
      </div>
    </div>
  );
};
