import { useEffect, useState } from "react";
import type { ReactNode } from "react";
import { ServerContext } from "../context/ServerContext";
import type { ServerContextType } from "../types/ServerContextType";

type ServerConfig = {
  baseUrl: string;
};

export const ServerProvider = ({ children }: { children: ReactNode }) => {
  const [baseUrl, setBaseUrl] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadConfig = async () => {
      try {
        const res = await fetch("../config/appConfig.json");

        if (!res.ok) {
          throw new Error("Failed to load config");
        }

        const data: ServerConfig = await res.json();
        setBaseUrl(data.baseUrl);
      } catch (e) {
        setError(e instanceof Error ? e.message : "Unknown error");
      } finally {
        setLoading(false);
      }
    };

    loadConfig();
  }, []);

  const value: ServerContextType = {
    baseUrl,
    loading,
    error,
  };

  return (
    <ServerContext.Provider value={value}>
      {children}
    </ServerContext.Provider>
  );
};