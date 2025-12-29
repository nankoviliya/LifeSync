# PrimeReact to shadcn/ui Migration Plan

## Overview

| Metric | Count |
|--------|-------|
| Files using PrimeReact | 21 |
| PrimeReact components | 12 types |
| SCSS modules to convert | 26 |
| Icons to replace | 11 |

**Approach:** Component-by-component migration with full Tailwind conversion
**Theming:** Migrate from `data-theme` attribute to shadcn class-based dark mode

---

## Phase 1: Foundation Setup

### Step 1.1: Install Dependencies

```bash
npm install tailwindcss @tailwindcss/vite
npm install class-variance-authority clsx tailwind-merge lucide-react @radix-ui/react-slot
npm install date-fns
```

### Step 1.2: Configure Vite for Tailwind

**File:** `vite.config.ts`

```typescript
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),  // Add this
    tsconfigPaths(),
    checker({ typescript: true }),
    basicSsl(),
  ],
  // ... rest unchanged
});
```

### Step 1.3: Create Tailwind Entry Point

**Create file:** `src/app.css`

```css
@import "tailwindcss";
```

### Step 1.4: Create Utility Function

**Create file:** `src/lib/utils.ts`

```typescript
import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
```

### Step 1.5: Run shadcn Init

```bash
npx shadcn@latest init
```

**Configuration choices:**
- Style: **New York**
- Base color: **Blue** (matches current `--primary-color: #3b82f6`)
- CSS variables: **Yes**

This creates:
- `components.json` - shadcn configuration
- `src/components/ui/` - component directory
- Updates CSS with shadcn color variables

---

## Phase 2: Theme Migration

### Step 2.1: Update ThemeProvider

**File:** `src/stores/ThemeProvider.tsx`

Change from `data-theme` attribute to class-based approach:

```typescript
// BEFORE:
document.documentElement.setAttribute('data-theme', effectiveTheme);

// AFTER:
document.documentElement.classList.remove('light', 'dark');
document.documentElement.classList.add(effectiveTheme);
```

### Step 2.2: Update theme.ts

**File:** `src/lib/theme.ts`

Update `initializeTheme()` function to use class-based approach instead of `data-theme` attribute.

### Step 2.3: Map CSS Variables

| Current Variable | shadcn Variable |
|------------------|-----------------|
| `--surface-ground` | `--background` |
| `--surface-card` | `--card` |
| `--surface-border` | `--border` |
| `--text-color` | `--foreground` |
| `--text-color-secondary` | `--muted-foreground` |
| `--primary-color` | `--primary` |
| `--primary-color-text` | `--primary-foreground` |

---

## Phase 3: Component Migration

### Migration Order (by dependency complexity)

| Order | Component | Files | shadcn Command |
|-------|-----------|-------|----------------|
| 1 | Button | 8 | `npx shadcn@latest add button` |
| 2 | Input | 6 | `npx shadcn@latest add input label` |
| 3 | Select | 3 | `npx shadcn@latest add select` |
| 4 | InputNumber | 3 | Use `<Input type="number">` |
| 5 | Password | 1 | Custom component |
| 6 | Calendar | 3 | `npx shadcn@latest add calendar popover` |
| 7 | Card | 1 | `npx shadcn@latest add card` |
| 8 | DropdownMenu | 4 | `npx shadcn@latest add dropdown-menu` |
| 9 | Dialog | 1 | `npx shadcn@latest add dialog` |
| 10 | Sheet | 1 | `npx shadcn@latest add sheet` |
| 11 | MultiSelect | 1 | `npx shadcn@latest add checkbox` + custom |
| 12 | Avatar | 1 | `npx shadcn@latest add avatar` |

### Step 3.1: Button Component

**Add component:**
```bash
npx shadcn@latest add button
```

**Files to modify:**
- `src/components/buttons/Button.tsx` - main wrapper
- `src/features/auth/login/components/Login.tsx`
- `src/features/auth/register/components/Register.tsx`
- `src/features/finances/transactions/components/TransactionsFilters.tsx`
- `src/features/finances/transactions/components/NewTransactionButtons.tsx`
- `src/features/userProfile/components/profileData/UserProfileDataEditable.tsx`
- `src/components/header/components/UserAvatar.tsx`
- `src/components/header/components/ThemeToggle.tsx`

