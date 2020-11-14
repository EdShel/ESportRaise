let teamVM = new Vue({
    el: "#team",
    data: {
        team: {
            id: teamId,
            name: null,
            members: []
        },
        newTeam: {
            name: null
        }
    },
    mounted: function () {
        this.getTeamInfo();
    },
    methods: {
        async getTeamInfo() {
            if (this.team.id === -1) {
                this.team.id = await this.getCurrentUserTeamId();
                console.log(this.team.id);
            }

            sendGet('team/full', {
                id: this.team.id
            }).then(r => {
                let data = r.data;
                this.team.id = data.id;
                this.team.name = data.name;
                this.team.members = data.members;
            }).catch(error => {
                console.log(error);
                alert(error);
            });
        },
        getCurrentUserTeamId() {
            return sendGet('teamMember/me')
                .then(r => {
                    let data = r.data;
                    return data.teamId;
                }).catch(e => {
                    // TODO: don't have an access, redirect
                });
        },
        registerTeam() {
            sendPost('team/create', null, { teamName: this.newTeam.name })
                .then(r => {
                    let data = r.data;
                    this.team.id = data.teamId;
                    this.getTeamInfo();
                }).catch(e => {
                    handle(e);
                });
        }
    }
})