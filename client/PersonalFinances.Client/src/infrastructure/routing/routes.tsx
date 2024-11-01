import App from '@App';
import { RouteObject } from 'react-router-dom';
import { routePaths } from '@infrastructure/routing/routePaths';

export const routes: RouteObject[] = [
  {
    path: routePaths.home.path,
    element: <App />,
  },
];
