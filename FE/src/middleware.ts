import { appEnv } from '@const/env'
import { NextURL } from 'next/dist/server/web/next-url';
import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

export function middleware(request: NextRequest) {
  const url = request.nextUrl.clone();
  const hostname = request.headers.get('host');
  switch (hostname) {
    case appEnv.FE_USER:
      if (/^\/user(\/|$)/.test(url.pathname)) {
        const user_sanitizedPathname = url.pathname.replace(/^\/user(\/)?/, '/') || '/';
        return NextResponse.redirect(new NextURL(user_sanitizedPathname, request.url));
      }
      const prefixedPathname = url.pathname === '/' ? '/user' : `/user${url.pathname}`;
      return NextResponse.rewrite(new NextURL(prefixedPathname, request.url))
    case appEnv.FE_ADMIN:
      if (/^\/admin(\/|$)/.test(url.pathname)) {
        const admin_sanitizedPathname = url.pathname.replace(/^\/admin(\/)?/, '/') || '/';
        return NextResponse.redirect(new NextURL(admin_sanitizedPathname, request.url));
      }
      const newAdminPathname = url.pathname.startsWith('/admin') ? url.pathname : `/admin${url.pathname}`;
      return NextResponse.rewrite(new NextURL(newAdminPathname, request.url))
    default:
      break;
  }
  return NextResponse.next()
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon).*)'],
}