Vue.component('state-displayer', {
    props: ["trainingId", "usersList"],
    data: function () {
        return {
            records: [],
            memberIndex: 0,
            _chart: null
        }
    },
    mounted() {
        this.updateChart();
    },
    computed: {
        member: {
            get() {
                return this.usersList[this.memberIndex];
            },
            set(val) {
                for (let i = 0; i < this.usersList.length; i++) {
                    if (val === this.usersList[i]) {
                        this.memberIndex = i;
                        return;
                    }
                }
            }
        },
        heartrateAsCartesianPoints() {
            if (!this.records) {
                return [];
            }

            let points = [];
            for (let state of this.records) {
                points.push({
                    x: new Date(state.createTime),
                    y: state.heartRate
                });
            }
            return points;
        },
        temperatureAsCartesianPoints() {
            if (!this.records) {
                return [];
            }

            let points = [];
            for (let state of this.records) {
                points.push({
                    x: new Date(state.createTime),
                    y: state.temperature
                });
            }
            return points;
        },
        chartData() {
            return {
                datasets: [
                    {
                        label: 'HR',
                        fill: false,
                        backgroundColor: 'dc3545',
                        borderColor: '#dc3545',
                        data: this.heartrateAsCartesianPoints,
                        yAxisID: 'hrAxis'
                    },
                    {
                        label: 'T',
                        backgroundColor: '#ffc107',
                        borderColor: '#ffc107',
                        data: this.temperatureAsCartesianPoints,
                        yAxisID: 'tAxis'
                    }]
            }
        },
        chartOptions() {
            return {
                responsive: true,
                maintainAspectRatio: true,
                scales: {
                    xAxes: [{
                        type: 'time',
                        time: {
                            format: "hh:mm:ss",
                            tooltipFormat: 'LTS'
                        },
                        position: 'bottom',
                    }],
                    yAxes: [
                        {
                            id: 'tAxis',
                            type: 'linear',
                            position: 'right'
                        },
                        {
                            id: 'hrAxis',
                            type: 'linear',
                            position: 'left'
                        }
                    ]
                }
            }
        }
    },
    methods: {
        getChart() {
            if (this._chart) {
                return this._chart;
            }
            let chartCanvas = document.getElementById("stateChart");
            this._chart = new Chart(chartCanvas.getContext('2d'), {
                type: 'line',
                data: this.chartData,
                options: this.chartOptions
            });
            return this._chart;
        },
        updateChart() {
            sendGet('stateRecord', {
                trainingId: this.trainingId,
                memberId: this.member.id
            }).then(r => {
                let data = r.data;
                this.records = data.records;
                this.getChart().data = this.chartData;
                this.getChart().update();
            }).catch(handleCriticalError);
        }
    },
    template: `
<div v-if="usersList && usersList.length > 0">
    <slot></slot>
    <div>
        <select v-model="member" v-on:change="updateChart" class="browser-default custom-select">
            <option v-for="user in usersList" v-bind:value="user">
                {{ user.name }}
            </option>
        </select>
    </div>
    <div>
        <canvas id="stateChart"></canvas>
    </div>
</div>
`
})