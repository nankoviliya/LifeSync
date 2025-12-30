import { Check, ChevronsUpDown } from 'lucide-react';
import { forwardRef, useState } from 'react';

import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover';
import { cn } from '@/lib/utils';

export interface MultiSelectOption {
  label: string;
  value: string;
}

export interface MultiSelectProps {
  id?: string;
  name?: string;
  value?: string[];
  options: MultiSelectOption[];
  onChange?: (value: string[]) => void;
  onBlur?: () => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
}

export const MultiSelect = forwardRef<HTMLButtonElement, MultiSelectProps>(
  (
    {
      id,
      name,
      value = [],
      options,
      onChange,
      onBlur,
      placeholder = 'Select options...',
      className,
      disabled,
    },
    ref,
  ) => {
    const [open, setOpen] = useState(false);

    const handleToggle = (optionValue: string) => {
      const newValue = value.includes(optionValue)
        ? value.filter((v) => v !== optionValue)
        : [...value, optionValue];
      onChange?.(newValue);
    };

    const selectedLabels = options
      .filter((option) => value.includes(option.value))
      .map((option) => option.label);

    return (
      <Popover open={open} onOpenChange={setOpen}>
        <PopoverTrigger asChild>
          <Button
            ref={ref}
            id={id}
            variant="outline"
            role="combobox"
            aria-expanded={open}
            className={cn('w-full justify-between font-normal', className)}
            disabled={disabled}
            onBlur={onBlur}
          >
            <span className="truncate">
              {selectedLabels.length > 0
                ? selectedLabels.join(', ')
                : placeholder}
            </span>
            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-full min-w-[200px] p-2">
          <div className="flex flex-col gap-1">
            {options.map((option) => (
              <label
                key={option.value}
                className="flex cursor-pointer items-center gap-2 rounded px-2 py-1.5 hover:bg-accent"
              >
                <Checkbox
                  checked={value.includes(option.value)}
                  onCheckedChange={() => handleToggle(option.value)}
                />
                <span className="text-sm">{option.label}</span>
              </label>
            ))}
          </div>
        </PopoverContent>
      </Popover>
    );
  },
);
MultiSelect.displayName = 'MultiSelect';
