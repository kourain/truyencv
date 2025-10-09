import { getHttpClient } from "@helpers/httpClient";
import type { AxiosRequestConfig } from "axios";

export type PingResponse = {
  message: string;
};

export const fetchPing = async (config?: AxiosRequestConfig) => {
  const client = getHttpClient();
  const response = await client.get<PingResponse>("/ping", config);

  return response.data;
};
