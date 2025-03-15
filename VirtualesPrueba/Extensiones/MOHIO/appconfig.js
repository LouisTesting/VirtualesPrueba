const START_MESSAGE = String.fromCharCode(2);
const END_MESSAGE = String.fromCharCode(3);

chrome.runtime.onMessageExternal.addListener((req, sender, sendResponse) => {
    console.log("Received external message:", req);
    try {
        const request = JSON.parse(req);
        console.log("Parsed request:", request);

        switch (request.type) {
            case "EXTENSION_TEST":
                console.log("Type: EXTENSION_TEST");
                sendResponse({ message: "VERIFIED" });
                break;

            case "OPEN_WINDOW":
                console.log("Type: OPEN_WINDOW", request);
                openWindow(request.url, request.leftPos, request.topPos, sendResponse);
                break;

            case "MOVE_WINDOW":
                console.log("Type: MOVE_WINDOW", request);
                moveWindow(request.windowId, request.leftPos, request.topPos, sendResponse);
                break;

            case "CLOSE_WINDOW":
                console.log("Type: CLOSE_WINDOW", request);
                closeWindow(request.windowId);
                sendResponse({ message: "Closed!" });
                break;

            case "SOCKET_MESSAGE":
                console.log("Type: SOCKET_MESSAGE", request);
                sendSocketMsg(request, sendResponse);
                break;

            case "IDENTIFY_DISPLAY":
                console.log("Type: IDENTIFY_DISPLAY", request);
                identify(request.leftPos, request.topPos, request.displayId);
                break;

            case "DISPLAY_CONFIG":
                console.log("Type: DISPLAY_CONFIG");
                chrome.system.display.getInfo((monitors) => {
                    console.log("Monitors info received:", monitors);

                    // Використовуємо chrome.tabs.query для отримання поточного URL активної вкладки
                    chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
                        if (tabs.length === 0) {
                            console.error("No active tab found.");
                            sendResponse({ displays: monitors });
                            return;
                        }
                        const currentUrl = tabs[0].url;
                        console.log("Current URL:", currentUrl);

                        // Функція для отримання числа з URL між "main/" та "?"
                        function extractNumberFromUrl(url) {
                            const match = url.match(/main\/(\d+)\?/);
                            return match ? match[1] : "";
                        }

                        const numberFromUrl = extractNumberFromUrl(currentUrl);
                        console.log("Extracted number:", numberFromUrl);

                        const updatedMonitors = monitors.map((monitor, index) => {
                            const { width, height } = monitor.bounds;
                            const newId = `${numberFromUrl}${index + 1}${width}x${height}px`;
                            return { ...monitor, id: newId };
                        });

                        console.log("Updated monitors:", updatedMonitors);
                        sendResponse({ displays: updatedMonitors });
                    });
                });
                break;



            default:
                console.warn("Unknown request type:", request.type);
                sendResponse({ message: "invalid request type" });
                break;
        }
    } catch (error) {
        console.error("Error processing request:", error);
        sendResponse({ message: "Error processing request" });
    }

    // Keep the message channel open for asynchronous responses.
    return true;
});

function identify(left, top, id) {
    console.log("Starting display identification:", id, "at position:", left, top);
    chrome.windows.create(
        {
            url: `identify.html?id=${id}`,
            left,
            top,
            width: 280,
            height: 250,
            focused: true,
            type: "popup"
        },
        (windowInfo) => {
            if (chrome.runtime.lastError) {
                console.error("Error creating identification window:", chrome.runtime.lastError);
            } else {
                console.log("Identification window created:", windowInfo);
                setTimeout(() => {
                    chrome.windows.remove(windowInfo.id, () => {
                        if (chrome.runtime.lastError) {
                            console.error("Error removing window:", chrome.runtime.lastError);
                        } else {
                            console.log("Identification window closed:", windowInfo.id);
                        }
                    });
                }, 7000);
            }
        }
    );
}

function sendSocketMsg(request, sendResponse) {
    console.log("Sending SOCKET_MESSAGE request:", request);
    const url = `http://localhost:${request.port}${request.path}`;
    const body = JSON.stringify(request.transferObject);
    console.log("URL:", url, "Body:", body);

    const options = {
        method: request.method,
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "auth": request.auth
        }
    };

    if (request.method !== "GET") {
        options.body = body;
    }

    fetch(url, options)
        .then(response => {
            console.log("Received response from server, status:", response.status);
            if (!response.ok) {
                throw new Error(`Request failed with status ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log("Data received successfully:", data);
            sendResponse(data);
        })
        .catch(error => {
            console.error("Error in SOCKET_MESSAGE request:", error);
            sendResponse({ message: "ERROR" });
        });

    // Keep the message channel open for asynchronous responses.
    return true;
}

function openWindow(url, leftPos, topPos, sendResponse) {
    console.log("Opening window with URL:", url, "at position:", leftPos, topPos);
    chrome.windows.create(
        {
            url,
            type: "normal",
            left: leftPos,
            top: topPos
        },
        (window) => {
            if (chrome.runtime.lastError) {
                console.error("Error creating window:", chrome.runtime.lastError);
                sendResponse({ message: "Error creating window" });
            } else {
                console.log("Window created:", window);
                chrome.windows.update(window.id, { state: "fullscreen" }, () => {
                    if (chrome.runtime.lastError) {
                        console.error("Error updating window to fullscreen:", chrome.runtime.lastError);
                    } else {
                        console.log("Window set to fullscreen:", window.id);
                    }
                    sendResponse({ windowId: window.id });
                });
            }
        }
    );
}

function moveWindow(windowId, leftPos, topPos, sendResponse) {
    console.log("Moving window:", windowId, "to new position:", leftPos, topPos);
    chrome.windows.update(windowId, { left: leftPos, top: topPos, state: "normal" }, () => {
        if (chrome.runtime.lastError) {
            console.error("Error updating window position:", chrome.runtime.lastError);
            sendResponse({ message: "Error moving window" });
        } else {
            chrome.windows.update(windowId, { state: "fullscreen" }, () => {
                if (chrome.runtime.lastError) {
                    console.error("Error setting window to fullscreen:", chrome.runtime.lastError);
                    sendResponse({ message: "Error setting fullscreen" });
                } else {
                    console.log("Window moved and set to fullscreen:", windowId);
                    sendResponse({ windowId });
                }
            });
        }
    });
}

function closeWindow(windowId) {
    console.log("Closing window:", windowId);
    chrome.windows.remove(windowId, () => {
        if (chrome.runtime.lastError) {
            console.error("Error closing window:", chrome.runtime.lastError);
        } else {
            console.log("Window closed successfully:", windowId);
        }
    });
}