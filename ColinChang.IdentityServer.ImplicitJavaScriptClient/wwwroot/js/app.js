(function () {
    let mgr = new Oidc.UserManager({
        authority: "https://localhost:5000",
        client_id: "ImplicitJavaScriptClient",
        redirect_uri: window.location.origin + "/signin-oidc.html",
        post_logout_redirect_uri: window.location.origin + "/signout-oidc.html",
        silent_redirect_uri: window.location.origin + "/silent-oidc.html",
        automaticSilentRenew: true,
        response_type: "id_token token",
        scope: "WeatherApi openid profile",
        revokeAccessTokenOnSignout: true,
    });

    mgr.events.addUserSignedIn(function (e) {
        log("user logged in to the token server");
    });
    mgr.events.addUserSignedOut(function () {
        log("User signed out of OP");
    });
    mgr.events.addAccessTokenExpiring(function () {
        log("Access token expiring...");
    });

    function signIn() {
        mgr.signinRedirect();
    }

    function signOut() {
        mgr.signoutRedirect();
    }

    function renewToken() {
        mgr.signinSilent()
            .then(function () {
                log("silent renew success");
                showTokens();
            }).catch(function (err) {
            log("silent renew error", err);
        });
    }

    function callApi() {
        mgr.getUser().then(function (user) {
            if (!user)
                signIn();
            
            let xhr = new XMLHttpRequest();
            xhr.onload = function () {
                if (xhr.status < 400) {
                    display("#api", JSON.parse(xhr.response));
                    return;
                }
                if (xhr.status == 401)
                    signIn();
                
                log({
                    status: xhr.status,
                    statusText: xhr.statusText,
                    wwwAuthenticate: xhr.getResponseHeader("WWW-Authenticate")
                });
            }
            xhr.open("GET", "https://localhost:10000/WeatherForecast", true);
            xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
            xhr.send();
        });
    }

    function showTokens() {
        mgr.getUser()
            .then(function (user) {
                if (!!user)
                    display("#identityData", user);
                else
                    log("Not logged in");
            });
    }

    document.querySelector("#signIn").addEventListener("click", signIn);
    document.querySelector("#callApi").addEventListener("click", callApi);
    document.querySelector("#signOut").addEventListener("click", signOut);
    document.querySelector("#renew").addEventListener("click", renewToken);
    showTokens();


    function display(selector, data) {
        if (!!data && typeof data !== 'string')
            data = JSON.stringify(data, null, 2);

        document.querySelector(selector).textContent = data;
    }

    function log(data) {
        document.querySelector("#log").innerHTML = "";
        Array.prototype.forEach.call(arguments, function (msg) {
            if (msg instanceof Error)
                msg = "Error: " + msg.message;
            else if (typeof msg !== 'string')
                msg = JSON.stringify(msg, null, 2);
            document.querySelector("#log").innerText += msg + '\r\n';
        });
    }
})();

