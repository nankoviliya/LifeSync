import { Controller } from 'react-hook-form';
import { Link } from 'react-router-dom';

import { Button } from '@/components/buttons/Button';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { routePaths } from '@/config/routing/routePaths';
import { useRegistration } from '@/features/auth/register/hooks/useRegistration';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useFrontendSettings } from '@/hooks/useFrontendSettings';

export const Register = () => {
  const { translate } = useAppTranslations();
  const { control, onSubmit, isSubmitting } = useRegistration();
  const { frontendSettings, isLoading } = useFrontendSettings();

  return (
    <form
      className="mt-4 flex max-w-[600px] flex-col justify-center gap-4 rounded-lg bg-card px-4 pb-4 text-card-foreground shadow-md transition-colors"
      onSubmit={onSubmit}
    >
      {isLoading && <div>Loading configuration...</div>}
      {!isLoading && frontendSettings && (
        <>
          <div className="inline-flex justify-center">
            <h2>Register</h2>
          </div>

          <div className="inline-flex flex-row gap-4">
            <div className="inline-flex flex-col gap-4">
              <Controller
                name="firstName"
                control={control}
                rules={{ required: 'First Name is required.' }}
                render={({ field, fieldState }) => (
                  <Input
                    id={field.name}
                    placeholder={'Enter a First Name'}
                    {...field}
                    autoFocus
                    aria-invalid={fieldState.invalid}
                  />
                )}
              />

              <Controller
                name="lastName"
                control={control}
                rules={{ required: 'Last Name is required.' }}
                render={({ field, fieldState }) => (
                  <Input
                    id={field.name}
                    placeholder={'Enter a Last Name'}
                    {...field}
                    aria-invalid={fieldState.invalid}
                  />
                )}
              />

              <Controller
                name="email"
                control={control}
                rules={{ required: 'Email is required.' }}
                render={({ field, fieldState }) => (
                  <Input
                    id={field.name}
                    placeholder={'Enter a Email'}
                    type="email"
                    {...field}
                    aria-invalid={fieldState.invalid}
                  />
                )}
              />

              <Controller
                name="password"
                control={control}
                rules={{ required: 'Password is required.' }}
                render={({ field, fieldState }) => (
                  <Input
                    id={field.name}
                    placeholder={'Enter a Password'}
                    type="password"
                    {...field}
                    aria-invalid={fieldState.invalid}
                  />
                )}
              />
            </div>

            <div className="inline-flex flex-col gap-4">
              <Controller
                name={'balance'}
                control={control}
                rules={{ required: 'Balance is required.' }}
                render={({ field, fieldState }) => (
                  <Input
                    id={field.name}
                    type="number"
                    placeholder={'Enter initial balance'}
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
                )}
              />

              <Controller
                name="currency"
                control={control}
                render={({ field }) => (
                  <Select value={field.value} onValueChange={field.onChange}>
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Select a currency" />
                    </SelectTrigger>
                    <SelectContent>
                      {frontendSettings.currencyOptions.map((opt) => (
                        <SelectItem key={opt.code} value={opt.code}>
                          {opt.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />

              <Controller
                name="languageId"
                control={control}
                render={({ field }) => (
                  <Select
                    value={field.value?.toString()}
                    onValueChange={(val) => field.onChange(Number(val))}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Select a language" />
                    </SelectTrigger>
                    <SelectContent>
                      {frontendSettings.languageOptions.map((opt) => (
                        <SelectItem key={opt.id} value={opt.id.toString()}>
                          {opt.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
            </div>
          </div>

          <Button
            label={translate('register-button-label')}
            type="submit"
            loading={isSubmitting}
          />

          <div className="inline-flex justify-center">
            <span>
              Already have an account?{' '}
              <Link
                to={routePaths.login.path}
                className="text-primary hover:underline"
              >
                {routePaths.login.name}
              </Link>
            </span>
          </div>
        </>
      )}
    </form>
  );
};
