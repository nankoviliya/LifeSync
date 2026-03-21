import { Controller } from 'react-hook-form';
import { Link } from 'react-router-dom';

import { Button } from '@/components/buttons/Button';
import { Input } from '@/components/ui/input';
import { PasswordInput } from '@/components/ui/password-input';
import { routePaths } from '@/config/routing/routePaths';
import { useLogin } from '@/hooks/auth/useLogin';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const Login = () => {
  const { translate } = useAppTranslations();
  const { control, onSubmit, isLoginPending } = useLogin();

  const noAccountMessage = "Don't have an account?";

  return (
    <form
      className="mt-4 flex max-h-[400px] max-w-[400px] flex-col justify-center gap-4 rounded-lg bg-card px-4 text-card-foreground shadow-md transition-colors"
      onSubmit={onSubmit}
    >
      <div className="inline-flex justify-center">
        <h2>Login</h2>
      </div>

      <Controller
        name="email"
        control={control}
        rules={{ required: 'Email is required.' }}
        render={({ field, fieldState }) => (
          <Input
            id={field.name}
            placeholder="Enter a email"
            {...field}
            autoFocus
            aria-invalid={fieldState.invalid}
          />
        )}
      />

      <Controller
        name="password"
        control={control}
        rules={{ required: 'Password is required.' }}
        render={({ field, fieldState }) => (
          <PasswordInput
            id={field.name}
            placeholder="Enter a password"
            {...field}
            aria-invalid={fieldState.invalid}
          />
        )}
      />

      <Button
        label={translate('login-button-label')}
        type="submit"
        loading={isLoginPending}
      />

      <div className="inline-flex justify-center">
        <span>
          {noAccountMessage + ' '}
          <Link
            to={routePaths.register.path}
            className="text-primary hover:underline"
          >
            {routePaths.register.name}
          </Link>
        </span>
      </div>
    </form>
  );
};
