import { useTranslation } from 'react-i18next';
import { useForm } from '@tanstack/react-form';
import { z } from 'zod';

import { Button } from '@/components/ui/button';
import { Field, FieldDescription, FieldError, FieldGroup } from '@/components/ui/field';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { toast } from 'sonner';
import { Link } from '@tanstack/react-router';
import { InvalidAuthException, useLoginMutation } from '@/lib/state/queries/auth/login';

const formSchema = z.object({
  username: z.string().nonempty('Please enter your username.'),
  password: z.string().nonempty('Please enter your password.'),
  rememberMe: z.boolean(),
});

export const LoginForm = () => {
  const { t } = useTranslation();
  const { mutateAsync } = useLoginMutation();

  const form = useForm({
    defaultValues: {
      username: '',
      password: '',
      rememberMe: false,
    },
    validators: {
      onSubmit: formSchema,
    },
    onSubmit: async ({ value, formApi }) => {
      try {
        await mutateAsync({ username: value.username, password: value.password });
      } catch (ex) {
        if (ex instanceof InvalidAuthException) {
          formApi.setErrorMap({
            onChange: {
              fields: {
                username: { message: 'Invalid username or password.' },
                password: { message: '' }
              }
            }
          });
        }
      }
    },
  });

  return (
    <form
      id="login-form"
      onSubmit={e => {
        e.preventDefault();
        form.handleSubmit();
      }}
    >
      <FieldGroup>
        <form.Field name="username">
          {(field) => {
            const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
            return (
              <Field data-invalid={isInvalid}>
                <Label htmlFor="username">{t('login.form.usernameLabel')}</Label>
                <Input
                  id="username"
                  name={field.name}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={e => field.handleChange(e.target.value)}
                  autoFocus
                  autoComplete="username"
                  tabIndex={1}
                  aria-invalid={isInvalid}
                />
                {isInvalid && <FieldError errors={field.state.meta.errors} />}
              </Field>
            );
          }}
        </form.Field>

        <form.Field name="password">
          {(field) => {
            const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
            return (
              <Field data-invalid={isInvalid}>
                <Label htmlFor="password">{t('login.form.passwordLabel')}</Label>
                <Input
                  id="password"
                  type="password"
                  name={field.name}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={e => field.handleChange(e.target.value)}
                  autoComplete="password"
                  tabIndex={2}
                  aria-invalid={isInvalid}
                />
                <FieldDescription>
                  <Link
                    to="#"
                  >
                    {t('login.form.forgotPassword')}
                  </Link>
                </FieldDescription>
                {isInvalid && <FieldError errors={field.state.meta.errors} />}
              </Field>
            );
          }}
        </form.Field>

        <form.Field name="rememberMe">
          {(field) => {
            const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
            return (
              <Field data-invalid={isInvalid}>
                <div className="flex items-center gap-3">
                  <Checkbox
                    id="rememberMe"
                    checked={field.state.value}
                    onCheckedChange={(checked) => field.handleChange(checked === true)}
                    tabIndex={3}
                    aria-invalid={isInvalid}
                  />
                  <Label htmlFor="rememberMe">{t('login.form.rememberLabel')}</Label>
                </div>
              </Field>
            );
          }}
        </form.Field>

        <Button type="submit" tabIndex={4}>{t('login.form.doLogin')}</Button>
      </FieldGroup>
    </form>
  );
};
