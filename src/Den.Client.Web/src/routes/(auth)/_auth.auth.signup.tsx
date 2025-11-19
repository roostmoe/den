import { Button } from '@/components/ui/button';
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardAction } from '@/components/ui/card';
import { SignupForm } from '@/components/views/auth/signup-form';
import { createFileRoute, Link } from '@tanstack/react-router'
import { ExternalLink } from 'lucide-react';
import { useTranslation } from 'react-i18next';

export const Route = createFileRoute('/(auth)/_auth/auth/signup')({
  component: () => {
    const { t } = useTranslation();

    return (
      <>
        <title>{`${t('signup.page.title')} — ${t('appTitle')}`}</title>
        <Card>
          <CardHeader>
            <CardTitle>{t('signup.form.title')}</CardTitle>
            <CardDescription>{t('signup.form.subtitle')}</CardDescription>
            <CardAction>
              <Button variant="link" asChild>
                <Link to="/auth/login">{t('login.page.title')} <ExternalLink /></Link>
              </Button>
            </CardAction>
          </CardHeader>

          <CardContent>
            <SignupForm />
          </CardContent>
        </Card>
      </>
    );
  },
});
