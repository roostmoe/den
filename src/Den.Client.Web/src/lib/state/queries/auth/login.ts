import { useMutation } from "@tanstack/react-query";

type AuthResponse = { token_type: 'Bearer', access_token: string, refresh_token: string, expires_in: number };

export class InvalidAuthException extends Error {}

export const useLoginMutation = () => {
  return useMutation<AuthResponse, Error, { username: string; password: string }>({
    mutationKey: ['auth.login'],
    mutationFn: async ({ username, password }) => {
      const resp = await fetch('/api/auth/v1/login', {
        method: 'POST',
        body: JSON.stringify({ username, password }),
        headers: {
          'Content-Type': 'application/json'
        },
      });

      if (!resp.ok) {
        const json = await resp.json() as { error: string };
        throw new InvalidAuthException(json.error);
      }

      const json = await resp.json() as AuthResponse;
      return json;
    },
  });
};
