import { useNavigate } from 'react-router-dom';

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { IApplicationService } from '@/config/applicationServices/applicationService';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export interface IServiceCardProps {
  service: IApplicationService;
}

export const ServiceCard = ({ service }: IServiceCardProps) => {
  const { translate } = useAppTranslations();
  const navigate = useNavigate();

  return (
    <Card
      className="cursor-pointer transition-shadow hover:shadow-lg"
      onClick={() => {
        navigate(service.routePath.path);
      }}
    >
      <CardHeader>
        <CardTitle>{translate(service.labelTranslationCode)}</CardTitle>
      </CardHeader>
      <CardContent>
        <CardDescription>
          {translate(service.descriptionTranslationCode)}
        </CardDescription>
      </CardContent>
    </Card>
  );
};
