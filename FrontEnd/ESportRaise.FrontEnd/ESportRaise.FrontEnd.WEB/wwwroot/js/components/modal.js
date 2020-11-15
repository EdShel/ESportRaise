Vue.component('modal', {
    props: ['header', 'closetext', 'oktext'],
    data: function () {
        return {
            count: 0
        }
    },
    methods: {
        openModal() {
            this.$el.classList.add("visible");
        },
        closeModal() {
            this.$el.classList.remove("visible");
            this.$emit('cancel');
        }
    },
    template: `
<div class="modal" id="loginModal" v-on:click.self="closeModal">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="exampleModalLongTitle">
                    {{ header }}
                </h5>
                <button type="button" class="close" v-on:click="closeModal">
                    <span>&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <slot></slot>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" v-on:click="closeModal">
                    {{ closetext }}
                </button>
                <button type="button" class="btn btn-primary" v-on:click="$emit('ok')">
                    {{ oktext }}
                </button>
            </div>
        </div>
    </div>
</div>
`
});