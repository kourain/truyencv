import { appEnv } from "@const/env";
import { FirebaseApp, getApps, initializeApp } from "firebase/app";
import { Auth, FacebookAuthProvider, GoogleAuthProvider, getAuth } from "firebase/auth";

let cachedApp: FirebaseApp | null = null;

const firebaseConfig = {
  apiKey: appEnv.FIREBASE_API_KEY,
  authDomain: appEnv.FIREBASE_AUTH_DOMAIN,
  projectId: appEnv.FIREBASE_PROJECT_ID,
  storageBucket: appEnv.FIREBASE_STORAGE_BUCKET,
  messagingSenderId: appEnv.FIREBASE_MESSAGING_SENDER_ID,
  appId: appEnv.FIREBASE_APP_ID,
  measurementId: appEnv.FIREBASE_MEASUREMENT_ID || undefined
};

const isBrowser = typeof window !== "undefined";

export const isFirebaseConfigured = () => Boolean(appEnv.FIREBASE_API_KEY && appEnv.FIREBASE_PROJECT_ID);

export const ensureFirebaseApp = (): FirebaseApp | null => {
  if (!isBrowser) {
    return null;
  }

  if (!isFirebaseConfigured()) {
    console.warn("[Firebase] Firebase environment variables are missing. Skipping initialization.");
    return null;
  }

  if (cachedApp) {
    return cachedApp;
  }

  const apps = getApps();
  cachedApp = apps.length ? apps[0] : initializeApp(firebaseConfig);
  return cachedApp;
};

export const getFirebaseAuthInstance = (): Auth => {
  const app = ensureFirebaseApp();
  if (!app) {
    throw new Error("Firebase chưa được cấu hình cho môi trường hiện tại");
  }

  return getAuth(app);
};

export const createGoogleProvider = () => {
  const provider = new GoogleAuthProvider();
  provider.setCustomParameters({ prompt: "select_account" });
  return provider;
};

export const createFacebookProvider = () => {
  const provider = new FacebookAuthProvider();
  provider.addScope("email");
  provider.setCustomParameters({ display: "popup" });
  return provider;
};
