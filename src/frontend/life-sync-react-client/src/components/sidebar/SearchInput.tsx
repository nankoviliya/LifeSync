import { Search } from 'lucide-react';

import { useAppTranslations } from '@/hooks/useAppTranslations';

export const SearchInput = () => {
  const { translate } = useAppTranslations();
  return (
    <div className="flex items-center gap-2.5 rounded-md bg-side-accent-soft px-2.5 py-2 text-[12px] text-side-muted">
      <Search className="size-3.5" aria-hidden="true" />
      <span className="flex-1">
        {translate('sidebar-search-placeholder', { defaultValue: 'Search' })}
      </span>
      <span className="rounded border border-side-border px-1.5 py-px font-mono text-[10px] text-side-muted">
        ⌘K
      </span>
    </div>
  );
};
