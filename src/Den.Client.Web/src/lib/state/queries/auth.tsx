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
      console.log('[login] mutation success, raw data:', data);
      console.log('[login] checking tokens:', { 
        hasAccessToken: !!data?.accessToken, 
        hasRefreshToken: !!data?.refreshToken,
        dataKeys: Object.keys(data || {})
      });
      
      if (data?.accessToken && data?.refreshToken) {
        console.log('[login] calling login() with tokens');
        login({
          accessToken: data.accessToken,
          refreshToken: data.refreshToken,
        });
      } else {
        console.error('[login] missing tokens in response!', data);
      }
    },
  });
}
