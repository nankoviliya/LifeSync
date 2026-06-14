import { AppShellHeader } from '@/components/layouts/AppShellHeader';
import { ServiceCard } from '@/components/serviceCard/components/ServiceCard';
import { applicationServices } from '@/config/applicationServices/applicationService';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const Home = () => {
  const { translate } = useAppTranslations();

  return (
    <>
      <AppShellHeader
        title={translate('page-dashboard-title', { defaultValue: 'Dashboard' })}
        subtitle={translate('page-dashboard-subtitle', {
          defaultValue: 'Today',
        })}
      />
      <div className="flex gap-4">
        <div className="m-8 grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
          <ServiceCard service={applicationServices.finances} />
        </div>
      </div>
    </>
  );
};
