import { Check, Pencil, X } from 'lucide-react';
import { Controller } from 'react-hook-form';

import { CopyableLabel } from '@/components/common/copyable-label';
import { Row } from '@/components/common/row';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { useUserProfileEditable } from '@/features/userProfile/components/profileData/hooks/useUserProfileEditable';
import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useFrontendSettings } from '@/hooks/useFrontendSettings';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';
import { getLanguageTranslation } from '@/utils/translationsUtilities';

interface IProps {
  userData: IUserProfileDataModel;
  isEditing: boolean;
  onEditingChange: (editing: boolean) => void;
}

export const UserProfileDataBody = ({
  userData,
  isEditing,
  onEditingChange,
}: IProps) => {
  const { translate } = useAppTranslations();
  const { frontendSettings, isLoading } = useFrontendSettings();
  const { control, handleSubmit, onSubmit, isSubmitting } =
    useUserProfileEditable(userData);

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

  const readonlyRows = [
    { label: translate('profile-user-id-label'), value: userId },
    { label: translate('profile-username-label'), value: userName },
    { label: translate('profile-email-label'), value: email },
    {
      label: translate('profile-balance-label'),
      value: `${balanceAmount} ${balanceCurrency}`,
    },
  ];

  return (
    <form
      className="flex flex-col gap-3 flex-1"
      onSubmit={handleSubmit(onSubmit)}
    >
      {readonlyRows.map(({ label, value }) => (
        <Row key={label} label={label}>
          <CopyableLabel value={value ?? ''} />
        </Row>
      ))}

      <Separator />

      <Row label={translate('profile-first-name-label')}>
        {isEditing ? (
          <Controller
            control={control}
            name="firstName"
            render={({ field }) => (
              <Input className="h-7 text-xs text-right w-32" {...field} />
            )}
          />
        ) : (
          <CopyableLabel value={firstName} />
        )}
      </Row>

      <Row label={translate('profile-last-name-label')}>
        {isEditing ? (
          <Controller
            control={control}
            name="lastName"
            render={({ field }) => (
              <Input className="h-7 text-xs text-right w-32" {...field} />
            )}
          />
        ) : (
          <CopyableLabel value={lastName} />
        )}
      </Row>

      <Row label={translate('profile-language-label')}>
        {isEditing && !isLoading && frontendSettings ? (
          <Controller
            name="languageId"
            control={control}
            render={({ field }) => (
              <Select value={field.value} onValueChange={field.onChange}>
                <SelectTrigger className="w-32" onBlur={field.onBlur}>
                  <SelectValue
                    placeholder={translate('profile-language-placeholder')}
                  />
                </SelectTrigger>
                <SelectContent>
                  {frontendSettings.languageOptions.map((opt) => (
                    <SelectItem key={opt.id} value={opt.id}>
                      {getLanguageTranslation(opt.code, translate)}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
        ) : (
          <CopyableLabel
            value={getLanguageTranslation(language.code, translate)}
          />
        )}
      </Row>

      <div className="flex justify-center gap-2 mt-auto">
        {isEditing ? (
          <>
            <Button type="submit" loading={isSubmitting}>
              {!isSubmitting && <Check className="h-4 w-4" />}
              {translate('profile-save-button')}
            </Button>
            <Button
              onClick={(e) => {
                onEditingChange(false);
                e.preventDefault();
              }}
            >
              <X className="h-4 w-4" />
              {translate('profile-cancel-button')}
            </Button>
          </>
        ) : (
          <Button
            type="button"
            onClick={(e) => {
              onEditingChange(true);
              e.preventDefault();
            }}
          >
            <Pencil className="h-4 w-4" />
            {translate('profile-edit-button')}
          </Button>
        )}
      </div>
    </form>
  );
};
