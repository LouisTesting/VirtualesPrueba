const START_MESSAGE = String.fromCharCode(2);
const END_MESSAGE = String.fromCharCode(3);

chrome.runtime.onMessageExternal.addListener((req, sender, sendResponse) => {
    try {
        const request = JSON.parse(req);

        switch (request.type) {
            case "EXTENSION_TEST":
                sendResponse({ message: "VERIFIED" });
                break;

            case "OPEN_WINDOW":
                openWindow(request.url, request.leftPos, request.topPos, sendResponse);
                break;

            case "MOVE_WINDOW":
                moveWindow(request.windowId, request.leftPos, request.topPos, sendResponse);
                break;

            case "CLOSE_WINDOW":
                closeWindow(request.windowId);
                sendResponse({ message: "Closed!" });
                break;

            case "SOCKET_MESSAGE":
                sendSocketMsg(request, sendResponse);
                break;

            case "IDENTIFY_DISPLAY":
                identify(request.leftPos, request.topPos, request.displayId);
                break;

            case "DISPLAY_CONFIG":
                chrome.system.display.getInfo((monitors) => {
                    sendResponse({ displays: monitors });
                    openWindowWhenReady(monitors);
                });
                break;

            default:
                sendResponse({ message: "invalid request type" });
                break;
        }
    } catch (error) {
        console.error("Error handling request:", error);
        sendResponse({ message: "Error processing request" });
    }

    // Keep the message channel open for asynchronous responses.
    return true;
});

function identify(left, top, id) {
    console.log("Identifying", id);
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
            setTimeout(() => {
                chrome.windows.remove(windowInfo.id, () => {});
            }, 7000);
        }
    );
}

function sendSocketMsg(request, sendResponse) {
    console.log("Sending request", request);
    const url = `http://localhost:${request.port}${request.path}`;
    const body = JSON.stringify(request.transferObject);

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
            if (!response.ok) {
                throw new Error(`Request failed with status ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log("Request successful!");
            sendResponse(data);
        })
        .catch(error => {
            console.error("Request error:", error);
            sendResponse({ message: "ERROR" });
        });

    return true; // Keep the message channel open for asynchronous responses.
}

function openWindow(url, leftPos, topPos, sendResponse) {
    console.log("Opening window");
    chrome.windows.create(
        {
            url,
            type: "normal",
            left: leftPos,
            top: topPos
        },
        (window) => {
            console.log("Window created", window);
            chrome.windows.update(window.id, { state: "fullscreen" });
            sendResponse({ windowId: window.id });
        }
    );
}

function moveWindow(windowId, leftPos, topPos, sendResponse) {
    chrome.windows.update(windowId, { left: leftPos, top: topPos, state: "normal" }, () => {
        chrome.windows.update(windowId, { state: "fullscreen" }, () => {
            sendResponse({ windowId });
        });
    });
}

function closeWindow(windowId) {
    console.log("Removing window", windowId);
    chrome.windows.remove(windowId, () => {
        console.log("Window removed", windowId);
    });
}

function openWindowWhenReady(monitors) {
    if (monitors && monitors.length > 0) {
        console.log("Monitors found:", monitors);

        const urlToOpen = "https://www.youtube.com/";
        chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
            const currentTabUrl = tabs[0].url;

            if (currentTabUrl.includes(".com/cashier/integration/main")) {
                openWindow(urlToOpen, 100, 100, (response) => {
                    console.log("Window opened with YouTube link", response);
                });
            }
        });
    }
}
