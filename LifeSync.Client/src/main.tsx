import { StrictMode } from 'react';
import './index.scss';
import { createRoot } from 'react-dom/client';

import 'primereact/resources/themes/lara-light-blue/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css'; // Icons

import { App } from '@/app/App';

const appRoot = document.getElementById('root')!;

const root = createRoot(appRoot);

root.render(
  <StrictMode>
    <App />
  </StrictMode>,
);
