Vue.component('state-displayer', {
    params: ["trainingId", "usersList"],
    data: function () {
        return {
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        }
    },
    computed: {
        temperatureAsCartesianPoints() {
            if (!this.stateRecords) {
                return [];
            }

            let points = [];
            for (let state of this.stateRecords) {
                points.push({
                    x: new Date(state.createTime),
                    y: stateRecords.temperature
                });
            }
            return points;
        }
    },
    mounted() {
        this.renderChart({
            datasets: [
                {
                    label: 'Коммиты на GitHub',
                    backgroundColor: '#f87979',
                    data: this.temperatureAsCartesianPoints
                }
            ]
        }, this.options)
    }
})