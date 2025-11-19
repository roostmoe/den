import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'http://localhost:8080/api/openapi/v1.json',
  output: 'src/lib/state/client',
  plugins: ['@hey-api/client-axios', '@tanstack/react-query'],
});
