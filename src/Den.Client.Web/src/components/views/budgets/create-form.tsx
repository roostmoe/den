import { Button } from "@/components/ui/button";
import { Field, FieldDescription, FieldError, FieldGroup, FieldLabel, FieldLegend, FieldSeparator, FieldSet } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectTrigger, SelectValue, SelectItem } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { getV1BudgetsQueryKey } from "@/lib/state/client/@tanstack/react-query.gen";
import { useBudgetsCreateMutation } from "@/lib/state/queries/budgets";
import { useForm } from "@tanstack/react-form";
import { useQueryClient } from "@tanstack/react-query";
import { useRouter } from "@tanstack/react-router";
import { t } from "i18next";
import z from "zod";

const formSchema = z.object({
  displayName: z.string().nonempty('Please enter your username.'),
  description: z.string().max(255),
  currency: z.enum(['USD', 'EUR', 'GBP', 'COP']),
  total: z.int().min(0),
});

export const CreateBudgetForm = () => {
  const { mutateAsync } = useBudgetsCreateMutation();
  const queryClient = useQueryClient();
  const router = useRouter();

  const form = useForm({
    defaultValues: {
      displayName: '',
      description: '',
      currency: 'GBP',
      total: 0,
    },
    validators: {
      onSubmit: formSchema,
    },
    onSubmit: async ({ value }) => {
      await mutateAsync({
        body: {
          displayName: value.displayName,
          description: value.description,
          currency: value.currency,
          total: value.total,
          period: 'Monthly',
        },
      });

      queryClient.invalidateQueries({ queryKey: getV1BudgetsQueryKey() });

      router.navigate({ to: '/budgets' });
    },
  });

  return (
    <form
      id="create-budget-form"
      onSubmit={e => {
        e.preventDefault();
        form.handleSubmit();
      }}
    >
      <FieldGroup>
        <FieldSet>
          <form.Field name="displayName">
            {field => {
              const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
              return (
                <Field data-invalid={isInvalid}>
                  <FieldLabel htmlFor="displayName">
                    {t('budgeting.create.form.displayNameLabel')}
                  </FieldLabel>
                  <Input
                    id="displayName"
                    name={field.name}
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={e => field.handleChange(e.target.value)}
                    autoFocus
                    tabIndex={1}
                    aria-invalid={isInvalid}
                  />
                  <FieldDescription>What should we call this budget?</FieldDescription>
                  {isInvalid && <FieldError errors={field.state.meta.errors} />}
                </Field>
              );
            }}
          </form.Field>

          <form.Field name="description">
            {field => {
              const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
              return (
                <Field data-invalid={isInvalid}>
                  <FieldLabel htmlFor="description">
                    {t('budgeting.create.form.descriptionLabel')}
                    <span className="text-muted-foreground">{t('optional')}</span>
                  </FieldLabel>
                  <Textarea
                    id="description"
                    name={field.name}
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={e => field.handleChange(e.target.value)}
                    tabIndex={2}
                    aria-invalid={isInvalid}
                  />
                  <FieldDescription>Is there any information you want to display alongside your budget?</FieldDescription>
                  {isInvalid && <FieldError errors={field.state.meta.errors} />}
                </Field>
              );
            }}
          </form.Field>
        </FieldSet>

        <FieldSeparator />

        <FieldSet>
          <FieldLegend>{t('budgeting.create.form.scope.title')}</FieldLegend>
          <FieldDescription>{t('budgeting.create.form.scope.subtitle')}</FieldDescription>

          <div className="grid grid-cols-1 md:grid-cols-[3fr_1fr] gap-4">
            <form.Field name="total">
              {field => {
                const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                return (
                  <Field data-invalid={isInvalid}>
                    <FieldLabel htmlFor="total">{t('budgeting.create.form.scope.totalLabel')}</FieldLabel>
                    <Input
                      id="total"
                      type="number"
                      name={field.name}
                      value={field.state.value}
                      onBlur={field.handleBlur}
                      onChange={e => field.handleChange(parseFloat(e.target.value))}
                      tabIndex={3}
                      aria-invalid={isInvalid}
                    />
                    <FieldDescription>How much will this budget let you spend?</FieldDescription>
                    {isInvalid && <FieldError errors={field.state.meta.errors} />}
                  </Field>
                );
              }}
            </form.Field>

            <form.Field name="currency">
              {field => {
                const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                return (
                  <Field data-invalid={isInvalid}>
                    <FieldLabel htmlFor="currency">{t('budgeting.create.form.scope.currencyLabel')}</FieldLabel>
                    <Select
                      defaultValue="GBP"
                      onValueChange={value => field.handleChange(value)}
                    >
                      <SelectTrigger id="currency" tabIndex={4}>
                        <SelectValue placeholder="ABC" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="USD">{t('budgeting.currencies.USD')}</SelectItem>
                        <SelectItem value="EUR">{t('budgeting.currencies.EUR')}</SelectItem>
                        <SelectItem value="GBP">{t('budgeting.currencies.GBP')}</SelectItem>
                        <SelectItem value="COP">{t('budgeting.currencies.COP')}</SelectItem>
                      </SelectContent>
                    </Select>
                    {isInvalid && <FieldError errors={field.state.meta.errors} />}
                  </Field>
                );
              }}
            </form.Field>
          </div>
        </FieldSet>
        <Field orientation="horizontal">
          <Button type="submit" form="create-budget-form" disabled={form.state.isSubmitting}>
            {t('submit')}
          </Button>
        </Field>
      </FieldGroup>

    </form>
  );
};
