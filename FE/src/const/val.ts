
export const emptyServerAuthState = {
  auth: {
    access_token: "",
    access_token_minutes: 0,
    refresh_token: "",
    refresh_token_days: 0
  },
  userProfile: {
    id: "-1",
    name: "Guest",
    email: "",
  } as UserProfileResponse
};