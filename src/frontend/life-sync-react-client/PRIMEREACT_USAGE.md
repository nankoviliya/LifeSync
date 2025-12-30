# PrimeReact & PrimeIcons Usage Reference

## PrimeReact Components

### Button
- `src/components/buttons/Button.tsx:1` - wrapper component
- `src/components/header/components/UserAvatar.tsx:1`
- `src/components/header/components/ThemeToggle.tsx:1`
- `src/features/auth/login/components/Login.tsx:7`
- `src/features/auth/register/components/Register.tsx:8`
- `src/features/userProfile/components/profileData/UserProfileDataEditable.tsx:5`
- `src/features/finances/transactions/components/TransactionsFilters.tsx:6`
- `src/features/finances/transactions/components/NewTransactionButtons.tsx:3`

### Menu
- `src/components/header/components/UserAvatar.tsx:2`
- `src/components/header/components/ThemeToggle.tsx:2`
- `src/components/header/hooks/useUserAvatar.ts:1`
- `src/components/header/hooks/useThemeToggle.ts:1`

### MenuItem (type)
- `src/components/header/hooks/useUserAvatar.ts:2`
- `src/components/header/hooks/useThemeToggle.ts:2`

### Card
- `src/components/serviceCard/components/ServiceCard.tsx:1`

### Sidebar
- `src/components/sidebar/SidebarContainer.tsx:1`

### Image
- `src/features/userProfile/components/UserProfile.tsx:1`

### InputText
- `src/features/auth/login/components/Login.tsx:1`
- `src/features/auth/register/components/Register.tsx:3`
- `src/features/userProfile/components/profileData/UserProfileDataEditable.tsx:2`
- `src/features/finances/transactions/components/TransactionsFilters.tsx:2`
- `src/features/finances/transactions/components/NewIncomeTransaction.tsx:3`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx:4`

### Password
- `src/features/auth/login/components/Login.tsx:2`

### Dropdown
- `src/features/auth/register/components/Register.tsx:1`
- `src/features/userProfile/components/profileData/UserProfileDataEditable.tsx:1`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx:2`

### InputNumber
- `src/features/auth/register/components/Register.tsx:2`
- `src/features/finances/transactions/components/NewIncomeTransaction.tsx:2`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx:3`

### Calendar
- `src/features/finances/transactions/components/TransactionsFilters.tsx:1`
- `src/features/finances/transactions/components/NewIncomeTransaction.tsx:1`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx:1`

### MultiSelect
- `src/features/finances/transactions/components/TransactionsFilters.tsx:3`

### Dialog
- `src/features/finances/transactions/components/NewTransactionButtons.tsx:1`

---

## PrimeIcons

### Theme Icons
- `pi-sun` - `src/components/header/hooks/useThemeToggle.ts:18,43`
- `pi-moon` - `src/components/header/hooks/useThemeToggle.ts:24,43`
- `pi-desktop` - `src/components/header/hooks/useThemeToggle.ts:30`

### User Icons
- `pi-user` - `src/components/header/components/UserAvatar.tsx:12`

### Action Icons
- `pi-check` - `src/features/userProfile/components/profileData/UserProfileDataEditable.tsx:92`, `src/features/finances/transactions/components/TransactionsFilters.tsx:139`
- `pi-times` - `src/features/userProfile/components/profileData/UserProfileDataEditable.tsx:96`
- `pi-refresh` - `src/features/finances/transactions/components/TransactionsFilters.tsx:147`

### Loading Icons
- `pi-spinner` - `src/features/auth/register/components/Register.tsx:148`, `src/features/auth/login/components/Login.tsx:59`

### Footer Social Icons
- `pi-external-link` - `src/components/footer/Footer.tsx:20`
- `pi-github` - `src/components/footer/Footer.tsx:29`
- `pi-linkedin` - `src/components/footer/Footer.tsx:38`

---

## PrimeReact Utilities

### classNames
- `src/components/sidebar/SidebarContainer.tsx:2`
- `src/components/header/components/HeaderUnauthenticatedButtons.tsx:1`
- `src/features/auth/login/components/Login.tsx:3`
- `src/features/auth/register/components/Register.tsx:4`
- `src/features/finances/transactions/components/TransactionItem.tsx:1`
- `src/features/finances/transactions/components/NewIncomeTransaction.tsx:4`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx:5`

### Nullable (type helper)
- `src/utils/dateUtilities.ts:1`

### PrimeReactProvider
- `src/app/Provider.tsx:2`

---

## CSS/Theme Imports

### Main Entry (main.tsx)
- `src/main.tsx:4` - `primereact/resources/themes/lara-light-blue/theme.css`
- `src/main.tsx:5` - `primereact/resources/primereact.min.css`
- `src/main.tsx:6` - `primeicons/primeicons.css`
- `src/main.tsx:8` - `./sharedStyles/primereact-overrides.scss`

---

## Summary

| Category | Count |
|----------|-------|
| Components | 12 |
| Icons | 11 |
| Utilities | 3 |
| Total Files | 21 |
