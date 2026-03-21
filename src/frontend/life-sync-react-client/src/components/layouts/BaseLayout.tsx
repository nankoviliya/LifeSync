import { PropsWithChildren } from 'react';

import { Footer } from '@/components/footer/components/Footer';
import { Header } from '@/components/header/components/Header';

export const BaseLayout = ({ children }: PropsWithChildren) => {
  return (
    <div className="grid min-h-dvh grid-cols-[minmax(0,1fr)] grid-rows-[auto_1fr_auto]">
      <Header />
      <main className="m-4 flex justify-center">{children}</main>
      <Footer />
    </div>
  );
};
