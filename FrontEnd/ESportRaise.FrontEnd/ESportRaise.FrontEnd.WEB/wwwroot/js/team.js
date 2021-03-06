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
            name: null,
            error: null
        },
        newTeamMember: {
            emailOrUserName: "",
            errorMessage: null
        },
        trainings: {
            spanType: 1,
            date: getDateYYYY_MM_DD(new Date()),
            list: []
        }
    },
    computed: {
        isValidNewTeamName() {
            return this.newTeam.name && /^[A-Za-z0-9_@]{4,30}$/.test(this.newTeam.name);
        }
    },
    mounted: function () {
        this.getTeamInfo();
    },
    methods: {
        async getTeamInfo() {
            if (this.team.id === -1) {
                this.team.id = await this.getCurrentUserTeamId();
                if (this.team.id === -1) {
                    this.team.id = -2;
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

                this.updateTrainings();
            }).catch(handleCriticalError);
        },
        getCurrentUserTeamId() {
            return sendGet('teamMember/me')
                .then(r => {
                    let data = r.data;
                    return data.teamId;
                }).catch(e => {
                    // Doesn't have a team
                    if (e.response.status === 400) {
                        return -1;
                    }
                    else {
                        handleCriticalError(e);
                    }
                });
        },
        registerTeam() {
            sendPost('team/create', null, { teamName: this.newTeam.name })
                .then(r => {
                    let data = r.data;
                    this.team.id = data.teamId;
                    this.getTeamInfo();
                }).catch(handleCriticalError);
        },
        switchEditingMode() {
            this.edit.turnedOn = !this.edit.turnedOn;
        },
        addMember() {
            sendPost('team/addMember', null, {
                teamId: this.team.id,
                user: this.newTeamMember.emailOrUserName
            }).then(r => {
                this.getTeamInfo();
                this.$refs.newTeamMemberModal.closeModal();
            }).catch(e => {
                if (e.response.status === 400 || e.response.status === 500) {
                    this.newTeamMember.errorMessage = userIsAlreadyInTeam;
                }
                else if (e.response.status === 404) {
                    this.newTeamMember.errorMessage = userNotFound;
                }
                else {
                    handleCriticalError(e);
                }
            });
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
            }).catch(handleCriticalError);
        },
        goToUserPage(memberIndex) {
            location.assign('/user?id=' + this.team.members[memberIndex].id);
        },
        updateTrainings() {
            let date;
            let hours;
            if (this.trainings.spanType == 0) {
                date = new Date().toISOString();
                hours = 24 * 7;
            } else if (this.trainings.spanType == 1) {
                date = new Date().toISOString();
                hours = 24 * 30;
            }
            else if (this.trainings.spanType == 2) {
                let chosenDate = new Date(this.trainings.date);
                chosenDate.setDate(chosenDate.getDate() + 1);
                date = chosenDate.toISOString();
                hours = 24;
            }
            sendGet('training/beforeDay', {
                id: this.team.id,
                dateTime: date,
                hours: hours
            }).then(r => {
                let data = r.data;
                this.trainings.list = data.trainings;
            }).catch(handleCriticalError);
        },
        goToTrainingPage(trainingId) {
            location.assign('/Training?id=' + trainingId);
        },
        formatTrainingTime(time) {
            return formatDateTimeLocale(new Date(time));
        }
    }
})