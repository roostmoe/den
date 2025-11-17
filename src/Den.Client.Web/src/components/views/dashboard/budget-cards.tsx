import type { ChartConfig } from "@/components/ui/chart";
import { Card, CardContent } from '@/components/ui/card';
import { ChartContainer } from "@/components/ui/chart";
import { PolarAngleAxis, RadialBar, RadialBarChart } from "recharts";
import { Button } from '@/components/ui/button';
import { useTranslation } from "react-i18next";

const data = [
  {
    name: "Household",
    progress: 25,
    budget: "£1,000",
    current: "£250",
    href: "#",
    fill: "var(--chart-1)",
  },
  {
    name: "Groceries",
    progress: 55,
    budget: "£1,000",
    current: "£550",
    href: "#",
    fill: "var(--chart-2)",
  },
  {
    name: "Bills & Services",
    progress: 85,
    budget: "£1,000",
    current: "£850",
    href: "#",
    fill: "var(--chart-3)",
  },
  {
    name: "Disposable Income",
    progress: 70,
    budget: "£2,000",
    current: "£1,400",
    href: "#",
    fill: "var(--chart-4)",
  },
];

const chartConfig = {
  progress: {
    label: "Progress",
    color: "var(--primary)",
  },
} satisfies ChartConfig;

export const BudgetCards = () => {
  const { t } = useTranslation();

  return (
    <div className="flex items-center justify-center w-full">
      <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4 w-full">
        {data.map((item) => (
          <Card key={item.name} className="p-0 gap-0">
            <CardContent className="p-4">
              <div className="flex items-center space-x-3">
                <div className="relative flex items-center justify-center">
                  <ChartContainer
                    config={chartConfig}
                    className="h-[80px] w-[80px]"
                  >
                    <RadialBarChart
                      data={[item]}
                      innerRadius={30}
                      outerRadius={60}
                      barSize={6}
                      startAngle={90}
                      endAngle={-270}
                    >
                      <PolarAngleAxis
                        type="number"
                        domain={[0, 100]}
                        angleAxisId={0}
                        tick={false}
                        axisLine={false}
                      />
                      <RadialBar
                        dataKey="progress"
                        background
                        cornerRadius={10}
                        fill={item.fill}
                        angleAxisId={0}
                      />
                    </RadialBarChart>
                  </ChartContainer>
                  <div className="absolute inset-0 flex items-center justify-center">
                    <span className="text-base font-medium text-foreground">
                      {item.progress}%
                    </span>
                  </div>
                </div>
                <div>
                  <dd className="text-base font-medium text-foreground">
                    {item.current} / {item.budget}
                  </dd>
                  <dt className="text-sm text-muted-foreground">
                    {item.name}
                  </dt>
                </div>

                <Button variant="outline" asChild className="ml-auto">
                  <a href={item.href}>
                    {t('misc.doViewMore')}
                  </a>
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </dl>
    </div>
  );
};
