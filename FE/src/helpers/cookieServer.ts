"use server";
// ==================== SERVER-SIDE ====================
export async function getAll() {
  const { cookies } = await import('next/headers');
  const cookieStore = await cookies();
  const allCookies: { [key: string]: string } = {};

  cookieStore.getAll().forEach((cookie) => {
    allCookies[cookie.name] = cookie.value;
  });

  return allCookies;
};

export async function get(name: string) {
  const { cookies } = await import('next/headers');
  const cookieStore = await cookies();
  return cookieStore.get(name)?.value;
};

export async function has(name: string) {
  const { cookies } = await import('next/headers');
  const cookieStore = await cookies();
  return cookieStore.has(name);
};