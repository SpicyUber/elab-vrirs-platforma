import { createContext } from "react";

export type ServerContextType = {
  baseUrl: string;
  loading: boolean;
  error: string | null;
};

export const ServerContext = createContext<ServerContextType | undefined>(
  undefined
);