**Button wrapper migration:**

```typescript
// BEFORE (src/components/buttons/Button.tsx):
import { Button as PrimeButton } from 'primereact/button';
export const Button = (props) => <PrimeButton {...props} />;

// AFTER:
import { Button as ShadcnButton } from '@/components/ui/button';
import { Loader2 } from 'lucide-react';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  label?: string;
  loading?: boolean;
  icon?: React.ReactNode;
  severity?: 'secondary' | 'success' | 'danger';
  outlined?: boolean;
  text?: boolean;
}

export const Button = ({
  label,
  loading,
  icon,
  severity,
  outlined,
  text,
  children,
  ...props
}: ButtonProps) => {
  const variant = text ? 'ghost'
    : outlined ? 'outline'
    : severity === 'secondary' ? 'secondary'
    : severity === 'danger' ? 'destructive'
    : 'default';

  return (
    <ShadcnButton variant={variant} disabled={loading || props.disabled} {...props}>
      {loading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
      {icon}
      {label || children}
    </ShadcnButton>
  );
};
```

**Icon mapping (PrimeIcons → Lucide):**

| PrimeIcon | Lucide Import |
|-----------|---------------|
| `pi pi-user` | `User` |
| `pi pi-sun` | `Sun` |
| `pi pi-moon` | `Moon` |
| `pi pi-desktop` | `Monitor` |
| `pi pi-check` | `Check` |
| `pi pi-times` | `X` |
| `pi pi-refresh` | `RefreshCw` |
| `pi pi-spinner` | `Loader2` |
| `pi pi-external-link` | `ExternalLink` |
| `pi pi-github` | `Github` |
| `pi pi-linkedin` | `Linkedin` |

### Step 3.2: Input Component

**Add component:**
```bash
npx shadcn@latest add input label
```

**Files to modify:**
- `src/features/auth/login/components/Login.tsx`
- `src/features/auth/register/components/Register.tsx`
- `src/features/finances/transactions/components/TransactionsFilters.tsx`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx`
- `src/features/finances/transactions/components/NewIncomeTransaction.tsx`
- `src/features/userProfile/components/profileData/UserProfileDataEditable.tsx`

**Migration pattern:**

```typescript
// BEFORE:
import { InputText } from 'primereact/inputtext';
import { classNames } from 'primereact/utils';

<InputText
  id={field.name}
  {...field}
  className={classNames({ 'p-invalid': fieldState.invalid })}
/>

// AFTER:
import { Input } from '@/components/ui/input';
import { cn } from '@/lib/utils';

<Input
  id={field.name}
  {...field}
  className={cn(fieldState.invalid && 'border-destructive')}
/>
```

### Step 3.3: Select Component

**Add component:**
```bash
npx shadcn@latest add select
```

**Files to modify:**
- `src/features/auth/register/components/Register.tsx`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx`
- `src/features/userProfile/components/profileData/UserProfileDataEditable.tsx`

**Migration pattern:**

```typescript
// BEFORE:
<Dropdown
  value={field.value}
  onChange={(e) => field.onChange(e.value)}
  options={options}
  optionLabel="name"
  optionValue="code"
  placeholder="Select..."
/>

// AFTER:
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

<Select value={field.value} onValueChange={field.onChange}>
  <SelectTrigger>
    <SelectValue placeholder="Select..." />
  </SelectTrigger>
  <SelectContent>
    {options.map(opt => (
      <SelectItem key={opt.code} value={opt.code}>
        {opt.name}
      </SelectItem>
    ))}
  </SelectContent>
</Select>
```

### Step 3.4: InputNumber

**Files to modify:**
- `src/features/auth/register/components/Register.tsx`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx`
- `src/features/finances/transactions/components/NewIncomeTransaction.tsx`

**Migration pattern:**

```typescript
// BEFORE:
<InputNumber
  value={field.value}
  onValueChange={(e) => field.onChange(e)}
/>

// AFTER:
<Input
  type="number"
  value={field.value ?? ''}
  onChange={(e) => field.onChange(e.target.valueAsNumber || null)}
/>
```

### Step 3.5: Password Component

**Create file:** `src/components/ui/password-input.tsx`

```typescript
import { forwardRef, useState } from 'react';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Eye, EyeOff } from 'lucide-react';
import { cn } from '@/lib/utils';

