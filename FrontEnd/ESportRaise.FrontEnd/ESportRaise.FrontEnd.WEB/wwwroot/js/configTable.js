var tableVM = new Vue({
    el: '#configTable',
    data: {
        configs: []
    },
    mounted: function () {
        if (!auth.isAdmin) {
            forbiddenPage();
            return;
        }
        this.updateTable();
    },
    methods: {
        updateTable() {
            sendGet('admin/config')
                .then(r => {
                    let data = r.data;
                    let newConfigs = [];
                    for (let config of data) {
                        newConfigs.push({
                            key: config.Key,
                            value: config.Value,
                            prevValue: config.Value
                        });
                    }

                    this.configs = newConfigs;
                }).catch(handleCriticalError);
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
                }).catch(handleCriticalError);
        }
    }
});
