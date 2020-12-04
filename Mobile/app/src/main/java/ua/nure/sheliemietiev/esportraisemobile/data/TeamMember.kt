package ua.nure.sheliemietiev.esportraisemobile.data

data class TeamMember(
    val id: Int,
    val userName: String
) {
    override fun toString(): String {
        return userName
    }
}