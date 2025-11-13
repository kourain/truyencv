import { appEnv } from '@const/env'
import { NextURL } from 'next/dist/server/web/next-url';
import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

export function middleware(request: NextRequest) {
  const url = request.nextUrl.clone();
  const requestHeaders = new Headers(request.headers);
  requestHeaders.set('x-url', request.url);
  const hostname = request.headers.get('host');
  if (appEnv.FE_USER.includes(hostname!)) {
    if (/^\/user(\/|$)/.test(url.pathname)) {
      const user_sanitizedPathname = url.pathname.replace(/^\/user(\/)?/, '/') || '/';
      return NextResponse.redirect(new NextURL(user_sanitizedPathname, request.url));
    }
    const prefixedPathname = url.pathname === '/' ? '/user' : `/user${url.pathname}`;
    return NextResponse.rewrite(new NextURL(prefixedPathname, request.url))
  }
  if (appEnv.FE_ADMIN.includes(hostname!)) {
    if (/^\/admin(\/|$)/.test(url.pathname)) {
      const admin_sanitizedPathname = url.pathname.replace(/^\/admin(\/)?/, '/') || '/';
      return NextResponse.redirect(new NextURL(admin_sanitizedPathname, request.url));
    }
    const newAdminPathname = url.pathname.startsWith('/admin') ? url.pathname : `/admin${url.pathname}`;
    return NextResponse.rewrite(new NextURL(newAdminPathname, request.url))
  }
  return NextResponse.next()
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon).*)'],
}