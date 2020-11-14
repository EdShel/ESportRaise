let userVM = new Vue({
    el: '#user',
    data: {
        user: {
            id: userId,
            userName: null,
            email: null
        },
        teamMember: {
            youTubeId: null
        },
        team: {
            exists: false,
            id: null,
            name: null
        }
    },
    mounted: function () {
        this.getUserInfo();
    },
    methods: {
        getUserInfo() {
            let userInfoPromise =
                this.user.id === -1
                    ? sendGet('auth/me')
                    : sendGet('auth/user', {
                        id: this.user.id
                    });

            userInfoPromise.then(r => {
                let data = r.data;
                this.user.id = data.id;
                this.user.userName = data.name;
                this.user.email = data.email;

                this.getTeamMemberInfo();
            }).catch(error => {
                console.log(error);
                alert(error);
            });
        },
        getTeamMemberInfo() {
            sendGet('teamMember/user', {
                id: this.user.id
            }).then(r => {
                let data = r.data;
                this.team.id = data.teamId;
                this.teamMember.youTubeId = data.youTubeId;

                this.getTeamInfo();
            }).catch(e => {
                this.team.exists = false;
            });
        },
        getTeamInfo() {
            sendGet('team/full', {
                id: this.team.id
            }).then(r => {
                let data = r.data;
                this.team.name = data.name;
                this.team.exists = true;
            }).catch(handle);
        }
    }
});