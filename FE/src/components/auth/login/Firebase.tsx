"use client";

import { useEffect, useState } from "react";
import type { Route } from "next";
import { useRouter } from "next/navigation";
import { LogIn } from "lucide-react";
import { FirebaseError } from "firebase/app";
import { signInWithPopup, signOut } from "firebase/auth";

import { useToast } from "@components/providers/ToastProvider";
import { useAuth } from "@hooks/useAuth";
import { createGoogleProvider, getFirebaseAuthInstance, isFirebaseConfigured } from "@helpers/firebaseClient";
import { loginWithFirebase } from "@services/auth.service";

const DEFAULT_FIREBASE_ERROR = "Không thể đăng nhập bằng Firebase. Vui lòng thử lại sau.";

type FirebaseLoginButtonProps = {
  fallback: Route;
  successToast: { title: string; description: string };
  notConfiguredDescription: string;
  genericErrorMessage?: string;
  setError: (message: string | null) => void;
  onPendingChange?: (pending: boolean) => void;
  disabled?: boolean;
};

const FirebaseLoginButton = ({
  fallback,
  successToast,
  notConfiguredDescription,
  genericErrorMessage,
  setError,
  onPendingChange,
  disabled = false
}: FirebaseLoginButtonProps) => {
  const router = useRouter();
  const auth = useAuth();
  const { pushToast } = useToast();

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
      const provider = createGoogleProvider();
      const credential = await signInWithPopup(authInstance, provider);
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

      await auth.updateAuthStateFromAccessToken(response.access_token);
      router.replace(fallback);
    } catch (error) {
      let message = genericErrorMessage ?? DEFAULT_FIREBASE_ERROR;
      if (error instanceof FirebaseError) {
        message = error.message;
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
      try {
        const authInstance = getFirebaseAuthInstance();
        await signOut(authInstance);
      } catch (signOutError) {
        console.error("[Firebase] Không thể đăng xuất Firebase tạm thời", signOutError);
      }
      setIsPending(false);
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
        <LogIn className="h-4 w-4" />
      )}
      Đăng nhập bằng Firebase
    </button>
  );
};

export default FirebaseLoginButton;
