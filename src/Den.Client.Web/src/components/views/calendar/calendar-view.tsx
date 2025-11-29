import { cn } from "@/lib/utils";
import { useState } from "react";

export type CalendarEvent = {
  id: string;
  title: string;
  date: Date;
};

export type CalendarProps = {
  events: CalendarEvent[];
  onEventClick: (event: CalendarEvent) => void;
};

export const CalendarView = ({ events, onEventClick }: CalendarProps) => {
  const [currentDate, setCurrentDate] = useState(new Date())

  const year = currentDate.getFullYear()
  const month = currentDate.getMonth()

  const firstDayOfMonth = new Date(year, month, 1)
  const lastDayOfMonth = new Date(year, month + 1, 0)
  const startingDayOfWeek = firstDayOfMonth.getDay()
  const daysInMonth = lastDayOfMonth.getDate()

  const previousMonth = () => {
    setCurrentDate(new Date(year, month - 1, 1))
  }

  const nextMonth = () => {
    setCurrentDate(new Date(year, month + 1, 1))
  }

  const getEventsForDay = (day: number) => {
    return events.filter((event) => {
      const eventDate = new Date(event.date)
      return eventDate.getDate() === day && eventDate.getMonth() === month && eventDate.getFullYear() === year
    })
  }

  const isToday = (day: number) => {
    const today = new Date()
    return today.getDate() === day && today.getMonth() === month && today.getFullYear() === year
  }

  // Create array of day cells including empty cells for alignment
  const calendarDays = []

  // Add empty cells for days before the first day of month
  for (let i = 0; i < startingDayOfWeek; i++) {
    calendarDays.push(null)
  }

  // Add cells for each day of the month
  for (let day = 1; day <= daysInMonth; day++) {
    calendarDays.push(day)
  }

  return (
    <div className="grid grid-cols-7 gap-2">
      {calendarDays.map((day, index) => {
        if (day === null) {
          return <div key={`empty-${index}`} className="aspect-square" />
        }

        const dayEvents = getEventsForDay(day);
        const isTodayDate = isToday(day);

        return (
          <div
            key={day}
            className={cn(
              "aspect-square border rounded-lg p-2 hover:border-primary/50 transition-colors",
              isTodayDate && "bg-primary/5 border-primary"
            )}
          >
            <div className="h-full flex flex-col">
              <span className={cn("text-sm font-medium mb-1", isTodayDate && "text-primary font-semibold")}>{day}</span>

              <div className="flex-1 flex flex-col gap-1 overflow-hidden">
                {dayEvents.slice(0, 3).map((event) => (
                  <button
                    key={event.id}
                    onClick={() => onEventClick(event)}
                    className={cn(
                      "text-left text-xs px-2 py-1 rounded truncate transition-all hover:scale-105",
                    )}
                  >
                    {event.title}
                  </button>
                ))}
              </div>
            </div>

          </div>
        );
      })}
    </div>
  );
};
