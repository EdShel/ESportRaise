package ua.nure.sheliemietiev.esportraisemobile.data

class VideoStreamItem(
    val videoId: String,
    val teamMemberName: String
) {
    override fun toString(): String {
        return teamMemberName
    }
}