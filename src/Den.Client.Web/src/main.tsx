import { StrictMode, useEffect } from 'react';
import { createRoot } from 'react-dom/client';
import { createRouter, RouterProvider } from '@tanstack/react-router';
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools';
import { routeTree } from './routeTree.gen';
import './i18n'
import './index.css'
import { ThemeProvider } from './components/theme-provider';
import { Toaster } from '@/components/ui/sonner';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { client } from './lib/state/client/client.gen';
import { AuthProvider, useAuth } from './lib/state/auth';

const router = createRouter({ 
  routeTree,
  context: {
    auth: undefined!, // will be set in InnerApp
  },
});

declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router
  }
}

client.setConfig({
  baseURL: `${window.origin}/api`,
});
const queryClient = new QueryClient();

function InnerApp() {
  const auth = useAuth();

  // invalidate router whenever auth state changes so beforeLoad hooks re-run
  useEffect(() => {
    router.invalidate();
  }, [auth.isAuthenticated]);

  return (
    <>
      <RouterProvider router={router} context={{ auth }} />
      <TanStackRouterDevtools router={router} position="bottom-right" />
      <Toaster />
    </>
  );
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <ThemeProvider defaultTheme="dark">
          <InnerApp />
        </ThemeProvider>
      </AuthProvider>
      <ReactQueryDevtools />
    </QueryClientProvider>
  </StrictMode>,
)
