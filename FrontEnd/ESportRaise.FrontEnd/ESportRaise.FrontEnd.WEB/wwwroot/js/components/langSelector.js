Vue.component('lang-selector', {
    computed: {
        language: {
            get() {
                return getLanguage();
            },
            set(val) {
                document.getElementById('langToSet').value = val;
                document.getElementById('changeLang').submit();
            }
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
<div>
    <i class="fas fa-globe"></i>
    <select v-model="language">
        <slot></slot>
    </select>
</div>
`
});