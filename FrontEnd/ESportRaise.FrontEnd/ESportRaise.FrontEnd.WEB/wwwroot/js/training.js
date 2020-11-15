let trainingVM = new Vue({
    el: "#training",
    data: {
        trainingId: trainingId,
        criticalMoments: []
    },
    methods: {
        getCriticalMoments() {
            sendGet('criticalMoment/all', {
                id: this.trainingId
            }).then(r => {
                let data = r.data;
                this.criticalMoments = data.moments;
            }).catch(e => {

            });
        }
    }
});