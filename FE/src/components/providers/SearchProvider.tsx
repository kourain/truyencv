"use client";

import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";

type SearchContextValue = {
  keyword: string;
  setKeyword: (keyword: string) => void;
  page: number;
  setPage: (page: number) => void;
  clear: () => void;
};

const SearchContext = createContext<SearchContextValue | undefined>(undefined);

interface SearchProviderProps {
  children: ReactNode;
}

const SearchProvider = ({ children }: SearchProviderProps) => {
  const [keyword, setKeyword] = useState<string>("");
  const [page, setPage] = useState<number>(1);

  const clear = useCallback(() => {
    setKeyword("");
    setPage(1);
  }, []);

  const value = useMemo<SearchContextValue>(
    () => ({
      keyword,
      page,
      setKeyword,
      setPage,
      clear,
    }),
    [keyword, page, setKeyword, setPage, clear],
  );

  return (
    <SearchContext.Provider value={value}>
      {children}
    </SearchContext.Provider>
  );
};
export default SearchProvider;
export const useSearch = () => {
  const context = useContext(SearchContext);

  if (!context) {
    throw new Error("useSearch must be used within a SearchProvider");
  }

  return context;
};
