
const backendServer = "http://localhost:5002/";

let auth = new Vue({
    el: "#navSection",
    data: {
        openedModal: null,
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
        openModal(modalId) {
            let el = document.getElementById(modalId);
            this.openedModal = el;
            el.classList.add("visible");
        },
        closeModal() {
            this.openedModal.classList.remove("visible");
            this.openedModal = null;
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
                loginData.user = registerData.userName;
                loginData.password = registerData.password;
                login();
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

document.addEventListener("click", function (e) {
    if (e.target === auth.openedModal) {
        auth.closeModal();
    }
})

function sendGet(url) {
    if (auth.isAuthorized) {
        return axios.get(backendServer + url, {
            headers: {
                Authorization: 'Bearer ' + auth.user.token
            }
        });
    }
    return axios.get(backendServer + url);
}

function sendPost(url, data) {
    if (auth.isAuthorized) {
        return axios.post(backendServer + url, data, {
            headers: {
                Authorization: 'Bearer ' + auth.user.token
            }
        });
    }
    return axios.post(backendServer + url, data);
}
