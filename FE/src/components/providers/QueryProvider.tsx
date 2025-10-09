"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactNode, useState } from "react";

interface QueryProviderProps {
  children: ReactNode;
}

const QueryProvider = ({ children }: QueryProviderProps) => {
  const [client] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            retry: 1,
            refetchOnWindowFocus: false,
            refetchOnReconnect: true,
            staleTime: 30_000
          }
        }
      })
  );

  return <QueryClientProvider client={client}>{children}</QueryClientProvider>;
};

export default QueryProvider;
