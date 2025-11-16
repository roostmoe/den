import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { createRouter, RouterProvider } from '@tanstack/react-router';
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools';
import { routeTree } from './routeTree.gen';
import './i18n'
import './index.css'
import { ThemeProvider } from './components/theme-provider';
import { Toaster } from '@/components/ui/sonner';

const router = createRouter({ routeTree });

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider defaultTheme="dark">
      <RouterProvider router={router} />
      <TanStackRouterDevtools router={router} />
      <Toaster />
    </ThemeProvider>
  </StrictMode>,
)
