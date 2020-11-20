
const backendServer = "https://localhost:5003/";

let auth = new Vue({
    el: "#navSection",
    data: {
        loginData: {
            user: "",
            password: ""
        },
        registerData: {
            email: "",
            userName: "",
            password: ""
        }
    },
    computed: {
        isAuthorized() {
            let authToken = localStorage.getItem("token");
            return authToken !== null;
        },
        isAdmin() {
            let authToken = localStorage.getItem("token");
            let tokenPayload = parseJwt(authToken);
            const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            return tokenPayload[roleClaim] === "Admin";
        },
        user() {
            return {
                email: localStorage.getItem("email"),
                userName: localStorage.getItem("userName"),
                token: localStorage.getItem("token"),
                refreshToken: localStorage.getItem("refreshToken")
            }
        }
    },
    methods: {
        saveUser(obj) {
            localStorage.setItem("email", obj.email);
            localStorage.setItem("userName", obj.userName);
            localStorage.setItem("token", obj.token);
            localStorage.setItem("refreshToken", obj.refreshToken);
        },
        clearUser() {
            localStorage.removeItem("email");
            localStorage.removeItem("userName");
            localStorage.removeItem("token");
            localStorage.removeItem("refreshToken");
        },
        openLoginModal() {
            this.$refs.loginModal.openModal();
        },
        openRegisterModal() {
            this.$refs.registerModal.openModal();
        },
        login() {
            axios.post(backendServer + "auth/login", {
                emailOrUserName: this.loginData.user,
                password: this.loginData.password
            }).then(r => {
                this.saveUser(r.data);
                location.reload();
            }).catch(error => {
                console.log(error);
            });
        },
        register() {
            axios.post(backendServer + "auth/register", {
                email: this.registerData.email,
                userName: this.registerData.userName,
                password: this.registerData.password,
                role: "Member"
            }).then(reponse => {
                this.loginData.user = this.registerData.userName;
                this.loginData.password = this.registerData.password;
                this.login();
            }).catch(error => {
                console.log(error.request);
                console.log(error.response);
                console.log(error.message);
            });
        },
        logout() {
            this.clearUser();
            location.reload();
        }
    }
})

function sendGet(url, params) {
    if (auth.isAuthorized) {
        return axios.get(backendServer + url, {
            params: params,
            headers: {
                Authorization: 'Bearer ' + auth.user.token
            }
        });
    }
    return axios.get(backendServer + url);
}

function sendPost(url, params, data) {
    if (auth.isAuthorized) {
        return axios.post(backendServer + url, data, {
            params: params,
            headers: {
                Authorization: 'Bearer ' + auth.user.token
            }
        });
    }
    return axios.post(backendServer + url, data, {
        params: params
    });
}

function sendPut(url, params, data) {
    if (auth.isAuthorized) {
        return axios.put(backendServer + url, data, {
            params: params,
            headers: {
                Authorization: 'Bearer ' + auth.user.token
            }
        });
    }
    return axios.post(backendServer + url, data, {
        params: params
    });
}

function downloadFile(url, params, fileName) {
    return axios({
        url: backendServer + url,
        headers: {
            Authorization: 'Bearer ' + auth.user.token
        },
        params: params,
        method: 'GET',
        responseType: 'blob',
    }).then((response) => {
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
    });
}

function handle(e) {
    console.log(e);
    console.log(e.request);
    console.log(e.response);
}

function getDateYYYY_MM_DD(date) {
    return date.getFullYear().toString() + '-'
        + (date.getMonth() + 1).toString().padStart(2, 0) + '-'
        + date.getDate().toString().padStart(2, 0);
}

function parseJwt(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
};

function getCookie(name) {
    var cookiestring = RegExp(name + "=[^;]+").exec(document.cookie);
    return decodeURIComponent(
        !!cookiestring
            ? cookiestring.toString().replace(/^[^=]+./, "")
            : "");
}

function getLanguage() {
    let langCookie = getCookie(".AspNetCore.Culture");
    return langCookie.substr(langCookie.length - 2);
}

function formatDateTimeLocale(dateTime) {
    return dateTime.toLocaleString(getLanguage());
}

function formatDateLocale(dateTime) {
    return dateTime.toLocaleDateString(getLanguage());
}

function formatTimeLocale(dateTime) {
    return dateTime.toLocaleTimeString(getLanguage());
}