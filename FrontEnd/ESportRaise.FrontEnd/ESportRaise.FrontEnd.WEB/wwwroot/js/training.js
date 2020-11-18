﻿let trainingVM = new Vue({
    el: "#training",
    data: {
        trainingId: trainingId,
        teamId: null,
        beginTime: null,
        criticalMoments: [],
        videoStreams: [],
        teamMembers: {},

        currentCriticalMomentIndex: criticalMomentIndex,
        currentVideoIndex: -1
    },
    computed: {
        currentCriticalMoment: {
            get() {
                if (this.currentCriticalMomentIndex === -1) {
                    return null;
                }

                return this.criticalMoments[this.currentCriticalMomentIndex]
            },
            set(value) {
                for (let i = 0; i < this.criticalMoments.length; i++) {
                    if (value === this.criticalMoments[i]) {
                        this.currentCriticalMomentIndex = i;
                        return;
                    }
                }
            }
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
                this.teamMembers = data.members;

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
            }).catch(e => {

            });
        },
        secondsToHHMMSS(secs) {
            return new Date(secs * 1000).toISOString().substr(11, 8);
        },
        getTeamMemberNameById(memberId) {
            for (let member of this.teamMembers) {
                if (member.id === memberId) {
                    return member.name;
                }
            }
            return null;
        }
    }
});

function datesDifferenceSeconds(bigger, smaller) {

    let diff = (bigger - smaller) / 1000;
    return Math.round(diff);

}