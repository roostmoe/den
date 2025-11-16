import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList } from '@/components/ui/breadcrumb';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { SidebarTrigger } from '@/components/ui/sidebar';
import { createFileRoute, Link } from '@tanstack/react-router'
import { ListIcon } from 'lucide-react';
import { useTranslation } from 'react-i18next';

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
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            <Card>
              <CardHeader>
                <div className="grid grid-cols-[auto_auto]">
                  <ListIcon />
                  <CardTitle> Groceries</CardTitle>
                </div>
              </CardHeader>
              <CardContent>
                <span className="text-3xl font-bold">14</span>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Reminders</CardTitle>
              </CardHeader>
              <CardContent>
                <span className="text-3xl font-bold">10</span>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Calendar Events</CardTitle>
              </CardHeader>
              <CardContent>
                <span className="text-3xl font-bold">3 today</span>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Notes</CardTitle>
              </CardHeader>
              <CardContent>
                <span className="text-3xl font-bold">32</span>
              </CardContent>
            </Card>
          </div>
          Home page
        </div>
      </>
    );
  }
});
