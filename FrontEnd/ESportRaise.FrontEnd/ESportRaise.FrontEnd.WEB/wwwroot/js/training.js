let trainingVM = new Vue({
    el: "#training",
    data: {
        trainingId: trainingId,
        criticalMoments: [],
        currentVideoStreamId: 0,
        videoStreams: []
    },
    computed: {
        currentVideo() {
            return this.videoStreams[this.currentVideoStreamId];
        },
        currentVideoSpans() {
            let spans = [];
            let videoBegin = new Date(this.currentVideo.startTime);
            for (let i = 0; i < this.criticalMoments.length; i++) {
                let criticalMoment = this.criticalMoments[i];

                console.log(222);

                spans.push({
                    begin: Math.round((new Date(criticalMoment.begin) - videoBegin) / 1000),
                    end: Math.round((new Date(criticalMoment.end) - videoBegin) / 1000)
                });
            }

            return spans;
        }
    },
    mounted: function () {
        this.getCriticalMoments();
    },
    methods: {
        getCriticalMoments() {
            sendGet('criticalMoment/all', {
                id: this.trainingId
            }).then(r => {
                let data = r.data;
                this.criticalMoments = data.moments;

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

                this.currentVideoStreamId = 0;
            }).catch(e => {

            });
        }
    }
});

function datesDifferenceSeconds(bigger, smaller) {

    let diff = (bigger - smaller) / 1000;
    return Math.round(diff);

}