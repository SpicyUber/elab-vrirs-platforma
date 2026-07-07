import { useContext } from "react";
import { ServerContext } from "../context/ServerContext";

export const useServer = () => {
  const ctx = useContext(ServerContext);

  if (!ctx) {
    throw new Error("useServer must be used within a ServerProvider");
  }

  return ctx;
};