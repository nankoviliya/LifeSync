import { Outlet } from 'react-router-dom';

import { BaseLayout } from '@/components/layouts/BaseLayout';

export const AppRoot = () => {
  return (
    <BaseLayout>
      <Outlet />
    </BaseLayout>
  );
};
