import { appEnv } from '@const/env'
import { emptyServerAuthState } from '@const/val';
import { ACCESS_TOKEN_COOKIE, REFRESH_TOKEN_COOKIE } from '@helpers/authTokens';
import { getServerAuthState } from '@server/auth';
import { NextURL } from 'next/dist/server/web/next-url';
import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

export async function middleware(request: NextRequest) {
  const url = request.nextUrl.clone();
  const requestHeaders = new Headers(request.headers);
  requestHeaders.set('x-url', request.url);
  const header_url = request.url || "";
  const hostname = request.headers.get('host');
  let authState = emptyServerAuthState as { userProfile: UserProfileResponse; auth: AuthTokensResponse };
  let response: NextResponse;

  if (!header_url.includes("/auth/") && !header_url.includes("/privacy-policy") && !header_url.includes("/terms-of-service")) {
    authState = await getServerAuthState();
    if (authState.userProfile.id === "-1") {
      if (url.pathname.startsWith("/admin")) {
        return NextResponse.redirect(new URL("/admin/auth/login", request.url));
      }
      if (url.pathname.startsWith("/user")) {
        return NextResponse.redirect(new URL("/user/auth/login", request.url));
      }
    }
  }

  const authStateBase64 = Buffer.from(JSON.stringify(authState)).toString('base64');
  requestHeaders.set("x-auth-state", authStateBase64);
  if (appEnv.FE_USER.includes(hostname!)) {
    if (/^\/user(\/|$)/.test(url.pathname)) {
      const user_sanitizedPathname = url.pathname.replace(/^\/user(\/)?/, '/') || '/';
      response = NextResponse.redirect(new NextURL(user_sanitizedPathname, request.url));
    } else {
      const prefixedPathname = url.pathname === '/' ? '/user' : `/user${url.pathname}`;
      response = NextResponse.rewrite(new NextURL(prefixedPathname, request.url), { request: { headers: requestHeaders } });
    }
  } else if (appEnv.FE_ADMIN.includes(hostname!)) {
    if (/^\/admin(\/|$)/.test(url.pathname)) {
      const admin_sanitizedPathname = url.pathname.replace(/^\/admin(\/)?/, '/') || '/';
      response = NextResponse.redirect(new NextURL(admin_sanitizedPathname, request.url));
    } else {
      const newAdminPathname = url.pathname.startsWith('/admin') ? url.pathname : `/admin${url.pathname}`;
      response = NextResponse.rewrite(new NextURL(newAdminPathname, request.url), { request: { headers: requestHeaders } });
    }
  } else {
    response = NextResponse.next({ request: { headers: requestHeaders } });
  }

  if (authState.auth?.access_token) {
    const accessTokenExpiry = new Date(Date.now() + (authState.auth.access_token_minutes ?? 5) * 60 * 1000);
    response.cookies.set(ACCESS_TOKEN_COOKIE, authState.auth.access_token, {
      secure: process.env.NODE_ENV === 'production',
      sameSite: 'strict',
      path: '/',
      expires: accessTokenExpiry
    });
  }

  if (authState.auth?.refresh_token) {
    const refreshTokenExpiry = new Date(Date.now() + (authState.auth.refresh_token_days ?? 30) * 24 * 60 * 60 * 1000);
    response.cookies.set(REFRESH_TOKEN_COOKIE, authState.auth.refresh_token, {
      secure: process.env.NODE_ENV === 'production',
      sameSite: 'strict',
      path: '/',
      expires: refreshTokenExpiry
    });
  }

  return response;
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon).*)'],
}