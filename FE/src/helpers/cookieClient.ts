'use client';

import Cookies from 'js-cookie';

// ==================== CLIENT-SIDE ====================

export const clientCookies = {
  // Get single cookie
  get(name: string): string | undefined {
    return Cookies.get(name);
  },

  // Get all cookies
  getAll(): { [key: string]: string } {
    return Cookies.get();
  },

  // Set cookie
  set(
    name: string,
    value: string,
    options?: {
      expires?: number | Date;
      path?: string;
      domain?: string;
      secure?: boolean;
      sameSite?: 'strict' | 'lax' | 'none';
    }
  ): void {
    Cookies.set(name, value, {
      expires: options?.expires || 7, // Default 7 days
      path: options?.path || '/',
      secure: options?.secure ?? process.env.NODE_ENV === 'production',
      sameSite: options?.sameSite || 'strict',
      ...options,
    });
  },

  // Remove cookie
  remove(name: string, options?: { path?: string; domain?: string }): void {
    Cookies.remove(name, options);
  },

  // Remove all cookies
  removeAll(): void {
    const allCookies = Cookies.get();
    Object.keys(allCookies).forEach((cookieName) => {
      Cookies.remove(cookieName);
    });
  },

  // Check if cookie exists
  has(name: string): boolean {
    return Cookies.get(name) !== undefined;
  },

  // Get cookies as array
  toArray(): Array<{ name: string; value: string }> {
    const cookies = Cookies.get();
    return Object.entries(cookies).map(([name, value]) => ({ name, value }));
  },

  // Get cookie names
  getNames(): string[] {
    return Object.keys(Cookies.get());
  },

  // Get cookies count
  count(): number {
    return Object.keys(Cookies.get()).length;
  },

  // Clear cookies by prefix
  clearByPrefix(prefix: string): void {
    const allCookies = Cookies.get();
    Object.keys(allCookies).forEach((name) => {
      if (name.startsWith(prefix)) {
        Cookies.remove(name);
      }
    });
  },

  // Get cookies by prefix
  getByPrefix(prefix: string): { [key: string]: string } {
    const allCookies = Cookies.get();
    return Object.keys(allCookies)
      .filter((name) => name.startsWith(prefix))
      .reduce((acc, name) => {
        acc[name] = allCookies[name];
        return acc;
      }, {} as { [key: string]: string });
  },
};
