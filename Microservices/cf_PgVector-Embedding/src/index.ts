export interface Env {
	VECTORIZE: Vectorize;
	AI: Ai;
}
interface EmbeddingResponse {
	shape: number[];
	data: number[][];
}

export default {
	async fetch(request, env, ctx): Promise<Response> {
		let path = new URL(request.url).pathname;

		// You only need to generate vector embeddings once (or as
		// data changes), not on every request
		if (path === "/embed" && request.method === "POST") {
			// In a real-world application, you could read content from R2 or
			// a SQL database (like D1) and pass it to Workers AI
			const stories = await request.json();
			const modelResp: EmbeddingResponse = await env.AI.run(
				"@cf/google/embeddinggemma-300m",
				{
					text: stories["texts"] as string[],
				},
			);

			// Convert the vector embeddings into a format Vectorize can accept.
			// Each vector needs an ID, a value (the vector) and optional metadata.
			// In a real application, your ID would be bound to the ID of the source
			// document.
			let vectors: VectorizeVector[] = [];
			let id = 1;
			modelResp.data.forEach((vector) => {
				vectors.push({ id: `${id}`, values: vector });
				id++;
			});

			let inserted = await env.VECTORIZE.upsert(vectors);
			return Response.json({ inserted, vectors });
		}
		if (path === "/query" && request.method === "POST") {
			const queryData = await request.json();
			let userQuery = queryData["texts"][0];
			const queryVector: EmbeddingResponse = await env.AI.run(
				"@cf/google/embeddinggemma-300m",
				{
					text: [userQuery],
				},
			);

			let matches = await env.VECTORIZE.query(queryVector.data[0], {
				topK: 1,
			});
			return Response.json({
				// Expect a vector ID. 1 to be your top match with a score of
				// ~0.89693683
				// This tutorial uses a cosine distance metric, where the closer to one,
				// the more similar.
				matches: matches,
			});
		}
		return new Response("", { status: 404 });
	},
} satisfies ExportedHandler<Env>;