package ua.nure.sheliemietiev.esportraisemobile.ui.connectIot

import android.Manifest
import android.app.Activity
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothDevice
import android.bluetooth.BluetoothSocket
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.content.pm.PackageManager
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import ua.nure.sheliemietiev.esportraisemobile.App
import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.api.AuthorizationInfo
import java.io.IOException
import java.io.OutputStream
import java.util.*
import javax.inject.Inject

data class DeviceInfo(
    val name: String,
    val macAddress: String
) {
    override fun toString(): String {
        return name
    }
}

class DeviceAdapter(
    private val context: Context,
    private val devicesList: List<DeviceInfo>
) : BaseAdapter() {

    private val inflater: LayoutInflater

    init {
        inflater = context.getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater
    }

    override fun getCount(): Int {
        return devicesList.count()
    }

    override fun getItem(position: Int): Any {
        return devicesList[position]
    }

    override fun getItemId(position: Int): Long {
        return position.toLong()
    }

    override fun getView(position: Int, convertView: View?, parent: ViewGroup?): View {
        val row = convertView ?: inflater.inflate(
            R.layout.bluetooth_device_row, parent, false
        )
        val deviceNameView = row.findViewById<TextView>(R.id.device_name)
        deviceNameView.text = devicesList[position].name
        return row
    }

}

