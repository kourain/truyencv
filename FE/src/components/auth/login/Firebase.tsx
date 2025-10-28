"use client";

import { useEffect, useState } from "react";
import type { Route } from "next";
import { useRouter } from "next/navigation";
import { Facebook, LogIn } from "lucide-react";
import { FirebaseError } from "firebase/app";
import { AuthProvider, signInWithPopup, signOut } from "firebase/auth";

import { useToast } from "@components/providers/ToastProvider";
import { useAuth } from "@hooks/useAuth";
import {
  createFacebookProvider,
  createGoogleProvider,
  getFirebaseAuthInstance,
  isFirebaseConfigured
} from "@helpers/firebaseClient";
import { loginWithFirebase } from "@services/auth.service";
import { AuthStateFromJWT, decodeJwtToken } from "@helpers/authTokens";

const DEFAULT_FIREBASE_ERROR = "Không thể đăng nhập bằng Firebase. Vui lòng thử lại sau.";

type FirebaseProviderKey = "google" | "facebook";

type ProviderConfig = {
  label: string;
  icon: typeof LogIn;
  createProvider: () => AuthProvider;
};

const PROVIDER_CONFIG: Record<FirebaseProviderKey, ProviderConfig> = {
  google: {
    label: "Đăng nhập bằng Google",
    icon: LogIn,
    createProvider: () => createGoogleProvider()
  },
  facebook: {
    label: "Đăng nhập bằng Facebook",
    icon: Facebook,
    createProvider: () => createFacebookProvider()
  }
};

type FirebaseLoginButtonProps = {
  fallback: Route;
  successToast: { title: string; description: string };
  notConfiguredDescription: string;
  genericErrorMessage?: string;
  setError: (message: string | null) => void;
  onPendingChange?: (pending: boolean) => void;
  disabled?: boolean;
  provider?: FirebaseProviderKey;
};

const FirebaseLoginButton = ({
  fallback,
  successToast,
  notConfiguredDescription,
  genericErrorMessage,
  setError,
  onPendingChange,
  disabled = false,
  provider = "google"
}: FirebaseLoginButtonProps) => {
  const router = useRouter();
  const auth = useAuth();
  const { pushToast } = useToast();

  const config = PROVIDER_CONFIG[provider];
  const Icon = config.icon;
  const firebaseEnabled = isFirebaseConfigured();
  const [isPending, setIsPending] = useState(false);

  useEffect(() => {
    onPendingChange?.(isPending);
  }, [isPending, onPendingChange]);

  const handleFirebaseLogin = async () => {
    if (!firebaseEnabled) {
      pushToast({
        title: "Firebase chưa được cấu hình",
        description: notConfiguredDescription,
        variant: "error"
      });
      return;
    }

    setError(null);
    setIsPending(true);

    try {
      const authInstance = getFirebaseAuthInstance();
      const authProvider = config.createProvider();
      const credential = await signInWithPopup(authInstance, authProvider);
      const idToken = await credential.user.getIdToken();

      const response = await loginWithFirebase({
        id_token: idToken,
        display_name: credential.user.displayName ?? undefined,
        avatar_url: credential.user.photoURL ?? undefined,
        phone: credential.user.phoneNumber ?? undefined
      });

      pushToast({
        title: successToast.title,
        description: successToast.description,
        variant: "success"
      });

      const nextAuthState = AuthStateFromJWT(decodeJwtToken(response.access_token));
      auth.updateAuthState(nextAuthState);
      router.replace(fallback);
    } catch (error) {
      let message = genericErrorMessage ?? DEFAULT_FIREBASE_ERROR;
      if (error instanceof FirebaseError) {
        if (error.message.includes("auth/popup-closed-by-user")) {
          message = "Cửa sổ đăng nhập đã bị đóng. Vui lòng thử lại.";
        } else if (error.message.includes("auth/cancelled-popup-request")) {
          message = "Yêu cầu đăng nhập đã bị hủy. Vui lòng thử lại.";
        } else {
          message = error.message;
        }
      } else if (error instanceof Error) {
        message = error.message;
      }

      setError(message);
      pushToast({
        title: "Đăng nhập Firebase thất bại",
        description: message,
        variant: "error"
      });
    } finally {
      setIsPending(false);
      try {
        const authInstance = getFirebaseAuthInstance();
        await signOut(authInstance);
      } catch (signOutError) {
        console.error("[Firebase] Không thể đăng xuất Firebase tạm thời", signOutError);
      }
    }
  };

  return (
    <button
      type="button"
      onClick={handleFirebaseLogin}
      disabled={!firebaseEnabled || isPending || disabled}
      className="inline-flex w-full items-center justify-center gap-2 rounded-lg border border-surface-muted bg-surface px-6 py-3 text-sm font-semibold text-surface-foreground transition hover:border-primary hover:text-primary disabled:cursor-not-allowed disabled:opacity-60"
    >
      {isPending ? (
        <span className="h-4 w-4 animate-spin rounded-full border-2 border-primary border-t-transparent" />
      ) : (
        <Icon className="h-4 w-4" />
      )}
      {config.label}
    </button>
  );
};

export default FirebaseLoginButton;
