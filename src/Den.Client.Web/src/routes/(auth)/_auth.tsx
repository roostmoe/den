import { createFileRoute, Outlet, redirect } from '@tanstack/react-router'
import { useTranslation } from 'react-i18next';
import { z } from 'zod';

const authSearchSchema = z.object({
  redirect: z.string().optional(),
});

export const Route = createFileRoute('/(auth)/_auth')({
  validateSearch: authSearchSchema,
  beforeLoad: ({ context, search }) => {
    if (context.auth.isAuthenticated) {
      throw redirect({ to: search.redirect || '/' });
    }
  },
  component: RouteComponent,
})

function RouteComponent() {
  const { t } = useTranslation();

  return (
    <div className="bg-muted flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
      <div className="flex w-full max-w-sm flex-col gap-6">
        <a href="/" className="flex items-center gap-2 self-center font-medium">
          {t('appTitle')}
        </a>
        <Outlet />
      </div>
    </div>
  );
}
