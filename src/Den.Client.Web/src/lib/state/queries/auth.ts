import { useMutation, useQuery } from "@tanstack/react-query"
import { getV1AuthMeOptions, postV1AuthLoginMutation } from "../client/@tanstack/react-query.gen"
import { useAuth } from "../auth";

export const useMeQuery = () => {
  const { tokens } = useAuth();
  
  return useQuery({
    ...getV1AuthMeOptions({
      auth: tokens?.accessToken,
    }),
    enabled: !!tokens?.accessToken,
  });
};

export const useLoginMutation = () => {
  const { login } = useAuth();

  return useMutation({
    ...postV1AuthLoginMutation(),
    onSuccess: (data) => {
      login({
        accessToken: data.accessToken,
        refreshToken: data.refreshToken,
      });
    },
  });
}
