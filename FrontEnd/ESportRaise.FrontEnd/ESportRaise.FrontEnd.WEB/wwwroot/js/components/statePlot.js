Vue.component('stateplot', {
    extends: VueChartJs.Line,
    props: ["stateRecords"],
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
                    y: state.temperature
                });
            }
            return points;
        },
        chartDatasets() {
            return {
                datasets: [
                    {
                        label: 'Коммиты на GitHub',
                        backgroundColor: '#f87979',
                        data: this.temperatureAsCartesianPoints
                    }
                ]
            };
        }
    },
    mounted() {
        this.renderChart(this.chartDatasets, this.options)
    },
    methods: {
        update() {
            console.log("updating");
            if (this._chart) {
                this._chart.destroy();
            }
            this.renderChart(this.chartDatasets, this.options);
        }
    }
})