var tableVM = new Vue({
    el: '#configTable',
    data: {
        configs: []
    },
    mounted: function () {
        this.updateTable();
    },
    methods: {
        updateTable() {
            sendGet('admin/config')
                .then(r => {
                    let data = r.data;
                    let newConfigs = [];
                    for (let config of data) {
                        console.log(config);
                        newConfigs.push({
                            key: config.Key,
                            value: config.Value,
                            prevValue: config.Value
                        });
                    }

                    this.configs = newConfigs;
                }).catch(e => {

                });
        },
        saveChanges() {
            let toUpdate = [];
            for (let config of this.configs) {
                if (config.prevValue !== config.value) {
                    toUpdate.push({
                        key: config.key,
                        value: config.value
                    });
                }
            }

            sendPut('admin/config', null, toUpdate)
                .then(r => {
                    this.updateTable();
                }).catch(e => {

                })
        }
    }
});
