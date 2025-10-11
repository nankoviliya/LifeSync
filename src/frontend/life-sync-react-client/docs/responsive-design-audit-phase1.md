# Responsive Design Audit - Phase 1

## Scope & Method
- Reviewed 22 SCSS module files under `src/` (fewer than the 28 called out in the issue; confirm whether additional modules exist elsewhere or if the count in the ticket is outdated).
- Inspected breakpoint mixins in `src/sharedStyles/abstracts/breakpoints/breakpoints.scss` (`Tablet` <=1024px, `Mobile` <=767px). Only `Home.module.scss` and `Finances.module.scss` currently consume these mixins.
- Focused on Login, Register, User Profile, Finances, and Transactions experiences, checking layout structure, viewport scaling, and likely touch target sizing.
- Static code review only. UI screenshots pending once we can run the app in a browser or Storybook environment.

## SCSS Module Inventory (initial)
- `components/errors/MainErrorFallback.module.scss` - Static layout; no responsive rules.
- `components/footer/components/Footer.module.scss` - Fixed font sizes and horizontal link spacing; no adjustments for narrow screens.
- `components/header/components/Header.module.scss` - Horizontal flex layout without wrapping; header height locked to `3em`.
- `components/header/components/HeaderUnauthenticatedButtons.module.scss` - Uses flex row with 1rem gap; modifier class mismatch (`__button--login` vs `__button__login` usage) prevents login button colors from applying.
- `components/layouts/BaseLayout.module.scss` - Centers content with `display: flex` but no max-width handling; header/footer stack fixed.
- `components/serviceCard/components/ServiceCard.module.scss` - `max-width: 350px` and `max-height: 300px`; no responsive overrides.
- `components/sidebar/SidebarContainer.module.scss` - Bare `inline-flex`.
- `features/home/Home.module.scss` - Uses CSS grid and breakpoint mixins; margins stay at 2em on all sizes.
- `features/login/components/Login.module.scss` - Column flex with `max-width: 400px`; form depends on parent centering, no min-width safeguards.
- `features/register/components/Register.module.scss` - Two-column inline-flex form; no wrap at narrow widths; box-shadow container adds extra width with padding.
- `features/userProfile/components/UserProfile.module.scss` - Row `inline-flex` with `space-evenly`; avatar locked at 100px.
- `features/userProfile/components/profileData/*` - Column layouts with inline-flex; action buttons in row with 1rem gap.
- `features/finances/Finances.module.scss` - Responsive grid using breakpoint mixins (3/2/1 columns).
- `features/finances/transactions/components/Transactions.module.scss` - `inline-flex` row keeping filters/content side-by-side; list stays grid 1fr.
- `features/finances/transactions/components/TransactionsFilters.module.scss` - Column flex, fixed `max-height: 400px` with `overflow-y: scroll`; control buttons in single row.
- `features/finances/transactions/components/TransactionsSummary.module.scss` - Column flex with row sections; relies on `inline-flex`.
- `features/finances/transactions/components/TransactionItem.module.scss` - `inline-flex` row with `justify-content: space-between`; padding only horizontal.
- `features/finances/transactions/components/NewTransactionButtons.module.scss` - Buttons laid out in row; no wrap.
- `features/finances/transactions/components/NewIncomeTransaction.module.scss` / `.../NewExpenseTransaction.module.scss` - Column flex forms.

## Feature-Level Responsive Risks
### Login
- `max-height: 400px` can clip validation errors or additional fields on smaller devices with the virtual keyboard visible.
- Form relies on parent layout for centering; no `width: 100%` or `margin: 0 auto`, so alignment may break if parent flex behavior changes.
- Touch targets (PrimeReact `Button`, `InputText`, `Password`) likely render around 36-38px tall; need verification against 44px target.

### Register
- `register-page__form` is fixed to a row layout; at <=768px the two column sections will overflow horizontally.
- Repeated `autoFocus` on multiple fields will break focus order on mobile browsers.
- Box shadow + 1rem horizontal padding pushes total width beyond viewport when combined with `max-width: 600px`.

### User Profile
- `user-profile__info` keeps avatar and data side-by-side with `space-evenly`; on screens <600px content will shrink excessively or overflow.
- Edit mode wraps form controls inside `span` elements; without responsive rules, labels and fields will wrap unpredictably.
- Action buttons remain in a single row; need wrap/stack for narrow widths.

### Finances Dashboard
- `Finances.module.scss` grid adapts, but child `ServiceCard` still maxes at 350px; cards may float left leaving unused space on small screens.
- Dialogs in `NewTransactionButtons` are set to `50vw`; acceptable on tablet/desktop but cramped on phones - should expand to about 90vw and adjust height.

### Transactions
- Root container uses `inline-flex` row, forcing filters sidebar and transaction list to share horizontal space; expect horizontal scroll on <=1024px devices.
- Filters panel has `overflow-y: scroll` with `max-height: 400px`; on mobile this creates nested scrolling and hidden filter options.
- Summary sections remain side-by-side; stacked layout needed under tablet breakpoint.
- Control buttons (filter apply/reset, new transaction buttons) lack `flex-wrap` and minimum sizing, so they collapse under ~360px width.
- `TransactionItem` uses horizontal padding only; content can touch edges on mobile and icons/amounts may wrap inconsistently.

## Touch Target & Accessibility Notes
- PrimeReact buttons/inputs need confirmation of min height >=44px; if default theme does not guarantee this, add explicit `min-height` or padding.
- Header logo acts as button but uses `height: 3em`; ensure focus outline is visible and tap area >=44x44px (currently 3em is about 48px but depends on root font size).
- Footer links have `margin: 0 15px` but no padding; effective touch area likely <44px tall - needs vertical padding.
- Dialog close icons (PrimeReact default) should be verified for accessibility and target size.

## Initial Checklist (to evolve)
- [ ] Confirm total SCSS module inventory (ticket expects 28; only 22 found).
- [ ] Document screenshots for Login/Register/Profile/Finances/Transactions across desktop (>=1280px), tablet (768-1024px), and mobile (<=480px).
- [ ] Validate touch target sizes for header buttons, auth CTAs, filters, and dialog actions (>=44px).
- [ ] Add breakpoint mixins to high-risk layouts (Register form, User Profile, Transactions, Footer, Header).
- [ ] Replace `inline-flex` with `flex` or grid where semantic; enable wrapping or column stacks under breakpoints.
- [ ] Audit `autoFocus` usage; restrict to first field per view to improve mobile UX.
- [ ] Capture issues list with severity/ticket references once verified visually.

## Next Steps
- Spin up the frontend locally to capture actual breakage and screenshots (blocked until runtime instructions provided).
- Prioritise fixing layout-breaking issues (Register form overflow, Transactions two-column layout) before smaller polish items.
- Coordinate with design/product for expected mobile behaviors (e.g., whether filters should collapse into accordion/drawer on phones).
