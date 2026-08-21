import { AppShellHeader } from '@/components/layouts/AppShellHeader';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Switch } from '@/components/ui/switch';
import { AppearanceThemeControl } from '@/features/settings/components/AppearanceThemeControl';
import { SettingsRow } from '@/features/settings/components/SettingsRow';
import { SettingsSection } from '@/features/settings/components/SettingsSection';
import { SettingsSubNav } from '@/features/settings/components/SettingsSubNav';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const Settings = () => {
  const { translate } = useAppTranslations();

  const navItems = [
    {
      id: 'general',
      label: translate('settings-nav-general', { defaultValue: 'General' }),
    },
    {
      id: 'appearance',
      label: translate('settings-nav-appearance', {
        defaultValue: 'Appearance',
      }),
    },
    {
      id: 'notifications',
      label: translate('settings-nav-notifications', {
        defaultValue: 'Notifications',
      }),
    },
    {
      id: 'security',
      label: translate('settings-nav-security', { defaultValue: 'Security' }),
    },
    {
      id: 'data',
      label: translate('settings-nav-data', { defaultValue: 'Data & privacy' }),
    },
    {
      id: 'danger',
      label: translate('settings-nav-danger', { defaultValue: 'Danger zone' }),
    },
  ];

  return (
    <>
      <AppShellHeader
        title={translate('page-settings-title', { defaultValue: 'Settings' })}
        subtitle={translate('page-settings-subtitle', {
          defaultValue: 'Preferences',
        })}
        actions={
          <Button type="button" variant="secondary" disabled>
            {translate('settings-save', { defaultValue: 'Save changes' })}
          </Button>
        }
      />

      <div className="grid grid-cols-1 gap-6 md:grid-cols-[220px_1fr]">
        <SettingsSubNav items={navItems} activeId="appearance" />

        <div className="flex flex-col gap-[18px]">
          {/* Appearance — the only functional section */}
          <SettingsSection
            id="appearance"
            title={translate('settings-appearance-title', {
              defaultValue: 'Appearance',
            })}
            description={translate('settings-appearance-desc', {
              defaultValue: 'How LifeSync looks on this device.',
            })}
          >
            <SettingsRow
              first
              title={translate('settings-theme-title', {
                defaultValue: 'Theme',
              })}
              description={translate('settings-theme-desc', {
                defaultValue: 'Match your preference or the system.',
              })}
              control={<AppearanceThemeControl />}
            />
            <SettingsRow
              title={translate('settings-reduce-motion-title', {
                defaultValue: 'Reduce motion',
              })}
              description={translate('settings-reduce-motion-desc', {
                defaultValue: 'Turn off non-essential animations.',
              })}
              control={
                <Switch
                  disabled
                  aria-label={translate('settings-reduce-motion-aria', {
                    defaultValue: 'Reduce motion (coming soon)',
                  })}
                />
              }
            />
            <SettingsRow
              title={translate('settings-compact-title', {
                defaultValue: 'Compact mode',
              })}
              description={translate('settings-compact-desc', {
                defaultValue: 'Tighter density across the app.',
              })}
              control={
                <Switch
                  disabled
                  aria-label={translate('settings-compact-mode-aria', {
                    defaultValue: 'Compact mode (coming soon)',
                  })}
                />
              }
            />
          </SettingsSection>

          {/* Notifications — visual only */}
          <SettingsSection
            id="notifications"
            title={translate('settings-notifications-title', {
              defaultValue: 'Notifications',
            })}
            description={translate('settings-notifications-desc', {
              defaultValue: 'Pick what lands in your inbox.',
            })}
          >
            <SettingsRow
              first
              title={translate('settings-digest-title', {
                defaultValue: 'Weekly digest',
              })}
              description={translate('settings-digest-desc', {
                defaultValue: 'Every Sunday evening, your week in review.',
              })}
              control={
                <Switch
                  disabled
                  aria-label={translate('settings-weekly-digest-aria', {
                    defaultValue: 'Weekly digest (coming soon)',
                  })}
                />
              }
            />
            <SettingsRow
              title={translate('settings-budget-alerts-title', {
                defaultValue: 'Budget alerts',
              })}
              description={translate('settings-budget-alerts-desc', {
                defaultValue: "Ping when you're close to a limit.",
              })}
              control={
                <Switch
                  disabled
                  aria-label={translate('settings-budget-alerts-aria', {
                    defaultValue: 'Budget alerts (coming soon)',
                  })}
                />
              }
            />
            <SettingsRow
              title={translate('settings-product-updates-title', {
                defaultValue: 'Product updates',
              })}
              description={translate('settings-product-updates-desc', {
                defaultValue: 'Occasional emails when we ship something new.',
              })}
              control={
                <Switch
                  disabled
                  aria-label={translate('settings-product-updates-aria', {
                    defaultValue: 'Product updates (coming soon)',
                  })}
                />
              }
            />
          </SettingsSection>

          {/* Security — visual only */}
          <SettingsSection
            id="security"
            title={translate('settings-security-title', {
              defaultValue: 'Security',
            })}
            description={translate('settings-security-desc', {
              defaultValue: 'Keep your account safe.',
            })}
          >
            <SettingsRow
              first
              title={translate('settings-2fa-title', {
                defaultValue: 'Two-factor authentication',
              })}
              description={translate('settings-2fa-desc', {
                defaultValue: 'Authenticator app. Strongly recommended.',
              })}
              control={
                <Badge variant="secondary">
                  {translate('settings-2fa-disabled', {
                    defaultValue: 'Not set up',
                  })}
                </Badge>
              }
            />
            <SettingsRow
              title={translate('settings-password-title', {
                defaultValue: 'Change password',
              })}
              description={translate('settings-password-desc', {
                defaultValue: 'Update your password regularly.',
              })}
              control={
                <Button type="button" variant="outline" size="sm" disabled>
                  {translate('settings-password-button', {
                    defaultValue: 'Change',
                  })}
                </Button>
              }
            />
            <SettingsRow
              title={translate('settings-sessions-title', {
                defaultValue: 'Active sessions',
              })}
              description={translate('settings-sessions-desc', {
                defaultValue: 'Devices currently signed in.',
              })}
              control={
                <Button type="button" variant="ghost" size="sm" disabled>
                  {translate('settings-sessions-button', {
                    defaultValue: 'Manage →',
                  })}
                </Button>
              }
            />
          </SettingsSection>

          {/* Data & privacy — visual only (real export/import lives on Profile) */}
          <SettingsSection
            id="data"
            title={translate('settings-data-title', {
              defaultValue: 'Data & privacy',
            })}
            description={translate('settings-data-desc', {
              defaultValue:
                'Export or import your account data from your profile.',
            })}
          >
            <SettingsRow
              first
              title={translate('settings-data-export-title', {
                defaultValue: 'Export & import',
              })}
              description={translate('settings-data-export-desc', {
                defaultValue: 'Manage your data on the Profile page.',
              })}
              control={
                <Button type="button" variant="outline" size="sm" disabled>
                  {translate('settings-data-export-button', {
                    defaultValue: 'On Profile',
                  })}
                </Button>
              }
            />
          </SettingsSection>

          {/* Danger zone — visual only */}
          <SettingsSection
            id="danger"
            danger
            title={translate('settings-danger-title', {
              defaultValue: 'Danger zone',
            })}
            description={translate('settings-danger-desc', {
              defaultValue: "These actions can't be undone.",
            })}
          >
            <SettingsRow
              first
              title={translate('settings-wipe-title', {
                defaultValue: 'Delete all transactions',
              })}
              description={translate('settings-wipe-desc', {
                defaultValue: 'Keep your account, wipe the ledger.',
              })}
              control={
                <Button type="button" variant="outline" size="sm" disabled>
                  {translate('settings-wipe-button', { defaultValue: 'Wipe' })}
                </Button>
              }
            />
            <SettingsRow
              title={translate('settings-delete-account-title', {
                defaultValue: 'Delete account',
              })}
              description={translate('settings-delete-account-desc', {
                defaultValue: 'Permanent. This cannot be undone.',
              })}
              control={
                <Button type="button" variant="destructive" size="sm" disabled>
                  {translate('settings-delete-account-button', {
                    defaultValue: 'Delete',
                  })}
                </Button>
              }
            />
          </SettingsSection>
        </div>
      </div>
    </>
  );
};
