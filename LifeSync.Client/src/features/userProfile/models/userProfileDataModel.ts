export interface IUserProfileDataModel {
  userId: string;
  userName?: string;
  email?: string;
  firstName: string;
  lastName: string;
  language: ILanguage;
  balanceAmount: number;
  balanceCurrency: string;
}

export interface ILanguage {
  id: string;
  name: string;
  code: string;
}
