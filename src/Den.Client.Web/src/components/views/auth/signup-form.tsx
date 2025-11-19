import { useTranslation } from 'react-i18next';
import { useForm } from '@tanstack/react-form';
import { z } from 'zod';

import { Button } from '@/components/ui/button';
import { Field, FieldDescription, FieldError, FieldGroup } from '@/components/ui/field';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { toast } from 'sonner';

const formSchema = z
  .object({
    username: z.string().nonempty('Please enter your username.'),
    email: z.email().nonempty('Please enter your email.'),
    password: z.string().nonempty('Please enter your password.'),
    passwordConfirm: z.string().nonempty('Confirm your password.'),
  })
  .superRefine(({ password, passwordConfirm }, ctx) => {
    if (passwordConfirm !== password) ctx.addIssue({
      code: 'custom',
      message: 'The passwords did not match.',
      path: ['passwordConfirm'],
    });
  });

export const SignupForm = () => {
  const { t } = useTranslation();

  const form = useForm({
    defaultValues: {
      username: '',
      email: '',
      password: '',
      passwordConfirm: '',
    },
    validators: {
      onSubmit: formSchema,
    },
    onSubmit: async ({ value }) => {
      toast.success('Successfully signed up!', {
        description: `Signed up as ${value.username} (${value.email}), password ${value.password}.`,
      });
    },
  });

  return (
    <form
      id="signup-form"
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
                <Label htmlFor="username">{t('signup.form.usernameLabel')}</Label>
                <Input
                  id="username"
                  name={field.name}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={e => field.handleChange(e.target.value)}
                  autoFocus
                  autoComplete="username"
                  tabIndex={0}
                  aria-invalid={isInvalid}
                />
                <FieldDescription>{t('signup.form.usernameDescription')}</FieldDescription>
                {isInvalid && <FieldError errors={field.state.meta.errors} />}
              </Field>
            );
          }}
        </form.Field>

        <form.Field name="email">
          {(field) => {
            const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
            return (
              <Field data-invalid={isInvalid}>
                <Label htmlFor="email">{t('signup.form.emailLabel')}</Label>
                <Input
                  id="email"
                  type="email"
                  name={field.name}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={e => field.handleChange(e.target.value)}
                  autoComplete="email"
                  tabIndex={1}
                  aria-invalid={isInvalid}
                />
                <FieldDescription>{t('signup.form.emailDescription')}</FieldDescription>
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
                <Label htmlFor="password">{t('signup.form.passwordLabel')}</Label>
                <Input
                  id="password"
                  type="password"
                  name={field.name}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={e => field.handleChange(e.target.value)}
                  autoComplete="new-password"
                  tabIndex={2}
                  aria-invalid={isInvalid}
                />
                {isInvalid && <FieldError errors={field.state.meta.errors} />}
              </Field>
            );
          }}
        </form.Field>

        <form.Field name="passwordConfirm">
          {(field) => {
            const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
            return (
              <Field data-invalid={isInvalid}>
                <Label htmlFor="passwordConfirm">{t('signup.form.passwordConfirmLabel')}</Label>
                <Input
                  id="passwordConfirm"
                  type="password"
                  name={field.name}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={e => field.handleChange(e.target.value)}
                  autoComplete="new-password"
                  tabIndex={3}
                  aria-invalid={isInvalid}
                />
                {isInvalid && <FieldError errors={field.state.meta.errors} />}
              </Field>
            );
          }}
        </form.Field>

        <Button type="submit" tabIndex={4}>{t('signup.form.doSignup')}</Button>
      </FieldGroup>
    </form>
  );
};
