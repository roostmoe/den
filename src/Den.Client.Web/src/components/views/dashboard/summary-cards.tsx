import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { CalendarIcon, ClapperboardIcon, LightbulbIcon, ShoppingBasketIcon } from "lucide-react";
import { useTranslation } from "react-i18next";

export const SummaryCards = () => {
  const { t } = useTranslation();

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      <Card>
        <CardHeader>
          <div className="flex items-center gap-4">
            <ShoppingBasketIcon />
            <CardTitle>{t('dashboard.groceriesCountTitle')}</CardTitle>
          </div>
        </CardHeader>
        <CardContent>
          <span className="text-3xl font-bold">14</span>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <div className="flex items-center gap-4">
            <LightbulbIcon />
            <CardTitle>{t('dashboard.remindersCountTitle')}</CardTitle>
          </div>
        </CardHeader>
        <CardContent>
          <span className="text-3xl font-bold">10</span>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <div className="flex items-center gap-4">
            <CalendarIcon />
            <CardTitle>{t('dashboard.calendarEventsTitle')}</CardTitle>
          </div>
        </CardHeader>
        <CardContent className="flex items-center gap-2">
          <span className="text-3xl font-bold">3</span>
          <span className="text-sm">{t('dashboard.calendarEventsCountSubtext')}</span>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <div className="flex items-center gap-4">
            <ClapperboardIcon />
            <CardTitle>{t('dashboard.watchlistCountTitle')}</CardTitle>
          </div>
        </CardHeader>
        <CardContent className="flex items-end gap-2">
          <span className="text-3xl font-bold">2</span>
          <span className="text-sm">{t('dashboard.watchlistCountSubtext')}</span>
        </CardContent>
      </Card>
    </div>
  );
};
