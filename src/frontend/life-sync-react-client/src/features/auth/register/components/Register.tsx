import { Dropdown } from 'primereact/dropdown';
import { InputNumber } from 'primereact/inputnumber';
import { InputText } from 'primereact/inputtext';
import { classNames } from 'primereact/utils';
import { Controller } from 'react-hook-form';
import { Link } from 'react-router-dom';

import { Button } from '@/components/buttons/Button';
import { routePaths } from '@/config/routing/routePaths';
import { useRegistration } from '@/features/auth/register/hooks/useRegistration';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useFrontendSettings } from '@/hooks/useFrontendSettings';

import styles from './Register.module.scss';

export const Register = () => {
  const { translate } = useAppTranslations();
  const { control, onSubmit, isSubmitting } = useRegistration();
  const { frontendSettings, isLoading } = useFrontendSettings();

  return (
    <form className={styles['register-page']} onSubmit={onSubmit}>
      {isLoading && <div>Loading configuration...</div>}
      {!isLoading && frontendSettings && (
        <>
          <div className={styles['register-page__label']}>
            <h2>Register</h2>
          </div>

          <div className={styles['register-page__form']}>
            <div className={styles['register-page__form__section']}>
              <Controller
                name="firstName"
                control={control}
                rules={{ required: 'First Name is required.' }}
                render={({ field, fieldState }) => (
                  <InputText
                    id={field.name}
                    placeholder={'Enter a First Name'}
                    {...field}
                    autoFocus
                    className={classNames({ 'p-invalid': fieldState.invalid })}
                  />
                )}
              />

              <Controller
                name="lastName"
                control={control}
                rules={{ required: 'Last Name is required.' }}
                render={({ field, fieldState }) => (
                  <InputText
                    id={field.name}
                    placeholder={'Enter a Last Name'}
                    {...field}
                    autoFocus
                    className={classNames({ 'p-invalid': fieldState.invalid })}
                  />
                )}
              />

              <Controller
                name="email"
                control={control}
                rules={{ required: 'Email is required.' }}
                render={({ field, fieldState }) => (
                  <InputText
                    id={field.name}
                    placeholder={'Enter a Email'}
                    type="email"
                    {...field}
                    autoFocus
                    className={classNames({ 'p-invalid': fieldState.invalid })}
                  />
                )}
              />

              <Controller
                name="password"
                control={control}
                rules={{ required: 'Password is required.' }}
                render={({ field, fieldState }) => (
                  <InputText
                    id={field.name}
                    placeholder={'Enter a Password'}
                    type="password"
                    {...field}
                    autoFocus
                    className={classNames({ 'p-invalid': fieldState.invalid })}
                  />
                )}
              />
            </div>

            <div className={styles['register-page__form__section']}>
              <Controller
                name={'balance'}
                control={control}
                rules={{ required: 'Balance is required.' }}
                render={({ field, fieldState }) => (
                  <InputNumber
                    id={field.name}
                    placeholder={'Enter initial balance'}
                    ref={field.ref}
                    value={field.value}
                    onBlur={field.onBlur}
                    onValueChange={(e) => field.onChange(e)}
                    className={classNames({ 'p-invalid': fieldState.invalid })}
                  />
                )}
              />

              <Controller
                name="currency"
                control={control}
                render={({ field }) => (
                  <Dropdown
                    value={field.value}
                    onChange={(e) => field.onChange(e.value)}
                    options={frontendSettings.currencyOptions}
                    optionLabel="name"
                    optionValue="code"
                    placeholder="Select a currency"
                  />
                )}
              />

              <Controller
                name="languageId"
                control={control}
                render={({ field }) => (
                  <Dropdown
                    value={field.value}
                    onChange={(e) => field.onChange(e.value)}
                    options={frontendSettings.languageOptions}
                    optionLabel="name"
                    optionValue="id"
                    placeholder="Select a language"
                  />
                )}
              />
            </div>
          </div>

          <Button
            label={translate('register-button-label')}
            type="submit"
            loadingIcon="pi pi-spinner"
            loading={isSubmitting}
          />

          <div className={styles['register-page__login-container']}>
            <span>
              Already have an account?{' '}
              <Link to={routePaths.login.path}>{routePaths.login.name}</Link>
            </span>
          </div>
        </>
      )}
    </form>
  );
};
