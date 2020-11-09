function getToken() {
    localStorage.getItem("token");
}

function authorize() {


}

let navVM = new Vue({
    el: "#navSection",
    data: {
        openedModal: null,
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
        openModal(modalId) {
            let el = document.getElementById(modalId);
            this.openedModal = el;
            el.classList.add("visible");
            console.log(el);
        },
        closeModal() {
            this.openedModal.classList.remove("visible");
            this.openedModal = null;
        }
    }
})

document.addEventListener("click", function (e) {
    if (e.target === navVM.openedModal) {
        navVM.closeModal();
    }
})

