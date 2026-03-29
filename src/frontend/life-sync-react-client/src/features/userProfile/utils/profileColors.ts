export interface IProfileColor {
  bg: string;
  text: string;
  bgLight: string;
  textLight: string;
}

export const PROFILE_COLORS: IProfileColor[] = [
  {
    bg: 'bg-red-500',
    text: 'text-red-100',
    bgLight: 'bg-red-100',
    textLight: 'text-red-700',
  },
  {
    bg: 'bg-blue-500',
    text: 'text-blue-100',
    bgLight: 'bg-blue-100',
    textLight: 'text-blue-700',
  },
  {
    bg: 'bg-green-500',
    text: 'text-green-100',
    bgLight: 'bg-green-100',
    textLight: 'text-green-700',
  },
  {
    bg: 'bg-yellow-400',
    text: 'text-yellow-100',
    bgLight: 'bg-yellow-100',
    textLight: 'text-yellow-700',
  },
  {
    bg: 'bg-purple-500',
    text: 'text-purple-100',
    bgLight: 'bg-purple-100',
    textLight: 'text-purple-700',
  },
  {
    bg: 'bg-pink-500',
    text: 'text-pink-100',
    bgLight: 'bg-pink-100',
    textLight: 'text-pink-700',
  },
  {
    bg: 'bg-indigo-500',
    text: 'text-indigo-100',
    bgLight: 'bg-indigo-100',
    textLight: 'text-indigo-700',
  },
];

export function getProfileColor(initials: string): IProfileColor {
  const hash = initials.charCodeAt(0) + initials.charCodeAt(1);
  return PROFILE_COLORS[hash % PROFILE_COLORS.length];
}
