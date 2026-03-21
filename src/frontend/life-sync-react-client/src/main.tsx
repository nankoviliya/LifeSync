import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';

import './app.css';

import { App } from '@/app/App';
import { initializeTheme } from '@/lib/theme';

const appRoot = document.getElementById('root')!;

initializeTheme();

const root = createRoot(appRoot);

root.render(
  <StrictMode>
    <App />
  </StrictMode>,
);
