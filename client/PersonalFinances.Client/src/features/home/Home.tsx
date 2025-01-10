import { routePaths } from '@/infrastructure/routing/routePaths';
import { Link, useNavigate } from 'react-router-dom';
import styles from './Home.module.scss';
import { Card } from 'primereact/card';

export const Home = () => {
  const navigate = useNavigate();

  return (
    <div className={styles['home']}>
      <div className={styles['home__services-list']}>
        <Card
          className={styles['home__service-card']}
          title="Finances"
          onClick={() => {
            navigate(routePaths.finances.path);
          }}
        >
          <p className="m-0">
            Here you can manage your income and expense transations.
          </p>
        </Card>
      </div>
    </div>
  );
};
