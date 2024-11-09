import { useLogin } from '@/features/login/hooks/useLogin';
import styles from './Login.module.scss';
import { Controller } from 'react-hook-form';
import { InputText } from 'primereact/inputtext';
import { classNames } from 'primereact/utils';
import { Password } from 'primereact/password';
import { Button } from 'primereact/button';

export const Login = () => {
  const { control, onSubmit } = useLogin();

  return (
    <form className={styles['login-page']} onSubmit={onSubmit}>
      <Controller
        name="email"
        control={control}
        rules={{ required: 'Email is required.' }}
        render={({ field, fieldState }) => (
          <InputText
            id={field.name}
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
            {...field}
            className={classNames({ 'p-invalid': fieldState.invalid })}
            toggleMask
          />
        )}
      />

      <Button label="Submit" type="submit" />
    </form>
  );
};
