import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList } from '@/components/ui/breadcrumb';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { SidebarTrigger } from '@/components/ui/sidebar';
import { createFileRoute, Link } from '@tanstack/react-router'
import { useTranslation } from 'react-i18next';
import { BudgetCards } from '@/components/views/dashboard/budget-cards';
import { SummaryCards } from '@/components/views/dashboard/summary-cards';

export const Route = createFileRoute('/(app)/_home/')({
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
                  <Link to="/">{t('dashboard.title')}</Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </header>

        <div className="flex flex-1 flex-col gap-4 p-4">
          <SummaryCards />
          <BudgetCards />
          <Separator />

          <div className="grid lg:grid-cols-2 gap-4">
            <Card>
              <CardHeader>
                <CardTitle>Shopping list</CardTitle>
              </CardHeader>
              <CardContent>
                Table here
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Reminders</CardTitle>
              </CardHeader>
              <CardContent>
                Table here
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Calendar</CardTitle>
              </CardHeader>
              <CardContent>
                Table here
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Upcoming movies</CardTitle>
              </CardHeader>
              <CardContent>
                Table here
              </CardContent>
            </Card>
          </div>
        </div>
      </>
    );
  }
});
