import { Breadcrumb, BreadcrumbList, BreadcrumbItem, BreadcrumbLink, BreadcrumbSeparator, BreadcrumbPage } from '@/components/ui/breadcrumb';
import { SidebarTrigger } from '@/components/ui/sidebar';
import { Separator } from '@/components/ui/separator';
import { createFileRoute, Link } from '@tanstack/react-router'
import { useTranslation } from 'react-i18next';
import { CalendarView, type CalendarEvent } from '@/components/views/calendar/calendar-view';
import { EventCard } from '@/components/views/calendar/event-card';
import { useState } from 'react';
import { Empty, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from '@/components/ui/empty';
import { CalendarIcon } from 'lucide-react';

export const Route = createFileRoute('/(app)/_home/calendar/')({
  component: () => {
    const [events, _setEvents] = useState<CalendarEvent[]>([
      { id: 'abc123', title: 'Flower Delivery', date: new Date() },
      { id: 'def456', title: 'Meeting with Bob', date: new Date() },
    ]);
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
          <div className="grid flex-1 grid-cols-1 lg:grid-cols-3 gap-4">
            <div className="flex flex-col gap-6">
              {events.length === 0 && (
                <Empty>
                  <EmptyHeader>
                    <EmptyMedia variant="icon">
                      <CalendarIcon />
                    </EmptyMedia>
                    <EmptyTitle>No Events</EmptyTitle>
                    <EmptyDescription>
                      You have no events scheduled. Your events will appear here.
                    </EmptyDescription>
                  </EmptyHeader>
                </Empty>
              )}
              {events.map((ev, idx) => <EventCard key={idx} event={ev} />)}
            </div>
            <div className="lg:col-span-2">
              <CalendarView events={events} onEventClick={() => {}} />
            </div>
          </div>
        </div>
      </>
    );
  }
});
