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
        },
        editYT: {
            canEdit: false,
            youTubeUrl: "",
            error: null
        }
    },
    mounted: function () {
        this.editYT.canEdit = this.user.id === -1
            || this.user.id == auth.id
            || auth.isAdmin;
        console.log(auth.isAdmin);
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
                console.log(r);
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
        },
        changeYouTubeId() {
            sendPut('teamMember/youTube', null, {
                teamMemberId: this.user.id,
                channelUrl: this.editYT.youTubeUrl
            }).then(r => {
                console.log(r);
                this.$refs.changeYouTubeIdModal.closeModal();
                this.getTeamMemberInfo();
                }).catch(e => {
                    console.log(e.response);
                if (e.response.status === 400) {
                    this.editYT.error = wrongYouTubeUrl;
                }
            });
        },
        discardYouTubeId() {
            sendPut('teamMember/youTube', null, {
                teamMemberId: this.user.id,
                channelUrl: ""
            }).then(r => {
                this.getTeamMemberInfo();
            }).catch(e => {
                console.log(e.response);
            })
        }
    }
});