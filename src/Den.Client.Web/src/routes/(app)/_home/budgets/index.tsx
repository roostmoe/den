import { Breadcrumb, BreadcrumbList, BreadcrumbItem, BreadcrumbLink, BreadcrumbSeparator, BreadcrumbPage } from '@/components/ui/breadcrumb';
import { SidebarTrigger } from '@/components/ui/sidebar';
import { Separator } from '@/components/ui/separator';
import { createFileRoute, Link } from '@tanstack/react-router'
import { BudgetsList } from '@/components/views/budgets/list/budgets-list';
import { useTranslation } from 'react-i18next';
import { Button } from '@/components/ui/button';
import { PlusIcon } from 'lucide-react';

export const Route = createFileRoute('/(app)/_home/budgets/')({
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
                  <Link to="/budgets">{t('menu.budgeting.title')}</Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage>
                  {t('menu.budgeting.allBudgets')}
                </BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </header>

        <div className="flex flex-1 flex-col gap-4 p-4">
          <div className="flex justify-between items-center">
            <div className="flex flex-col gap-2">
              <h1 className="text-2xl font-semibold">{t('menu.budgeting.allBudgets')}</h1>
              <p className="text-muted-foreground">Track your finances using Den's Budgeting feature. Below are any of your configured budgets.</p>
            </div>
            <Button variant="outline" asChild>
              <Link to="/budgets/create">
                <PlusIcon />
                {t('budgeting.create.doNewBudget')}
              </Link>
            </Button>
          </div>

          <Separator />

          <BudgetsList />
        </div>
      </>
    )
  },
});
