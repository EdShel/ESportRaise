package ua.nure.sheliemietiev.esportraisemobile.ui.connectIot

import android.bluetooth.BluetoothDevice
import android.bluetooth.BluetoothSocket
import ua.nure.sheliemietiev.esportraisemobile.BuildConfig
import java.io.IOException
import java.io.OutputStream
import java.util.*

class ConnectionProcessorThread(
    private val device: BluetoothDevice,
    private val connectIotViewModel: ConnectIotViewModel
) : Thread() {

    private val uuid = BuildConfig.APP_UUID

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
