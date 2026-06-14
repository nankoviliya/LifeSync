import { Controller } from 'react-hook-form';
import { Link } from 'react-router-dom';

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { PasswordInput } from '@/components/ui/password-input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Spinner } from '@/components/ui/spinner';
import { routePaths } from '@/config/routing/routePaths';
import { useRegistration } from '@/features/auth/register/hooks/useRegistration';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useFrontendSettings } from '@/hooks/useFrontendSettings';

export const Register = () => {
  const { translate } = useAppTranslations();
  const { control, onSubmit, isSubmitting, isError } = useRegistration();
  const {
    frontendSettings,
    isLoading: isSettingsLoading,
    isError: isSettingsError,
    refetch: refetchSettings,
  } = useFrontendSettings();

  if (isSettingsLoading) {
    return (
      <div className="flex min-h-[300px] items-center justify-center">
        <Spinner />
      </div>
    );
  }

  if (isSettingsError || !frontendSettings) {
    return (
      <div
        role="alert"
        className="flex flex-col items-start gap-3 rounded-md border border-destructive/40 bg-destructive/5 p-4 text-[13px] text-destructive"
      >
        <span>
          {translate('auth-settings-load-error', {
            defaultValue:
              "Couldn't load registration options. Please try again.",
          })}
        </span>
        <Button
          type="button"
          variant="outline"
          size="sm"
          onClick={() => refetchSettings()}
        >
          {translate('auth-settings-retry', { defaultValue: 'Retry' })}
        </Button>
      </div>
    );
  }

  const errorMessage = isError
    ? translate('auth-register-failed', {
        defaultValue:
          "We couldn't create your account. Please check your details and try again.",
      })
    : null;

  return (
    <form className="flex flex-col" onSubmit={onSubmit} noValidate>
      <div className="mb-3 font-mono text-[11.5px] uppercase tracking-[0.1em] text-primary">
        {translate('auth-register-eyebrow', { defaultValue: 'Create account' })}
      </div>
      <h1 className="m-0 text-[30px] font-semibold leading-[1.1] tracking-[-0.03em]">
        {translate('auth-register-title', { defaultValue: 'Get started' })}
        <br />
        <span className="font-medium text-muted-foreground">
          {translate('auth-register-subtitle', {
            defaultValue: 'in under a minute.',
          })}
        </span>
      </h1>

      <div className="mt-7 flex flex-col gap-3.5">
        <div className="grid grid-cols-1 gap-3.5 sm:grid-cols-2">
          <Controller
            name="firstName"
            control={control}
            rules={{
              required: translate('auth-firstname-required', {
                defaultValue: 'First name is required.',
              }),
            }}
            render={({ field, fieldState }) => (
              <FieldShell
                label={translate('auth-register-firstname-label', {
                  defaultValue: 'First name',
                })}
                error={fieldState.error?.message}
              >
                <Input
                  id={field.name}
                  autoComplete="given-name"
                  placeholder={translate(
                    'auth-register-firstname-placeholder',
                    {
                      defaultValue: 'Iliya',
                    },
                  )}
                  {...field}
                  autoFocus
                  aria-invalid={fieldState.invalid}
                />
              </FieldShell>
            )}
          />
          <Controller
            name="lastName"
            control={control}
            rules={{
              required: translate('auth-lastname-required', {
                defaultValue: 'Last name is required.',
              }),
            }}
            render={({ field, fieldState }) => (
              <FieldShell
                label={translate('auth-register-lastname-label', {
                  defaultValue: 'Last name',
                })}
                error={fieldState.error?.message}
              >
                <Input
                  id={field.name}
                  autoComplete="family-name"
                  placeholder={translate('auth-register-lastname-placeholder', {
                    defaultValue: 'Nankov',
                  })}
                  {...field}
                  aria-invalid={fieldState.invalid}
                />
              </FieldShell>
            )}
          />
        </div>

        <Controller
          name="email"
          control={control}
          rules={{
            required: translate('auth-email-required', {
              defaultValue: 'Email is required.',
            }),
          }}
          render={({ field, fieldState }) => (
            <FieldShell
              label={translate('auth-register-email-label', {
                defaultValue: 'Email',
              })}
              error={fieldState.error?.message}
            >
              <Input
                id={field.name}
                type="email"
                autoComplete="email"
                placeholder={translate('auth-register-email-placeholder', {
                  defaultValue: 'you@example.com',
                })}
                {...field}
                aria-invalid={fieldState.invalid}
              />
            </FieldShell>
          )}
        />

        <Controller
          name="password"
          control={control}
          rules={{
            required: translate('auth-password-required', {
              defaultValue: 'Password is required.',
            }),
          }}
          render={({ field, fieldState }) => (
            <FieldShell
              label={translate('auth-register-password-label', {
                defaultValue: 'Password',
              })}
              hint={translate('auth-register-password-hint', {
                defaultValue: 'At least 8 characters',
              })}
              error={fieldState.error?.message}
            >
              <PasswordInput
                id={field.name}
                autoComplete="new-password"
                placeholder={translate('auth-register-password-placeholder', {
                  defaultValue: 'Enter a password',
                })}
                {...field}
                aria-invalid={fieldState.invalid}
              />
            </FieldShell>
          )}
        />

        <div className="grid grid-cols-1 gap-3.5 sm:grid-cols-2">
          <Controller
            name="balance"
            control={control}
            rules={{
              required: translate('auth-balance-required', {
                defaultValue: 'Initial balance is required.',
              }),
            }}
            render={({ field, fieldState }) => (
              <FieldShell
                label={translate('auth-register-balance-label', {
                  defaultValue: 'Initial balance',
                })}
                error={fieldState.error?.message}
              >
                <Input
                  id={field.name}
                  type="number"
                  placeholder="0"
                  value={field.value ?? ''}
                  onChange={(e) =>
                    field.onChange(
                      e.target.value ? Number(e.target.value) : null,
                    )
                  }
                  onBlur={field.onBlur}
                  ref={field.ref}
                  aria-invalid={fieldState.invalid}
                />
              </FieldShell>
            )}
          />
          <Controller
            name="currency"
            control={control}
            render={({ field }) => (
              <FieldShell
                label={translate('auth-register-currency-label', {
                  defaultValue: 'Currency',
                })}
              >
                <Select value={field.value} onValueChange={field.onChange}>
                  <SelectTrigger className="w-full" onBlur={field.onBlur}>
                    <SelectValue
                      placeholder={translate(
                        'auth-register-currency-placeholder',
                        { defaultValue: 'Select a currency' },
                      )}
                    />
                  </SelectTrigger>
                  <SelectContent>
                    {frontendSettings.currencyOptions.map((opt) => (
                      <SelectItem key={opt.code} value={opt.code}>
                        {opt.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </FieldShell>
            )}
          />
        </div>

        <Controller
          name="languageId"
          control={control}
          render={({ field }) => (
            <FieldShell
              label={translate('auth-register-language-label', {
                defaultValue: 'Language',
              })}
            >
              <Select value={field.value} onValueChange={field.onChange}>
                <SelectTrigger className="w-full" onBlur={field.onBlur}>
                  <SelectValue
                    placeholder={translate(
                      'auth-register-language-placeholder',
                      { defaultValue: 'Select a language' },
                    )}
                  />
                </SelectTrigger>
                <SelectContent>
                  {frontendSettings.languageOptions.map((opt) => (
                    <SelectItem key={opt.id} value={opt.id}>
                      {opt.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FieldShell>
          )}
        />

        {errorMessage && (
          <div
            role="alert"
            aria-live="polite"
            className="rounded-md border border-destructive/40 bg-destructive/5 px-3 py-2 text-[12.5px] text-destructive"
          >
            {errorMessage}
          </div>
        )}

        <Button
          type="submit"
          size="lg"
          loading={isSubmitting}
          className="mt-1 w-full"
        >
          {translate('register-button-label', {
            defaultValue: 'Create account',
          })}
        </Button>

        <div className="mt-1.5 text-center text-[12.5px] text-muted-foreground">
          {translate('auth-register-have-account', {
            defaultValue: 'Already have one?',
          })}{' '}
          <Link
            to={routePaths.login.path}
            className="font-medium text-primary hover:underline"
          >
            {translate('auth-register-sign-in', { defaultValue: 'Sign in' })}
          </Link>
        </div>
      </div>
    </form>
  );
};

interface FieldShellProps {
  label: string;
  hint?: string;
  error?: string;
  children: React.ReactNode;
}

const FieldShell = ({ label, hint, error, children }: FieldShellProps) => (
  <div className="flex flex-col gap-1.5">
    <span className="text-[11.5px] font-medium text-foreground/85">
      {label}
    </span>
    {children}
    {hint && !error && (
      <span className="text-[11.5px] text-muted-foreground">{hint}</span>
    )}
    {error && <span className="text-[11.5px] text-destructive">{error}</span>}
  </div>
);
