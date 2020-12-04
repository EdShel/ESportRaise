package ua.nure.sheliemietiev.esportraisemobile.ui.connectIot

data class DeviceInfo(
    val name: String,
    val macAddress: String
) {
    override fun toString(): String {
        return name
    }
}