class ConnectIotViewModel @Inject constructor(
    private val authInfo: AuthorizationInfo
) : ViewModel() {
    private val devicesList: MutableList<DeviceInfo> = arrayListOf()
    private val _devicesData: MutableLiveData<List<DeviceInfo>> = MutableLiveData(devicesList)
    val devicesData: LiveData<List<DeviceInfo>> get() = _devicesData

    val messageForSmartDevice: String get() = authInfo.token

    private val _deviceStarted: MutableLiveData<Boolean?> = MutableLiveData(null)
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

private class ConnectionProcessorThread(
    private val device: BluetoothDevice,
    private val connectIotViewModel: ConnectIotViewModel
) : Thread() {

    private val uuid = "00001101-0000-1000-8000-00805F9B34FB" // TODO: to build config

    private var socket = device.createInsecureRfcommSocketToServiceRecord(UUID.fromString(uuid))

    override fun run() {
        try {
            socket?.use { socket ->
                connect()
                val outputStream: OutputStream = socket.outputStream
                val whatToSend = connectIotViewModel.messageForSmartDevice
                outputStream.write(whatToSend.toByteArray())
                connectIotViewModel.notifyMessageSent()
            }
        } catch (e: IOException) {
            connectIotViewModel.notifyMessageFail()
            cancel()
        }
    }

    private fun connect() {
        try {
            socket.connect()
        } catch (e: IOException) {
            val clazz: Class<*> = socket.getRemoteDevice().javaClass
            val paramTypes =
                arrayOf<Class<*>>(Integer.TYPE)
            val m = clazz.getMethod("createRfcommSocket", *paramTypes)
            val params = arrayOf<Any>(Integer.valueOf(1))
            socket = m.invoke(socket.getRemoteDevice(), *params) as BluetoothSocket
            socket.connect()
        }
    }

    fun cancel() {
        val connectedSocket = socket
        if (connectedSocket != null && connectedSocket.isConnected) {
            connectedSocket.close()
        }
    }
}

private const val ENABLE_BLUETOOTH_ACTIVITY_CODE = 1
private const val ENABLE_LOCATION_PERMISSION_CODE = 2

class ConnectIotActivity : AppCompatActivity() {

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    private lateinit var connectIotViewModel: ConnectIotViewModel

    private lateinit var bluetoothAdapter: BluetoothAdapter

    private lateinit var devicesList: ListView

    private lateinit var loadingBar: ProgressBar

    private var broadcastReceiver: BroadcastReceiver? = null

    private var connectionThread: ConnectionProcessorThread? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        (applicationContext as App).components.inject(this)
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_connect_iot)
        connectIotViewModel = ViewModelProvider(this, viewModelFactory)
            .get(ConnectIotViewModel::class.java)

        devicesList = findViewById(R.id.devices_list)
        loadingBar = findViewById(R.id.loading)

        if (!ensureBluetoothAdapterInitialized()) {
            return
        }

        initializeDevicesListView()

        askForPermissionsAndShowPairedDevices()

        initializeConnectionResultMonitoring()
    }

    private fun ensureBluetoothAdapterInitialized(): Boolean {
        val adapter = BluetoothAdapter.getDefaultAdapter()
        if (adapter == null) {
            Toast.makeText(this, "Bluetooth is not supported!", Toast.LENGTH_LONG).show()
            finish()
            return false
        }
        this.bluetoothAdapter = adapter
        return true
    }

    private fun initializeDevicesListView() {
        val foundDevicesList = connectIotViewModel.devicesData.value!!
        val listAdapter = DeviceAdapter(this, foundDevicesList)
        devicesList.adapter = listAdapter
        devicesList.setOnItemClickListener { parent, view, position, id ->
            devicesList.isEnabled = false
            val mac = foundDevicesList[position].macAddress
            val device = bluetoothAdapter.getRemoteDevice(mac)
            connectionThread = ConnectionProcessorThread(device, connectIotViewModel)
            connectionThread?.start()
        }
        connectIotViewModel.devicesData.observe(this, Observer {
            listAdapter.notifyDataSetChanged()
        })
    }

    private fun askForPermissionsAndShowPairedDevices() {
        if (bluetoothAdapter.isEnabled) {
            bluetoothGranted()
        } else {
            val enableBluetoothIntent = Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE)
            startActivityForResult(enableBluetoothIntent, ENABLE_BLUETOOTH_ACTIVITY_CODE)
        }
    }

    private fun initializeConnectionResultMonitoring() {
        connectIotViewModel.deviceStarted.observe(this, Observer {
            if (it != null) {
                val successfulConnect = it
                if (successfulConnect) {
                    Toast.makeText(
                        this, "The device has started training!", Toast.LENGTH_LONG
                    ).show()
                    finish()
                    return@Observer
                } else {
                    Toast.makeText(
                        this, "The connection can't be established!", Toast.LENGTH_LONG
                    ).show()
                    devicesList.isEnabled = true
                }
            }
        })
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == ENABLE_BLUETOOTH_ACTIVITY_CODE) {
            if (resultCode == Activity.RESULT_OK) {
                bluetoothGranted()
            } else {
                Toast.makeText(this, "You need to enable Bluetooth", Toast.LENGTH_LONG).show()
                finish()
            }
        }
    }

    private fun bluetoothGranted() {
        val hasAllPermissions = ensureAllPermissionsGrantedOrAskThem()
        if (hasAllPermissions) {
            startSearchingDevices()
        }
    }

    private fun permissionsGranted() {
        startSearchingDevices()
    }

    private fun permissionsDenied() {
        Toast.makeText(this, "Location permission isn't granted!", Toast.LENGTH_LONG).show()
        finish()
    }

    private fun ensureAllPermissionsGrantedOrAskThem(): Boolean {
        if (ContextCompat.checkSelfPermission(
                this,
                Manifest.permission.ACCESS_FINE_LOCATION
            ) != PackageManager.PERMISSION_GRANTED
        ) {
            if (ActivityCompat.shouldShowRequestPermissionRationale(
                    this,
                    Manifest.permission.ACCESS_FINE_LOCATION
                )
            ) {
                ActivityCompat.requestPermissions(
                    this,
                    arrayOf(Manifest.permission.ACCESS_FINE_LOCATION),
                    ENABLE_LOCATION_PERMISSION_CODE
                );
            } else {
                permissionsDenied()
            }
            return false
        }
        return true
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>,
        grantResults: IntArray
    ) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        when (requestCode) {
            ENABLE_LOCATION_PERMISSION_CODE -> {
                if (grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    permissionsGranted()
                } else {
                    permissionsDenied()
                }
            }
        }
    }

    private fun startSearchingDevices() {
        addPairedDevices()
        startDeviceDiscovering()
    }

    private fun addPairedDevices() {
        val boundedDevices = bluetoothAdapter.bondedDevices.map {
            DeviceInfo(it.name, it.address)
        }
        connectIotViewModel.addDevices(boundedDevices)
    }

    private fun startDeviceDiscovering() {
        val filter = IntentFilter()
        filter.addAction(BluetoothDevice.ACTION_FOUND)
        filter.addAction(BluetoothAdapter.ACTION_DISCOVERY_STARTED)
        filter.addAction(BluetoothAdapter.ACTION_DISCOVERY_FINISHED)
        broadcastReceiver = object : BroadcastReceiver() {
            override fun onReceive(context: Context, intent: Intent) {
                when (intent.action!!) {
                    BluetoothDevice.ACTION_FOUND -> {
                        val device: BluetoothDevice =
                            intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE)!!
                        val deviceName = device.name
                        val mac = device.address
                        connectIotViewModel.addDevices(listOf(DeviceInfo(deviceName, mac)))
                    }
                    BluetoothAdapter.ACTION_DISCOVERY_STARTED -> {
                        showLoading()
                    }
                    BluetoothAdapter.ACTION_DISCOVERY_FINISHED -> {
                        hideLoading()
                    }
                }
            }
        }
        registerReceiver(broadcastReceiver, filter)
        bluetoothAdapter.startDiscovery()
    }

    override fun onDestroy() {
        super.onDestroy()
        stopDeviceDiscovering()
    }

    private fun stopDeviceDiscovering() {
        hideLoading()
        if (broadcastReceiver != null) {
            unregisterReceiver(broadcastReceiver)
            bluetoothAdapter.cancelDiscovery()
        }
    }

    private fun showLoading() {
        loadingBar.visibility = View.VISIBLE
    }

    private fun hideLoading() {
        loadingBar.visibility = View.INVISIBLE
    }

    fun askLocationPermission(): Boolean {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION)
            != PackageManager.PERMISSION_GRANTED
        ) {
            if (ActivityCompat.shouldShowRequestPermissionRationale(
                    this,
                    Manifest.permission.ACCESS_COARSE_LOCATION
                )
            ) {
                Toast.makeText(
                    this,
                    "Permission must be granted to use the app.",
                    Toast.LENGTH_SHORT
                ).show();
            } else {
                ActivityCompat.requestPermissions(
                    this,
                    arrayOf<String>(Manifest.permission.ACCESS_FINE_LOCATION),
                    ENABLE_LOCATION_PERMISSION_CODE
                );
            }
            return false
        } else {
            return true
        }
    }
}