package ua.nure.sheliemietiev.esportraisemobile.ui.connectIot

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import ua.nure.sheliemietiev.esportraisemobile.api.AuthorizationInfo
import javax.inject.Inject

class ConnectIotViewModel @Inject constructor(
    private val authInfo: AuthorizationInfo
) : ViewModel() {
    private val devicesList: MutableList<DeviceInfo> = arrayListOf()
    private val _devicesData: MutableLiveData<List<DeviceInfo>> =
        MutableLiveData(devicesList)
    val devicesData: LiveData<List<DeviceInfo>> get() = _devicesData

    val messageForSmartDevice: String get() = authInfo.token

    private val _deviceStarted: MutableLiveData<Boolean?> =
        MutableLiveData(null)
    val deviceStarted: LiveData<Boolean?> get() = _deviceStarted

    fun addDevices(devices: Iterable<DeviceInfo>) {
        val notAddedDevices = devices.filter { d ->
            !devicesList.any { saved -> saved.macAddress == d.macAddress }
        }
        devicesList.addAll(notAddedDevices)
        _devicesData.value = _devicesData.value
    }

    fun notifyMessageSent() {
        _deviceStarted.postValue(true)
    }

    fun notifyMessageFail() {
        _deviceStarted.postValue(false)
    }
}