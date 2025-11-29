import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import type { CalendarEvent } from "./calendar-view";

export const EventCard = ({ event }: { event: CalendarEvent; }) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{event.title}</CardTitle>
        <CardDescription>{event.date.toLocaleString()}</CardDescription>
      </CardHeader>
      <CardContent>
        <dl className="grid grid-cols-1 gap-4">
          <div className="flex flex-col gap-2">
            <dt className="font-medium">Location</dt>
            <dd>123 Flower St, Springfield</dd>
          </div>

          <dt className="font-medium">Notes</dt>
          <dd>Don't forget to bring the flowers!</dd>
        </dl>
      </CardContent>
    </Card>
  );
}
