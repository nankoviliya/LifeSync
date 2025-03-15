import { Dropdown } from 'primereact/dropdown';
import { InputText } from 'primereact/inputtext';
import { Controller } from 'react-hook-form';

import { Button } from '@/components/buttons/Button';
import { useUserProfileEditable } from '@/features/userProfile/hooks/useUserProfileEditable';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useFrontendSettings } from '@/hooks/useFrontendSettings';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

import styles from './UserProfileDataEditable.module.scss';

export interface IUserProfileDataEditableProps {
  userData: IUserProfileDataModel;
  disableEditMode: () => void;
}

export const UserProfileDataEditable = ({
  userData,
  disableEditMode,
}: IUserProfileDataEditableProps) => {
  const { translate } = useAppTranslations();

  const { frontendSettings, isLoading } = useFrontendSettings();

  const { control, handleSubmit, onSubmit, isSubmitting } =
    useUserProfileEditable(userData, disableEditMode);

  const { userId, userName, email, balanceAmount, balanceCurrency, language } =
    userData;

  return (
    <form
      className={styles['user-profile-data']}
      onSubmit={handleSubmit(onSubmit)}
    >
      {isLoading && <div>Loading configuration...</div>}
      {frontendSettings && (
        <>
          <span>
            {translate('profile-user-id-label')}: {userId}
          </span>
          <span>
            {translate('profile-username-label')}: {userName}
          </span>
          <span>
            {translate('profile-email-label')}: {email}
          </span>
          <span>
            {translate('profile-first-name-label')}:{' '}
            <Controller
              control={control}
              name="firstName"
              render={({ field }) => <InputText {...field} />}
            />
          </span>
          <span>
            {translate('profile-last-name-label')}:{' '}
            <Controller
              control={control}
              name="lastName"
              render={({ field }) => <InputText {...field} />}
            />
          </span>
          <span>
            {translate('profile-balance-label')}: {balanceAmount}{' '}
            {balanceCurrency}
          </span>
          <span>
            {translate('profile-language-label')}: {language.name}
          </span>
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
            <Button
              type="submit"
              label="Save"
              loading={isSubmitting}
              icon="pi pi-check"
            />
            <Button
              label="Cancel"
              icon="pi pi-times"
              onClick={(e: React.MouseEvent<HTMLInputElement>) => {
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
