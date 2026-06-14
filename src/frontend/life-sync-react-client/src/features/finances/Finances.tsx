import { AppShellHeader } from '@/components/layouts/AppShellHeader';
import { ServiceCard } from '@/components/serviceCard/components/ServiceCard';
import { applicationServices } from '@/config/applicationServices/applicationService';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const Finances = () => {
  const { translate } = useAppTranslations();

  return (
    <>
      <AppShellHeader
        title={translate('page-money-title', { defaultValue: 'Money' })}
        subtitle={translate('page-money-subtitle', {
          defaultValue: 'This month',
        })}
      />
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        <ServiceCard service={applicationServices.financeTransactions} />
      </div>
    </>
  );
};
