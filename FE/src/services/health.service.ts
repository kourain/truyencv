import { getHttpClient } from "@helpers/httpClient";
import type { AxiosRequestConfig } from "axios";

export type HealthStatus = "healthy" | "degraded" | "unhealthy";

type RawPingResponse = {
  message: HealthStatus;
  api: {
    status: HealthStatus;
    timestamp: string;
  };
  dependencies: {
    database: {
      status: HealthStatus;
      latency_ms: number;
      error: string | null;
    };
  };
};

export type PingResponse = RawPingResponse & {
  api: RawPingResponse["api"] & {
    response_time_ms: number;
  };
};
export const fetchPing = async (config?: AxiosRequestConfig) => {
  const client = getHttpClient();
  const start = typeof performance !== "undefined" ? performance.now() : Date.now();
  const response = await client.get<RawPingResponse>("/ping", config);
  const end = typeof performance !== "undefined" ? performance.now() : Date.now();
  const latency = Math.max(0, Math.round(end - start));

  return {
    ...response.data,
    api: {
      ...response.data.api,
      response_time_ms: latency
    }
  } satisfies PingResponse;
};
