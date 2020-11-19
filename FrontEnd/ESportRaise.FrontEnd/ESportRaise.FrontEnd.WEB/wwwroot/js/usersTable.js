var tableVM = new Vue({
    el: '#usersTable',
    data: {
        pagesCount: 0,
        pageIndex: 0,
        users: [],
        userName: ""
    },
    mounted: function () {
        this.updateTablePage();
    },
    methods: {
        nextPage() {
            let prevPage = this.pageIndex;
            this.pageIndex = Math.max(0, Math.min(prevPage + 1, this.pagesCount));
            if (this.pageIndex != prevPage) {
                this.updateTablePage();
            }
        },
        prevPage() {
            let prevPage = this.pageIndex;
            this.pageIndex = Math.max(0, Math.min(prevPage - 1, this.pagesCount));
            if (this.pageIndex != prevPage) {
                this.updateTablePage();
            }
        },
        updateTablePage(pageIndex) {
            pageIndex = Math.max(0, Math.min(this.pageIndex, this.pagesCount));
            sendGet('admin/users', {
                pageIndex: this.pageIndex,
                pageSize: 10,
                name: this.userName.trim()
            }).then(r => {
                let data = r.data;
                console.log(data);
                this.pageIndex = data.pageIndex;
                this.pageSize = data.pageSize;
                this.pagesCount = data.totalPagesCount;
                this.users = data.users;
            }).catch(e => {
                console.log(e.response.data);
            });
        }
    }
});
