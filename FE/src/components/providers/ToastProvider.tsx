"use client";

import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState, type ReactNode } from "react";
import { createPortal } from "react-dom";
import { X } from "lucide-react";

type ToastVariant = "success" | "error" | "info" | "warning";

type ToastOptions = {
  id?: string;
  title?: string;
  description: string;
  variant?: ToastVariant;
  duration?: number;
};

type ToastRecord = Required<Pick<ToastOptions, "id" | "description">> & {
  title?: string;
  variant: ToastVariant;
  duration: number;
};

type ToastContextValue = {
  pushToast: (options: ToastOptions) => string;
  dismissToast: (id: string) => void;
  clearToasts: () => void;
};

const ToastContext = createContext<ToastContextValue | undefined>(undefined);

interface ToastProviderProps {
  children: ReactNode;
  defaultDuration?: number;
}

const DEFAULT_DURATION = 5000;

const variantStyles: Record<ToastVariant, string> = {
  success: "border-emerald-500/60 bg-emerald-500/10 text-emerald-100",
  error: "border-rose-500/60 bg-rose-500/10 text-rose-100",
  info: "border-slate-500/60 bg-slate-500/10 text-slate-100",
  warning: "border-amber-500/60 bg-amber-500/10 text-amber-100",
};

const ToastProvider = ({ children, defaultDuration = DEFAULT_DURATION }: ToastProviderProps) => {
  const [hasMounted, setHasMounted] = useState(false);
  const [toasts, setToasts] = useState<ToastRecord[]>([]);
  const timersRef = useRef<Record<string, number>>({});

  useEffect(() => {
    setHasMounted(true);

    return () => {
      Object.values(timersRef.current).forEach((timerId) => window.clearTimeout(timerId));
    };
  }, []);

  const dismissToast = useCallback((id: string) => {
    setToasts((prev) => prev.filter((toast) => toast.id !== id));
    const timerId = timersRef.current[id];
    if (timerId) {
      window.clearTimeout(timerId);
      delete timersRef.current[id];
    }
  }, []);

  const scheduleDismiss = useCallback(
    (id: string, duration: number) => {
      if (duration <= 0) {
        return;
      }

      const timerId = window.setTimeout(() => {
        dismissToast(id);
      }, duration);

      timersRef.current[id] = timerId;
    },
    [dismissToast],
  );

  const pushToast = useCallback(
    ({ id, title, description, variant = "info", duration = defaultDuration }: ToastOptions) => {
      const toastId = id ?? (typeof crypto !== "undefined" && "randomUUID" in crypto ? crypto.randomUUID() : `${Date.now()}-${Math.random()}`);

      setToasts((prev) => {
        const nextToasts = prev.filter((toast) => toast.id !== toastId);
        return [
          ...nextToasts,
          {
            id: toastId,
            title,
            description,
            variant,
            duration,
          },
        ];
      });

      scheduleDismiss(toastId, duration);
      return toastId;
    },
    [defaultDuration, scheduleDismiss],
  );

  const clearToasts = useCallback(() => {
    setToasts([]);
    Object.values(timersRef.current).forEach((timerId) => window.clearTimeout(timerId));
    timersRef.current = {};
  }, []);

  const value = useMemo<ToastContextValue>(
    () => ({
      pushToast,
      dismissToast,
      clearToasts,
    }),
    [pushToast, dismissToast, clearToasts],
  );

  return (
    <ToastContext.Provider value={value}>
      {children}
      {hasMounted &&
        createPortal(
          <div className="pointer-events-none fixed right-6 top-6 z-[2000] flex w-full max-w-sm flex-col gap-3">
            {toasts.map((toast) => {
              const variantClass = variantStyles[toast.variant];
              return (
                <article
                  key={toast.id}
                  className={`pointer-events-auto rounded-2xl border px-4 py-3 shadow-xl backdrop-blur-lg transition hover:-translate-y-0.5 ${variantClass}`}
                >
                  <div className="flex items-start gap-3">
                    <div className="flex-1">
                      {toast.title && <p className="text-sm font-semibold uppercase tracking-wide">{toast.title}</p>}
                      <p className="text-sm leading-relaxed">{toast.description}</p>
                    </div>
                    <button
                      type="button"
                      onClick={() => dismissToast(toast.id)}
                      className="mt-0.5 rounded-full p-1 text-inherit transition hover:bg-white/10 hover:text-white"
                      aria-label="Đóng thông báo"
                    >
                      <X className="h-4 w-4" />
                    </button>
                  </div>
                </article>
              );
            })}
          </div>,
          document.body,
        )}
    </ToastContext.Provider>
  );
};

export const useToast = () => {
  const context = useContext(ToastContext);

  if (!context) {
    throw new Error("useToast must be used within a ToastProvider");
  }

  return context;
};

export default ToastProvider;
