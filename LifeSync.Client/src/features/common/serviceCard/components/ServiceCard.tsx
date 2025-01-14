import { Card } from 'primereact/card';
import styles from './ServiceCard.module.scss';
import { useNavigate } from 'react-router-dom';
import { routePaths } from '@/infrastructure/routing/routePaths';
import { IApplicationService } from '@/infrastructure/applicationServices/applicationService';
import { useAppTranslations } from '@/infrastructure/translations/hooks/useAppTranslations';

export interface IServiceCardProps {
  service: IApplicationService;
}

export const ServiceCard = ({ service }: IServiceCardProps) => {
  const { translate } = useAppTranslations();
  const navigate = useNavigate();

  return (
    <Card
      className={styles['service-card']}
      title={translate(service.translationCode)}
      onClick={() => {
        navigate(service.routePath.path);
      }}
    >
      <p className="m-0">{service.description}</p>
    </Card>
  );
};
