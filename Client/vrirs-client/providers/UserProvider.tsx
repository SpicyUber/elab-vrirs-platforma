import { useState } from 'react';
import { UserContext } from '../context/UserContext';
import type { UserSessionInfo } from '../types/UserSessionInfo';

const STORAGE_KEY = 'user_session';

export function UserProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserSessionInfo | null>(() => {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored ? JSON.parse(stored) : null;
  });

  const login = (user: UserSessionInfo) => {
    setUser(user);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem(STORAGE_KEY);
  };

  return (
    <UserContext.Provider value={{ user, login, logout }}>
      {children}
    </UserContext.Provider>
  );
}