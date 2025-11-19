import { Button } from "@/components/ui/button";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "@/components/ui/empty";
import { Skeleton } from "@/components/ui/skeleton";
import { useBudgetsListQuery } from "@/lib/state/queries/budgets";
import { AlertCircle, CirclePoundSterlingIcon } from "lucide-react";
import { BudgetsListTable } from "./table";
import { Link } from "@tanstack/react-router";

export const BudgetsList = () => {
  const { data, isLoading, isPending, isError } = useBudgetsListQuery();

  if (isLoading || isPending)
    return (
      <Skeleton className="w-[100px]" />
    );

  if (isError) {
    return <AlertCircle />
  }

  if (data.length === 0) return <BudgetsListEmptyState />;

  return <BudgetsListTable data={data} />;
};

export const BudgetsListEmptyState = () => {
  return (
    <div className="flex flex-1 flex-col items-center justify-center gap-4">

      <Empty>
        <EmptyHeader>
          <EmptyMedia variant="icon">
            <CirclePoundSterlingIcon />
          </EmptyMedia>
          <EmptyTitle>No budgets yet</EmptyTitle>
          <EmptyDescription>You haven't created any budgets yet. Get started by creating your first budget.</EmptyDescription>
        </EmptyHeader>
        <EmptyContent>
          <div className="flex gap-2">
            <Button asChild>
              <Link to="/budgets/create">
                Create Budget
              </Link>
            </Button>
          </div>
        </EmptyContent>
      </Empty>

    </div>
  );
};
