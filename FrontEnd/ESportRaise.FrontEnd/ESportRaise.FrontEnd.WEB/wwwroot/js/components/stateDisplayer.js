Vue.component('state-displayer', {
    props: ["trainingId", "usersList"],
    data: function () {
        return {
            chartIsVisible: false,
            records: [],
            memberIndex: 0
        }
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
        }
    },
    methods: {
        updateChart() {
            sendGet('stateRecord', {
                trainingId: this.trainingId,
                memberId: this.member.id
            }).then(r => {
                let data = r.data;
                this.records = data.records;
                this.$refs.plot.update();
            }).catch(e => {

            });
        }
    },
    template: `
<div v-if="usersList && usersList.length > 0">
    <slot></slot>
    <div>
        <select v-model="member" v-on:change="updateChart">
            <option v-for="user in usersList" v-bind:value="user">
                {{ user.name }}
            </option>
        </select>
    </div>
    <button v-on:click="chartIsVisible = !chartIsVisible">Show</button>
    <stateplot ref="plot" v-if="chartIsVisible" v-bind:stateRecords="records"></stateplot>
</div>
`
})