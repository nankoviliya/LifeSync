import { useLogin } from '@/features/login/hooks/useLogin';
import styles from './Login.module.scss';
import { Controller } from 'react-hook-form';
import { InputText } from 'primereact/inputtext';
import { classNames } from 'primereact/utils';
import { Password } from 'primereact/password';
import { Button } from 'primereact/button';
import { useAppTranslations } from '@/infrastructure/translations/hooks/useAppTranslations';
import { Link, useNavigate } from 'react-router-dom';
import { routePaths } from '@/infrastructure/routing/routePaths';

export const Login = () => {
  const { translate } = useAppTranslations();
  const { control, onSubmit, isLoginPending } = useLogin();

  return (
    <form className={styles['login-page']} onSubmit={onSubmit}>
      <div className={styles['login-page__label']}>
        <h2>Login</h2>
      </div>

      <Controller
        name="email"
        control={control}
        rules={{ required: 'Email is required.' }}
        render={({ field, fieldState }) => (
          <InputText
            id={field.name}
            placeholder="Enter a email"
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
          <Password
            id={field.name}
            placeholder="Enter a password"
            {...field}
            className={classNames({ 'p-invalid': fieldState.invalid })}
            toggleMask
          />
        )}
      />

      <Button
        label={translate('login-button-label')}
        type="submit"
        loadingIcon="pi pi-spinner"
        loading={isLoginPending}
      />

      <div className={styles['login-page__signup-container']}>
        <span>
          Don't have an account?{' '}
          <Link to={routePaths.register.path}>{routePaths.register.name}</Link>
        </span>
      </div>
    </form>
  );
};