export const PasswordInput = forwardRef<
  HTMLInputElement,
  React.InputHTMLAttributes<HTMLInputElement>
>(({ className, ...props }, ref) => {
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className="relative">
      <Input
        type={showPassword ? 'text' : 'password'}
        className={cn('pr-10', className)}
        ref={ref}
        {...props}
      />
      <Button
        type="button"
        variant="ghost"
        size="sm"
        className="absolute right-0 top-0 h-full px-3 hover:bg-transparent"
        onClick={() => setShowPassword(!showPassword)}
      >
        {showPassword ? (
          <EyeOff className="h-4 w-4" />
        ) : (
          <Eye className="h-4 w-4" />
        )}
      </Button>
    </div>
  );
});

PasswordInput.displayName = 'PasswordInput';
```

**File to modify:** `src/features/auth/login/components/Login.tsx`

### Step 3.6: Calendar/DatePicker

**Add components:**
```bash
npx shadcn@latest add calendar popover
```

**Create file:** `src/components/ui/date-picker.tsx`

```typescript
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";

interface DatePickerProps {
  value?: Date | null;
  onChange?: (date: Date | undefined) => void;
  placeholder?: string;
  className?: string;
}

export function DatePicker({ value, onChange, placeholder = "Pick a date", className }: DatePickerProps) {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          className={cn(
            "w-full justify-start text-left font-normal",
            !value && "text-muted-foreground",
            className
          )}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {value ? format(value, "PPP") : placeholder}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={value ?? undefined}
          onSelect={onChange}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
}
```

**Files to modify:**
- `src/features/finances/transactions/components/TransactionsFilters.tsx`
- `src/features/finances/transactions/components/NewExpenseTransaction.tsx`
- `src/features/finances/transactions/components/NewIncomeTransaction.tsx`
- `src/utils/dateUtilities.ts` - remove `Nullable` import from PrimeReact

### Step 3.7: Card Component

**Add component:**
```bash
npx shadcn@latest add card
```

**File to modify:** `src/components/serviceCard/components/ServiceCard.tsx`

**Migration pattern:**

```typescript
// BEFORE:
import { Card } from 'primereact/card';
<Card title={title}><p>{description}</p></Card>

// AFTER:
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
<Card>
  <CardHeader>
    <CardTitle>{title}</CardTitle>
  </CardHeader>
  <CardContent>
    <p>{description}</p>
  </CardContent>
</Card>
```

### Step 3.8: DropdownMenu (Menu)

**Add component:**
```bash
npx shadcn@latest add dropdown-menu
```

**Files to modify:**
- `src/components/header/components/UserAvatar.tsx`
- `src/components/header/components/ThemeToggle.tsx`
- `src/components/header/hooks/useUserAvatar.ts`
- `src/components/header/hooks/useThemeToggle.ts`

**Migration pattern (UserAvatar.tsx):**

```typescript
// BEFORE:
import { Button } from 'primereact/button';
import { Menu } from 'primereact/menu';

<Button icon="pi pi-user" onClick={toggleAvatarMenu} rounded text />
<Menu model={avatarMenuItems} popup ref={avatarMenuRef} />

// AFTER:
import { Button } from '@/components/ui/button';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { User } from 'lucide-react';

<DropdownMenu>
  <DropdownMenuTrigger asChild>
    <Button variant="ghost" size="icon" className="rounded-full">
      <User className="h-4 w-4" />
    </Button>
  </DropdownMenuTrigger>
  <DropdownMenuContent align="end">
    <DropdownMenuItem onClick={navigateToProfile}>
      Profile
    </DropdownMenuItem>
    <DropdownMenuItem onClick={handleLogout}>
      Logout
    </DropdownMenuItem>
  </DropdownMenuContent>
</DropdownMenu>
```

**Note:** The hooks `useUserAvatar` and `useThemeToggle` can be simplified - remove ref management as shadcn handles state internally.

### Step 3.9: Dialog

**Add component:**
```bash
npx shadcn@latest add dialog
```

**File to modify:** `src/features/finances/transactions/components/NewTransactionButtons.tsx`

**Migration pattern:**

```typescript
// BEFORE:
<Dialog header="New expense" visible={isVisible} onHide={handleClose}>
  <FormContent />
