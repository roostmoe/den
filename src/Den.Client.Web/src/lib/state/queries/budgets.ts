import { useMutation, useQuery } from "@tanstack/react-query";
import { getV1BudgetsOptions, postV1BudgetsMutation } from "../client/@tanstack/react-query.gen";

export const useBudgetsListQuery = () => {
  return useQuery({
    ...getV1BudgetsOptions(),
  });
};

export const useBudgetsCreateMutation = () => {
  return useMutation({
    ...postV1BudgetsMutation(),
  });
};
