package ua.nure.sheliemietiev.esportraisemobile.ui.connectIot

import android.Manifest
import android.app.Activity
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothDevice
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.content.pm.PackageManager
import android.os.Bundle
import android.view.View
import android.widget.ListView
import android.widget.ProgressBar
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import ua.nure.sheliemietiev.esportraisemobile.App
import ua.nure.sheliemietiev.esportraisemobile.R
import javax.inject.Inject

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
        connectIotViewModel = ViewModelProvider(
            this,
            viewModelFactory
        )
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
            Toast.makeText(
                this,
                "Bluetooth is not supported!",
                Toast.LENGTH_LONG
            ).show()
            finish()
            return false
        }
        this.bluetoothAdapter = adapter
        return true
    }

    private fun initializeDevicesListView() {
        val foundDevicesList = connectIotViewModel.devicesData.value!!
        val listAdapter =
            DeviceAdapter(
                this,
                foundDevicesList
            )
        devicesList.adapter = listAdapter
        devicesList.setOnItemClickListener { parent, view, position, id ->
            devicesList.isEnabled = false
            val mac = foundDevicesList[position].macAddress
            val device = bluetoothAdapter.getRemoteDevice(mac)
            connectionThread =
                ConnectionProcessorThread(
                    device,
                    connectIotViewModel
                )
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
            val enableBluetoothIntent =
                Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE)
            startActivityForResult(enableBluetoothIntent,
                ENABLE_BLUETOOTH_ACTIVITY_CODE
            )
        }
    }

    private fun initializeConnectionResultMonitoring() {
        connectIotViewModel.deviceStarted.observe(this, Observer {
            if (it != null) {
                val successfulConnect = it
                if (successfulConnect) {
                    Toast.makeText(
                        this,
                        "The device has started training!",
                        Toast.LENGTH_LONG
                    ).show()
                    finish()
                    return@Observer
                } else {
                    Toast.makeText(
                        this,
                        "The connection can't be established!",
                        Toast.LENGTH_LONG
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
                Toast.makeText(
                    this,
                    "You need to enable Bluetooth",
                    Toast.LENGTH_LONG
                ).show()
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
        Toast.makeText(
            this,
            "Location permission isn't granted!",
            Toast.LENGTH_LONG
        ).show()
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
            DeviceInfo(
                it.name,
                it.address
            )
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
                        connectIotViewModel.addDevices(listOf(
                            DeviceInfo(
                                deviceName,
                                mac
                            )
                        ))
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
}