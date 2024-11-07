import globals from 'globals';
import pluginJs from '@eslint/js';
import tseslint from 'typescript-eslint';
import pluginReact from 'eslint-plugin-react';

export default [
  { files: ['**/*.{js,mjs,cjs,ts,jsx,tsx}'] },
  { languageOptions: { globals: globals.browser } },
  pluginJs.configs.recommended,
  ...tseslint.configs.recommended,
  pluginReact.configs.flat.recommended,
  {
    ignorePatterns: ['node_modules/'],
    settings: {
      'import/resolver': {
        node: {
          extensions: ['.js', '.jsx', '.ts', '.tsx'],
        },
        typescript: {
          alwaysTryTypes: true, // Always try to resolve @types packages
        },
      },
    },
    rules: {
      'react/jsx-uses-react': 'off',
      'react/react-in-jsx-scope': 'off',
      'import/no-restricted-paths': [
        'error',
        {
          zones: [
            {
              target: ['./src/features', './src/infrastructure'],
              from: ['./src/environments/environment.local.ts'],
              message:
                'Import the current environment file to get the options for the current environment',
            },
          ],
        },
      ],
    },
  },
];
