import { IUserProfileDataModel } from '@/features/userProfile/models/userProfileDataModel';
import styles from './UserProfileDataEditable.module.scss';
import { Controller } from 'react-hook-form';
import { InputText } from 'primereact/inputtext';
import { useUserProfileEditable } from '@/features/userProfile/hooks/useUserProfileEditable';
import { Button } from 'primereact/button';
import { useFrontendSettings } from '@/infrastructure/frontendSettings/hooks/useFrontendSettings';
import { Dropdown } from 'primereact/dropdown';

export interface IUserProfileDataEditableProps {
  userData: IUserProfileDataModel;
  disableEditMode: () => void;
}

export const UserProfileDataEditable = ({
  userData,
  disableEditMode,
}: IUserProfileDataEditableProps) => {
  const { frontendSettings, isLoading } = useFrontendSettings();

  const { control, handleSubmit, onSubmit } = useUserProfileEditable(
    userData,
    disableEditMode,
  );

  const {
    userId,
    userName,
    email,
    firstName,
    lastName,
    balanceAmount,
    balanceCurrency,
    language,
  } = userData;

  return (
    <form
      className={styles['user-profile-data']}
      onSubmit={handleSubmit(onSubmit)}
    >
      {isLoading && <div>Loading configuration...</div>}
      {frontendSettings && (
        <>
          <span>User ID: {userId}</span>
          <span>Username: {userName}</span>
          <span>Email: {email}</span>
          <span>
            First Name:{' '}
            <Controller
              control={control}
              name="firstName"
              render={({ field }) => <InputText {...field} />}
            />
          </span>
          <span>
            Last Name:{' '}
            <Controller
              control={control}
              name="lastName"
              render={({ field }) => <InputText {...field} />}
            />
          </span>
          <span>
            Balance: {balanceAmount} {balanceCurrency}
          </span>
          <span>Language: {language.name}</span>
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
          <br />
          <div className={styles['user-profile-data__navigation-buttons']}>
            <Button type="submit" label="Save" icon="pi pi-check" />
            <Button
              label="Cancel"
              icon="pi pi-times"
              onClick={(e) => {
                e.stopPropagation();
                disableEditMode();
              }}
            />
          </div>
        </>
      )}
    </form>
  );
};
