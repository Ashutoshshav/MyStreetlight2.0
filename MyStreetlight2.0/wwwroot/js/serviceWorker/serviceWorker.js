self.addEventListener("push", event => {
    let data = {};
    if (event.data) {
        data = event.data.json(); // parse the payload
    }

    const title = data.title || "Default title";
    const options = {
        body: data.body || "Default body",
        icon: "/icon.png",  // optional, must be a valid path in wwwroot
        badge: "/badge.png" // optional
    };

    event.waitUntil(
        self.registration.showNotification(title, options)
    );
});
