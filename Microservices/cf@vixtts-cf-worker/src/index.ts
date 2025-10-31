export interface Env {
  HF_API_KEY: string;
  MODEL_NAME?: string; // optional override, e.g. "capleaf/viXTTS" or "thinhlpg/vixtts-demo"
}

export default {
  async fetch(request: Request, env: Env): Promise<Response> {
    const url = new URL(request.url);
    const path = url.pathname;

    // CORS helper for synthesize
    const synthCorsHeaders = {
      "Access-Control-Allow-Origin": "*",
      "Access-Control-Allow-Methods": "POST, OPTIONS",
      "Access-Control-Allow-Headers": "Content-Type, Authorization",
    } as Record<string, string>;

    function synthResp(body: BodyInit | null, status = 200, headers: Record<string, string> = {}) {
      return new Response(body, { status, headers: { ...synthCorsHeaders, ...headers } });
    }

    // Simple health
    if (path === "/" && request.method === "GET") {
      return new Response(JSON.stringify({ status: "ok", model: env.MODEL_NAME ?? "capleaf/viXTTS" }), {
        headers: { "content-type": "application/json" },
      });
    }

    // handle preflight for synthesize
    if (path === "/synthesize" && request.method === "OPTIONS") {
      return synthResp(null, 204, {});
    }

    if (path === "/synthesize" && request.method === "POST") {
      // Validate bindings
      if (!env?.HF_API_KEY) {
        console.error("Missing HF_API_KEY binding");
        return synthResp(JSON.stringify({ error: "Missing HF_API_KEY. Set it in your Worker environment or wrangler.toml." }), 500, { "content-type": "application/json" });
      }

      let payload: any;
      try {
        payload = await request.json();
      } catch (err) {
        return synthResp(JSON.stringify({ error: "Invalid JSON body" }), 400, { "content-type": "application/json" });
      }

      const text = typeof payload?.text === "string" ? payload.text : null;
      if (!text || text.trim().length === 0) {
        return synthResp(JSON.stringify({ error: "Missing 'text' in request body" }), 400, { "content-type": "application/json" });
      }

      // Choose model from env override or default to capleaf/viXTTS
      const model = (env.MODEL_NAME && env.MODEL_NAME.trim()) || "capleaf/viXTTS";

      // If user requested the known Space thinhlpg/vixtts-demo, call its Spaces API
      if (model === "thinhlpg/vixtts-demo" || model.endsWith("/vixtts-demo")) {
        const spaceUrl = `https://huggingface.co/spaces/thinhlpg/vixtts-demo/api/predict`;
        try {
          // Build data array for Gradio-style Spaces. Many Spaces expect { data: [input, ...] }
          // Build Gradio-style data array: [text, reference_audio? , parameters?]
          const dataArray: any[] = [text];

          // Support reference audio provided as base64 data URL or raw base64 string
          if (payload.reference_audio_base64 && typeof payload.reference_audio_base64 === "string") {
            let base = payload.reference_audio_base64;
            // If it's raw base64 (no data: prefix), assume wav
            if (!base.startsWith("data:")) {
              base = `data:audio/wav;base64,${base}`;
            }
            dataArray.push(base);
          } else if (payload.reference_audio_url && typeof payload.reference_audio_url === "string") {
            // Fetch remote audio and convert to data URL
            try {
              const remote = await fetch(payload.reference_audio_url);
              if (!remote.ok) {
                const t = await remote.text();
                console.error("Failed to fetch reference audio URL", remote.status, t);
                return synthResp(JSON.stringify({ error: "Failed to fetch reference audio URL", status: remote.status, detail: t }), 502, { "content-type": "application/json" });
              }
              const ct = remote.headers.get("content-type") || "audio/wav";
              const ab = await remote.arrayBuffer();
              const bytes = new Uint8Array(ab);
              let binary = "";
              // Convert to binary string
              for (let i = 0; i < bytes.byteLength; i++) {
                binary += String.fromCharCode(bytes[i]);
              }
              const b64 = btoa(binary);
              const dataUrl = `data:${ct};base64,${b64}`;
              dataArray.push(dataUrl);
            } catch (err) {
              console.error("Error fetching reference audio URL:", String(err));
              return synthResp(JSON.stringify({ error: "Error fetching reference audio URL", detail: String(err) }), 502, { "content-type": "application/json" });
            }
          }

          // If caller provided extra parameters or options, append them as next element
          if (payload.parameters && typeof payload.parameters === "object") {
            dataArray.push(payload.parameters);
          }

          const hfResp = await fetch(spaceUrl, {
            method: "POST",
            headers: {
              Authorization: `Bearer ${env.HF_API_KEY}`,
              "Content-Type": "application/json",
              Accept: "application/json",
            },
            body: JSON.stringify({ data: dataArray }),
          });

          if (!hfResp.ok) {
            const textBody = await hfResp.text();
            console.error("Hugging Face Space returned non-200", hfResp.status, textBody);
            let parsed: any = textBody;
            try {
              parsed = JSON.parse(textBody);
            } catch {}
            return synthResp(JSON.stringify({ error: "Hugging Face Space error", status: hfResp.status, detail: parsed }), 502, { "content-type": "application/json" });
          }

          const json = await hfResp.json();
          // Expect json.data[0] to be either a data URL (data:audio/..;base64,...) or a URL string
          const first = json?.data?.[0];
          if (typeof first === "string") {
            if (first.startsWith("data:audio/")) {
              // data URL: data:audio/wav;base64,AAAA
              const parts = first.split(",");
              const meta = parts[0] || ""; // e.g. data:audio/wav;base64
              const b64 = parts[1] || "";
              const match = meta.match(/data:(audio\/[^;]+);base64/);
              const contentType = match ? match[1] : "audio/wav";
              try {
                const raw = Uint8Array.from(atob(b64), (c) => c.charCodeAt(0));
                return synthResp(raw, 200, { "content-type": contentType });
              } catch (err) {
                console.error("Failed to decode base64 audio from Space:", err);
                return synthResp(JSON.stringify({ error: "Failed to decode audio", detail: String(err) }), 500, { "content-type": "application/json" });
              }
            }

            if (first.startsWith("http")) {
              // the Space returned a URL to the audio file; proxy it
              try {
                const remote = await fetch(first);
                if (!remote.ok) {
                  const txt = await remote.text();
                  console.error("Failed fetching remote audio URL from Space", remote.status, txt);
                  return synthResp(JSON.stringify({ error: "Failed to fetch audio URL", status: remote.status }), 502, { "content-type": "application/json" });
                }
                const ct = remote.headers.get("content-type") || "application/octet-stream";
                const buf = await remote.arrayBuffer();
                return synthResp(buf, 200, { "content-type": ct });
              } catch (err) {
                console.error("Error proxying audio URL from Space:", err);
                return synthResp(JSON.stringify({ error: "Proxy error", detail: String(err) }), 502, { "content-type": "application/json" });
              }
            }
          }

          // Fallback: return the full JSON result
          return synthResp(JSON.stringify(json), 200, { "content-type": "application/json" });
        } catch (err) {
          console.error("Error calling Hugging Face Space API:", String(err));
          return synthResp(JSON.stringify({ error: "Internal fetch error", detail: String(err) }), 502, { "content-type": "application/json" });
        }
      }

      // Build HF inference request. Many TTS models on HF accept JSON { inputs: string } and return audio when Accept header is set
      const hfUrl = `https://api-inference.huggingface.co/models/${model}`;
      try {
        const hfResp = await fetch(hfUrl, {
          method: "POST",
          headers: {
            Authorization: `Bearer ${env.HF_API_KEY}`,
            Accept: "audio/wav, audio/mpeg, application/json",
            "Content-Type": "application/json",
          },
          // Some HF TTS models accept { inputs: "...", parameters: { ... } }
          body: JSON.stringify({ inputs: text, parameters: payload.parameters ?? {} }),
        });

        // If HF returns non-2xx, return JSON detail
        if (!hfResp.ok) {
          const textBody = await hfResp.text();
          console.error("Hugging Face returned non-200", hfResp.status, textBody);
          // Try parse JSON body
          let parsed: any = textBody;
          try {
            parsed = JSON.parse(textBody);
          } catch { /* leave as text */ }
          return synthResp(JSON.stringify({ error: "Hugging Face inference error", status: hfResp.status, detail: parsed }), 502, { "content-type": "application/json" });
        }

        // Forward content-type and body stream
        const contentType = hfResp.headers.get("content-type") || "application/octet-stream";
        // If HF returned JSON (e.g. a model that responds with JSON), forward that
        if (contentType.includes("application/json")) {
          const jsonBody = await hfResp.json();
          return synthResp(JSON.stringify(jsonBody), 200, { "content-type": "application/json" });
        }

        // Otherwise assume audio stream
        const audioBuffer = await hfResp.arrayBuffer();
        return synthResp(audioBuffer, 200, { "content-type": contentType });
      } catch (err) {
        console.error("Error calling Hugging Face Inference API:", String(err));
        return synthResp(JSON.stringify({ error: "Internal fetch error", detail: String(err) }), 502, { "content-type": "application/json" });
      }
    }

    return new Response(JSON.stringify({ error: "Not found" }), { status: 404, headers: { "content-type": "application/json" } });
  },
};

