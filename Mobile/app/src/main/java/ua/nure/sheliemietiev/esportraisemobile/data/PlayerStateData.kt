package ua.nure.sheliemietiev.esportraisemobile.data

class PlayerStateData(
    val viewedUserId: Int?,
    val stateRecords: Map<Int, Iterable<StateRecord>>,
    val nervousUsersIds: Iterable<Int>
) {
    val viewedUserStates
        get() = if (viewedUserId == null) {
            emptyList()
        } else {
            stateRecords[viewedUserId]
        }
}