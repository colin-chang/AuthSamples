(function () {
    let mgr = new Oidc.UserManager({
        authority: "https://localhost:5000",
        client_id: "ImplicitJavaScriptClient",
        redirect_uri: window.location.origin + "/signin-oidc.html",
        post_logout_redirect_uri: window.location.origin + "/signout-oidc.html",
        silent_redirect_uri: window.location.origin + "/silent-oidc.html",
        automaticSilentRenew: true,
        response_type: "id_token token",
        scope: "WeatherApi openid profile roles nationalities",
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

    function request(url, method, success) {
        mgr.getUser().then(function (user) {
            if (!user)
                mgr.signinRedirect();

            let xhr = new XMLHttpRequest();
            xhr.onload = function () {
                if (xhr.status < 400) {
                    if (!!success && typeof (success) == "function")
                        success(xhr.response);
                    return;
                }
                if (xhr.status == 401)
                    mgr.signinRedirect();

                log({
                    status: xhr.status,
                    statusText: xhr.statusText,
                    wwwAuthenticate: xhr.getResponseHeader("WWW-Authenticate")
                });
            }
            xhr.open(method, url, true);
            xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
            xhr.send();
        });
    }

    // Implicit
    (function () {
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
            request("https://localhost:10000/WeatherForecast", "GET", function (response) {
                display("#api", JSON.parse(response))
            })
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
    })();

    //RBAC
    (function () {
        function callUserRoleApi() {
            request("https://localhost:10000/Authorization", "GET", function (response) {
                alert(response)
            });
        }

        function callAdministratorRoleApi() {
            request("https://localhost:10000/Authorization", "POST", function (response) {
                alert(response)
            });
        }

        document.querySelector("#callUserRoleApi").addEventListener("click", callUserRoleApi);
        document.querySelector("#callAdministratorRoleApi").addEventListener("click", callAdministratorRoleApi);
    })();
    
    //PBAC
    (function () {
        function callPolicyApi() {
            request("https://localhost:10000/Authorization", "PUT", function (response) {
                alert(response)
            });
        }

        document.querySelector("#callPolicyApi").addEventListener("click", callPolicyApi);
    })();
})();

