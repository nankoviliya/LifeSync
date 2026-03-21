import { Link } from 'react-router-dom';

import { routePaths } from '@/config/routing/routePaths';

export const HeaderUnauthenticatedButtons = () => {
  return (
    <div className="flex items-center gap-4">
      <Link
        className="inline-block cursor-pointer rounded-md bg-primary px-4 py-2 text-base font-medium text-primary-foreground no-underline transition-colors hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-ring"
        to={routePaths.login.path}
      >
        {routePaths.login.name}
      </Link>
      <Link
        className="inline-block cursor-pointer rounded-md bg-emerald-600 px-4 py-2 text-base font-medium text-white no-underline transition-colors hover:bg-emerald-700 focus:outline-none focus:ring-2 focus:ring-ring"
        to={routePaths.register.path}
      >
        {routePaths.register.name}
      </Link>
    </div>
  );
};
