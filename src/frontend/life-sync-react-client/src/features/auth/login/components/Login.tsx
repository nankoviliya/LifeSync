import { isAxiosError } from 'axios';
import { Controller } from 'react-hook-form';
import { Link } from 'react-router-dom';

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { PasswordInput } from '@/components/ui/password-input';
import { routePaths } from '@/config/routing/routePaths';
import { useLogin } from '@/hooks/auth/useLogin';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const Login = () => {
  const { translate } = useAppTranslations();
  const { control, onSubmit, isLoginPending, error, isError } = useLogin();

  const is401 =
    isError && isAxiosError(error) && error.response?.status === 401;

  const errorMessage = isError
    ? is401
      ? translate('auth-invalid-credentials', {
          defaultValue: 'Invalid email or password.',
        })
      : translate('auth-login-failed', {
          defaultValue:
            "We couldn't sign you in. Please check your details and try again.",
        })
    : null;

  return (
    <form className="flex flex-col" onSubmit={onSubmit} noValidate>
      <div className="mb-3 font-mono text-[11.5px] uppercase tracking-[0.1em] text-primary">
        {translate('auth-login-eyebrow', { defaultValue: 'Sign in' })}
      </div>
      <h1 className="m-0 text-[30px] font-semibold leading-[1.1] tracking-[-0.03em]">
        {translate('auth-login-title', { defaultValue: 'Welcome back,' })}
        <br />
        <span className="font-medium text-muted-foreground">
          {translate('auth-login-subtitle', {
            defaultValue: 'good to see you.',
          })}
        </span>
      </h1>

      <div className="mt-8 flex flex-col gap-3.5">
        <Controller
          name="email"
          control={control}
          rules={{
            required: translate('auth-email-required', {
              defaultValue: 'Email is required.',
            }),
          }}
          render={({ field, fieldState }) => (
            <div className="flex flex-col gap-1.5">
              <label
                htmlFor={field.name}
                className="text-[11.5px] font-medium text-foreground/85"
              >
                {translate('auth-login-email-label', { defaultValue: 'Email' })}
              </label>
              <Input
                id={field.name}
                type="email"
                autoComplete="email"
                placeholder={translate('auth-login-email-placeholder', {
                  defaultValue: 'you@example.com',
                })}
                {...field}
                autoFocus
                aria-invalid={fieldState.invalid || is401}
              />
              {fieldState.error && (
                <span className="text-[11.5px] text-destructive">
                  {fieldState.error.message}
                </span>
              )}
            </div>
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
            <div className="flex flex-col gap-1.5">
              <label
                htmlFor={field.name}
                className="text-[11.5px] font-medium text-foreground/85"
              >
                {translate('auth-login-password-label', {
                  defaultValue: 'Password',
                })}
              </label>
              <PasswordInput
                id={field.name}
                autoComplete="current-password"
                placeholder={translate('auth-login-password-placeholder', {
                  defaultValue: 'Enter your password',
                })}
                {...field}
                aria-invalid={fieldState.invalid || is401}
              />
              {fieldState.error && (
                <span className="text-[11.5px] text-destructive">
                  {fieldState.error.message}
                </span>
              )}
            </div>
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
          loading={isLoginPending}
          className="mt-1 w-full"
        >
          {translate('login-button-label', { defaultValue: 'Sign in' })}
        </Button>

        <div className="mt-1.5 text-center text-[12.5px] text-muted-foreground">
          {translate('auth-login-no-account', {
            defaultValue: 'New here?',
          })}{' '}
          <Link
            to={routePaths.register.path}
            className="font-medium text-primary hover:underline"
          >
            {translate('auth-login-create-account', {
              defaultValue: 'Create an account',
            })}
          </Link>
        </div>
      </div>
    </form>
  );
};
