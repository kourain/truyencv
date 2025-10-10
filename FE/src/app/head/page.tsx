import { headers } from "next/headers";

const PageHead = async () => {
    const requestHeaders = await headers();
    const headersObject: Record<string, string> = {};

    requestHeaders.forEach((value, key) => {
        headersObject[key] = value;
    });

    return (
        <>
            <title>TruyenCV | Request Headers</title>
            <meta name="description" content="Hiển thị thông tin header của request hiện tại." />
            <script
                type="application/json"
                id="request-headers-json"
                dangerouslySetInnerHTML={{ __html: JSON.stringify(headersObject) }}
            />
            <script
                dangerouslySetInnerHTML={{
                    __html: `
                        (function(){
                            try {
                                const elementId = "request-headers-container";
                                let container = document.getElementById(elementId);
                                if (!container) {
                                    container = document.createElement("pre");
                                    container.id = elementId;
                                    container.style.whiteSpace = "pre-wrap";
                                    container.style.wordBreak = "break-word";
                                    container.style.padding = "16px";
                                    container.style.backgroundColor = "#0f172a";
                                    container.style.color = "#f8fafc";
                                    container.style.fontSize = "12px";
                                    container.style.borderRadius = "12px";
                                    container.style.maxWidth = "90vw";
                                    container.style.margin = "16px auto";
                                    document.body.prepend(container);
                                }

                                const jsonElement = document.getElementById("request-headers-json");
                                if (!jsonElement) return;

                                const data = JSON.parse(jsonElement.textContent || "{}");
                                container.textContent = JSON.stringify(data, null, 2);
                            } catch (error) {
                                console.error("Không thể hiển thị request headers:", error);
                            }
                        })();
                    `
                }}
            />
        </>
    );
};

export default PageHead;