</Dialog>

// AFTER:
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';

<Dialog open={isVisible} onOpenChange={(open) => !open && handleClose()}>
  <DialogContent>
    <DialogHeader>
      <DialogTitle>New expense</DialogTitle>
    </DialogHeader>
    <FormContent />
  </DialogContent>
</Dialog>
```

### Step 3.10: Sheet (Sidebar)

**Add component:**
```bash
npx shadcn@latest add sheet
```

**File to modify:** `src/components/sidebar/SidebarContainer.tsx`

**Migration pattern:**

```typescript
// BEFORE:
<Sidebar visible={isVisible} onHide={onClose} position="left">
  {children}
</Sidebar>

// AFTER:
import { Sheet, SheetContent } from '@/components/ui/sheet';

<Sheet open={isVisible} onOpenChange={(open) => !open && onClose()}>
  <SheetContent side="left">
    {children}
  </SheetContent>
</Sheet>
```

### Step 3.11: MultiSelect (Custom)

**Add component:**
```bash
npx shadcn@latest add checkbox
```

**Create file:** `src/components/ui/multi-select.tsx`

```typescript
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { ChevronsUpDown } from "lucide-react";
import { cn } from "@/lib/utils";

interface MultiSelectOption {
  label: string;
  value: string;
}

interface MultiSelectProps {
  options: MultiSelectOption[];
  value: string[];
  onChange: (value: string[]) => void;
  placeholder?: string;
  className?: string;
}

export function MultiSelect({
  options,
  value,
  onChange,
  placeholder = "Select...",
  className
}: MultiSelectProps) {
  const toggleOption = (optValue: string) => {
    onChange(
      value.includes(optValue)
        ? value.filter(v => v !== optValue)
        : [...value, optValue]
    );
  };

  const selectedLabels = options
    .filter(opt => value.includes(opt.value))
    .map(opt => opt.label);

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          className={cn("w-full justify-between", className)}
        >
          {selectedLabels.length > 0
            ? selectedLabels.length > 2
              ? `${selectedLabels.length} selected`
              : selectedLabels.join(", ")
            : placeholder}
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-full p-2">
        <div className="space-y-2">
          {options.map(opt => (
            <label
              key={opt.value}
              className="flex items-center space-x-2 cursor-pointer hover:bg-accent p-1 rounded"
            >
              <Checkbox
                checked={value.includes(opt.value)}
                onCheckedChange={() => toggleOption(opt.value)}
              />
              <span className="text-sm">{opt.label}</span>
            </label>
          ))}
        </div>
      </PopoverContent>
    </Popover>
  );
}
```

**File to modify:** `src/features/finances/transactions/components/TransactionsFilters.tsx`

### Step 3.12: Avatar (Image)

**Add component:**
```bash
npx shadcn@latest add avatar
```

**File to modify:** `src/features/userProfile/components/UserProfile.tsx`

**Migration pattern:**

```typescript
// BEFORE:
import { Image } from 'primereact/image';
<Image src={avatarUrl} alt="avatar" />

// AFTER:
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { User } from 'lucide-react';

<Avatar className="h-24 w-24">
  <AvatarImage src={avatarUrl} alt="avatar" />
  <AvatarFallback>
    <User className="h-12 w-12" />
  </AvatarFallback>
</Avatar>
```

---

## Phase 4: SCSS to Tailwind Conversion

### Files to Convert

| File | Location |
|------|----------|
| `Login.module.scss` | `src/features/auth/login/components/` |
| `Register.module.scss` | `src/features/auth/register/components/` |
| `Header.module.scss` | `src/components/header/components/` |
| `Footer.module.scss` | `src/components/footer/` |
| `BaseLayout.module.scss` | `src/components/layouts/` |
| `ServiceCard.module.scss` | `src/components/serviceCard/components/` |
| `SidebarContainer.module.scss` | `src/components/sidebar/` |
| `SkeletonLoader.module.scss` | `src/components/loaders/` |
| `MainErrorFallback.module.scss` | `src/components/errors/` |
| `UserProfile*.module.scss` | `src/features/userProfile/components/` |
| `Finances*.module.scss` | `src/features/finances/` |
| `Transactions*.module.scss` | `src/features/finances/transactions/components/` |

### Conversion Example

**Before (`Login.module.scss`):**
```scss
.login-page {
  display: flex;
  max-width: 400px;
  flex-direction: column;
  gap: 1em;
  padding: 0 1rem;
  background-color: var(--surface-card);
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}
```

**After (inline in `Login.tsx`):**
```typescript
<form
  className="flex flex-col gap-4 max-w-[400px] px-4 bg-card rounded-lg shadow-md"
  onSubmit={onSubmit}
