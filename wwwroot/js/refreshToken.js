async function checkJwtExpiryAndRefresh() {
    console.log("Checking JWT expiry...");

    const cookies = document.cookie.split(';').reduce((acc, cookie) => {
        const [key, value] = cookie.trim().split('=');
        acc[key] = decodeURIComponent(value);
        return acc;
    }, {});

    const tokenExpires = cookies["TokenExpires"];
    if (!tokenExpires) {
        console.warn("TokenExpires cookie missing.");
        return;
    }

    const exp = parseInt(tokenExpires, 10) * 1000; // Convert seconds to ms
    const now = Date.now();
    const timeLeft = exp - now;
    
    if (timeLeft < 5 * 60 * 1000) {
        console.log("Token expiring soon. Refreshing...");
        try {
            const res = await fetch('/auth/refresh', {
                method: 'POST',
                credentials: 'same-origin'
            });
            if (!res.ok) {
                console.error("Refresh failed:", res.statusText);
            } else {
                console.log("Token refreshed successfully.");
            }
        } catch (err) {
            console.error("Error calling /auth/refresh:", err);
        }
    }
}

// Run check every 1 minute
setInterval(checkJwtExpiryAndRefresh, 60 * 1000);