import { Check, X } from 'lucide-react';
import { Controller } from 'react-hook-form';

import { Button } from '@/components/buttons/Button';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { useUserProfileEditable } from '@/features/userProfile/hooks/useUserProfileEditable';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useFrontendSettings } from '@/hooks/useFrontendSettings';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

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
      className="inline-flex flex-col gap-4"
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
              render={({ field }) => <Input {...field} />}
            />
          </span>
          <span>
            {translate('profile-last-name-label')}:{' '}
            <Controller
              control={control}
              name="lastName"
              render={({ field }) => <Input {...field} />}
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
          <div className="mt-4 inline-flex flex-row gap-4">
            <Button
              type="submit"
              label="Save"
              loading={isSubmitting}
              icon={<Check className="h-4 w-4" />}
            />
            <Button
              label="Cancel"
              icon={<X className="h-4 w-4" />}
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
