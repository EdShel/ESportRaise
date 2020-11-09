function getToken() {
    localStorage.getItem("token");
}

function authorize() {


}
let navVM = new Vue({
    el: "#navSection",
    data: {
        whatIsOpened: "nothing",
        loginData: {
            user: "",
            password: ""
        },
        registerData: {
            user: "",
            password: ""
        }
    },
    computed: {
        isAuthorized() {
            let authToken = localStorage.getItem("token");
            return authToken == true;
        }
    },
    methods: {
        openLogin() {
            whatIsOpened = "login"
        },
        openRegister() {
            whatIsOpened = "register"
        },
        close() {
            wharIsOpened = "nothing"
        }
    }
})
