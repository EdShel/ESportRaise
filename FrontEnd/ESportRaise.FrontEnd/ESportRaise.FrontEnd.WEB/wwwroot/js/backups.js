let backupsVM = new Vue({
    el: "#backups",
    data: {
        file: "backup",
        successMessageSeen: false,
        errorMessage: null
    },
    methods: {
        download() {
            let fileName = this.file.trim() + '.bak';
            downloadFile('admin/getBackup', { file: fileName }, fileName)
                .then(r => {
                    console.log("Lol)");

                    this.errorMessage = null;
                }).catch(e => {
                    if (e.response.status === 400) {
                        this.errorMessage = wrongBackupFile;
                    }
                    else if (e.response.status === 404) {
                        this.errorMessage = backupNotFound;
                    }
                });
        },
        make() {
            sendPost('admin/backupDb', null, {
                backupFile: this.file.trim() + ".bak"
            }).then(r => {
                this.successMessageSeen = true;
                setTimeout(() => this.successMessageSeen = false, 5000);
            }).catch(e => {
                if (e.response.status === 400) {
                    this.errorMessage = wrongBackupFile;
                }
                else if (e.response.status === 500) {
                    this.errorMessage = unableToMakeBackup;
                }
            });
        }
    }
});