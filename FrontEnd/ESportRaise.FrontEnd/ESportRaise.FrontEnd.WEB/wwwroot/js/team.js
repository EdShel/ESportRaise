﻿let teamVM = new Vue({
    el: "#team",
    data: {
        edit: {
            turnedOn: false
        },
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
                if (this.team.id === -1) {
                    return;
                }
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
                    console.log(e.response);
                    // Doesn't have a team
                    if (e.response.status === 400) {
                        return -1;
                    }
                    else {
                        // TODO: redirect
                    }
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
        },
        switchEditingMode() {
            this.edit.turnedOn = !this.edit.turnedOn;
        },
        removeMember(memberIndex) {
            let memberName = this.team.members[memberIndex].name;
            if (!confirm(removeMemberConfirmBegin + memberName + removeMemberConfirmEnd)) {
                return;
            }

            sendPost('team/removeMember', null, {
                teamId: this.team.id,
                user: memberName
            }).then(r => {
                this.team.members.splice(memberIndex, 1);
                if (this.team.members.length === 0) {
                    location.reload();
                }
            }).catch(e => {
                handle(e);
            });
        }
    }
})