let certExpVM = new Vue({
    el: "#certExpiration",
    data: {
        expiration: null
    },
    mounted() {
        axios.get('/Admin/SslExpiration')
            .then(r => {
                this.expiration = new Date(r.data.expiresAt);
            }).catch(e => {

            });
    }
});