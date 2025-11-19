import { Breadcrumb, BreadcrumbList, BreadcrumbItem, BreadcrumbLink, BreadcrumbSeparator, BreadcrumbPage } from '@/components/ui/breadcrumb';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { SidebarTrigger } from '@/components/ui/sidebar';
import { CreateBudgetForm } from '@/components/views/budgets/create-form';
import { Separator } from '@radix-ui/react-separator';
import { createFileRoute, Link } from '@tanstack/react-router'
import { useTranslation } from 'react-i18next';

export const Route = createFileRoute('/(app)/_home/budgets/create')({
  component: () => {
    const { t } = useTranslation();

    return (
      <>
        <header className="flex h-16 shrink-0 items-center gap-2 border-b px-4">
          <SidebarTrigger />
          <Separator orientation="vertical" className="data-[orientation=vertical]:h-4" />
          <Breadcrumb>
            <BreadcrumbList>
              <BreadcrumbItem className="hidden md:block">
                <BreadcrumbLink asChild>
                  <Link to="/budgets">
                    {t('menu.budgeting.title')}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage>
                  New Budget
                </BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </header>

        <div className="flex flex-1 justify-center p-4">
          <div className="flex flex-col w-full max-w-2xl gap-6">
            <Card>
              <CardHeader>
                <CardTitle>{t('budgeting.create.title')}</CardTitle>
                <CardDescription>{t('budgeting.create.description')}</CardDescription>
              </CardHeader>
              <CardContent>
                <CreateBudgetForm />
              </CardContent>
            </Card>
          </div>
        </div>
      </>
    );
  },
});
