import { createContext } from 'react';
import type { UserSessionInfo } from '../types/UserSessionInfo';

type UserContextType = {
  user: UserSessionInfo | null;
  login: (user: UserSessionInfo) => void;
  logout: () => void;
};

export const UserContext = createContext<UserContextType | null>(null);