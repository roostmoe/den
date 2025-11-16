import { Button } from '@/components/ui/button';
import { Card, CardAction, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { LoginForm } from '@/components/views/auth/login-form';
import { createFileRoute, Link } from '@tanstack/react-router';
import { ExternalLink } from 'lucide-react';
import { useTranslation } from 'react-i18next';

export const Route = createFileRoute('/(auth)/_auth/auth/login')({
  component: () => {
    const { t } = useTranslation();

    return (
      <>
        <title>{`${t('login.page.title')} — ${t('appTitle')}`}</title>
        <Card>
          <CardHeader>
            <CardTitle>{t('login.form.title')}</CardTitle>
            <CardDescription>{t('login.form.subtitle')}</CardDescription>
            <CardAction>
              <Button variant="link" asChild>
                <Link to="/auth/signup">{t('signup.page.title')} <ExternalLink /></Link>
              </Button>
            </CardAction>
          </CardHeader>

          <CardContent>
            <LoginForm />
          </CardContent>
        </Card>
      </>
    );
  },
});