>
```

### Tailwind Class Reference

| SCSS Property | Tailwind Class |
|---------------|----------------|
| `display: flex` | `flex` |
| `flex-direction: column` | `flex-col` |
| `justify-content: center` | `justify-center` |
| `align-items: center` | `items-center` |
| `gap: 1rem` | `gap-4` |
| `padding: 1rem` | `p-4` |
| `margin-top: 1rem` | `mt-4` |
| `max-width: 400px` | `max-w-[400px]` |
| `border-radius: 8px` | `rounded-lg` |
| `background-color: var(--surface-card)` | `bg-card` |
| `color: var(--text-color)` | `text-foreground` |
| `box-shadow: ...` | `shadow-md` |

---

## Phase 5: Cleanup

### Step 5.1: Update main.tsx

**Remove these imports:**
```typescript
// DELETE:
import 'primereact/resources/themes/lara-light-blue/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';
import './sharedStyles/primereact-overrides.scss';
import './index.scss';

// ADD:
import './globals.css';  // shadcn styles
```

### Step 5.2: Update Provider.tsx

**Remove PrimeReactProvider:**
```typescript
// DELETE:
import { PrimeReactProvider } from 'primereact/api';

// REMOVE wrapper:
<PrimeReactProvider>
  ...
</PrimeReactProvider>
```

### Step 5.3: Delete Obsolete Files

```
DELETE:
├── src/index.scss
├── src/sharedStyles/
│   ├── primereact-overrides.scss
│   └── abstracts/
│       ├── colors.scss
│       ├── variables.scss
│       └── breakpoints/
├── src/**/*.module.scss (all SCSS modules after conversion)
```

### Step 5.4: Uninstall Packages

```bash
npm uninstall primereact primeicons sass
```

### Step 5.5: Verify package.json

**Final dependencies should include:**
```json
{
  "dependencies": {
    "@radix-ui/react-slot": "^1.x",
    "class-variance-authority": "^0.x",
    "clsx": "^2.x",
    "date-fns": "^3.x",
    "lucide-react": "^0.x",
    "tailwind-merge": "^2.x"
  },
  "devDependencies": {
    "tailwindcss": "^4.x",
    "@tailwindcss/vite": "^4.x"
  }
}
```

---

## Quick Reference

### All shadcn Components to Add
```bash
npx shadcn@latest add button input label select calendar popover card dropdown-menu dialog sheet checkbox avatar
```

### Key Pattern Changes

| Pattern | PrimeReact | shadcn |
|---------|------------|--------|
| Utility class | `classNames()` | `cn()` |
| Invalid state | `p-invalid` | `border-destructive` |
| Dropdown value | `e.value` | `value` directly |
| Dialog visible | `visible` | `open` |
| Dialog close | `onHide` | `onOpenChange` |
| Menu items | `model` prop array | Declarative children |
| Icons | String `"pi pi-*"` | Lucide components |

---

## Testing Checklist

- [ ] Phase 1: Tailwind CSS loads correctly
- [ ] Phase 2: Theme switching works (light/dark)
- [ ] Phase 3.1: Button renders and functions
- [ ] Phase 3.2: Input fields work with react-hook-form
- [ ] Phase 3.3: Select dropdowns work
- [ ] Phase 3.4: Number inputs work
- [ ] Phase 3.5: Password toggle works
- [ ] Phase 3.6: Date picker opens and selects
- [ ] Phase 3.7: Cards display correctly
- [ ] Phase 3.8: Dropdown menus open/close
- [ ] Phase 3.9: Dialogs open/close
- [ ] Phase 3.10: Sidebar opens/closes
- [ ] Phase 3.11: MultiSelect works
- [ ] Phase 3.12: Avatar displays
- [ ] Phase 4: All pages render correctly
- [ ] Phase 5: No console errors, build succeeds
