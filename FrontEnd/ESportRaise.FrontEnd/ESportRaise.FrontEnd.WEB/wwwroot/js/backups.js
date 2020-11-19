let backupsVM = new Vue({
    el: "#backups",
    data: {
        file: "backup",
        successMessageSeen: false
    },
    methods: {
        download() {
            let fileName = this.file + '.bak';
            downloadFile('admin/getBackup', { file: fileName }, fileName)
                .catch(e => {
                    console.log(e.response);
                });
        },
        make() {
            sendPost('admin/backupDb', null, {
                backupFile: this.file + ".bak"
            }).then(r => {
                this.successMessageSeen = true;
                setTimeout(() => this.successMessageSeen = false, 5000);
            }).catch(e => {

            });

        }
    }
});