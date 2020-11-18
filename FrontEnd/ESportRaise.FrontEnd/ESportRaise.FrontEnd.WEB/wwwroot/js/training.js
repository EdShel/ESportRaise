let trainingVM = new Vue({
    el: "#training",
    data: {
        trainingId: trainingId,
        teamId: null,
        beginTime: null,
        criticalMoments: [],
        videoStreams: [],
        teamMembers: {},

        currentCriticalMomentIndex: criticalMomentIndex,
        currentVideoIndex: -1,

        stateRecordsOfMember: {}
    },
    computed: {
        currentCriticalMoment() {
            if (this.currentCriticalMomentIndex === -1) {
                return null;
            }

            return this.criticalMoments[this.currentCriticalMomentIndex]
        },
        videosForMoment() {
            if (this.currentCriticalMomentIndex === -1) {
                return [];
            }
            let minTime = new Date(this.currentCriticalMoment.begin);
            let maxTime = new Date(this.currentCriticalMoment.end);
            let videosThatMatch = [];
            for (let video of this.videoStreams) {
                if (new Date(video.startTime) <= minTime && maxTime <= new Date(video.endTime)) {
                    videosThatMatch.push(video);
                }
            }
            return videosThatMatch;
        },
        currentVideo() {
            if (this.currentCriticalMoment
                && this.videosForMoment.length > 0
                && this.videosForMoment.length > this.currentVideoIndex) {
                return this.videosForMoment[this.currentVideoIndex];
            }
            return null;
        },
        momentVideoInterval() {
            if (!this.currentVideo) {
                return null;
            }

            let videoBegin = new Date(this.currentVideo.startTime);
            let criticalMoment = this.currentCriticalMoment;

            return {
                begin: Math.round((new Date(criticalMoment.begin) - videoBegin) / 1000),
                end: Math.round((new Date(criticalMoment.end) - videoBegin) / 1000)
            };
        },
        restartVideoUrl() {
            if (!this.currentVideo) {
                return;
            }
            let interval = this.momentVideoInterval;

            let videoUrl = '//www.youtube.com/embed/' + this.currentVideo.streamId;
            let playerParams = '?rel=0';//'?rel=0&autoplay=1';
            let spanParams = '&start=' + interval.begin + '&end=' + interval.end;
            let langParam = '&hl=en';

            return videoUrl + playerParams + spanParams + langParam;
        },
        currentMemberState() {
            if (this.teamMembers.length == 0) {
                return [];
            }
            return this.stateRecordsOfMember[4];
        }
    },
    created: function () {
        this.getTrainingInfo();
    },
    methods: {
        getTrainingInfo() {
            sendGet('training', {
                id: this.trainingId
            }).then(r => {
                let data = r.data;
                this.teamId = data.teamId;
                this.beginTime = new Date(data.beginTime);

                this.getTeamMembersInfo();
            }).catch(e => {

            });
        },
        getTeamMembersInfo() {

            sendGet('team/full', {
                id: this.teamId
            }).then(r => {
                let data = r.data;
                console.log(data);
                this.teamMembers = {};
                for (let member of data.members) {
                    this.teamMembers[member.id] = {
                        name: member.name
                    };
                }

                this.getCriticalMoments();
            }).catch(e => {

            });
        },
        getCriticalMoments() {
            sendGet('criticalMoment/all', {
                id: this.trainingId
            }).then(r => {
                let data = r.data;
                this.criticalMoments = data.moments;
                if (this.criticalMoments.length > 0) {
                    this.currentCriticalMomentIndex = 0;
                }

                this.getVideos();
            }).catch(e => {

            });
        },
        getVideos() {
            sendGet('training/broadcast', {
                id: this.trainingId
            }).then(r => {
                let data = r.data;
                this.videoStreams = data.streams;

                if (this.videoStreams.length > 0) {
                    this.currentVideoIndex = 0;
                }

                this.getStateRecords();
            }).catch(e => {

            });
        },
        getStateRecords() {
            sendGet('stateRecord/all', {
                trainingId: this.trainingId
            }).then(r => {
                let data = r.data;
                for (let record of data.records) {
                    console.log(record);
                    let member = record.teamMemberId;
                    if (this.stateRecordsOfMember[member]) {
                        this.stateRecordsOfMember[member].push(record);
                    } else {
                        this.stateRecordsOfMember[member] = [record];
                    }
                }
            }).catch(e => {

            });
        },
        secondsToHHMMSS(secs) {
            return new Date(secs * 1000).toISOString().substr(11, 8);
        }
    }
});

function datesDifferenceSeconds(bigger, smaller) {

    let diff = (bigger - smaller) / 1000;
    return Math.round(diff);

}