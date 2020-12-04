package ua.nure.sheliemietiev.esportraisemobile.ui.connectIot

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import ua.nure.sheliemietiev.esportraisemobile.R

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
        val deviceNameView = row.findViewById<TextView>(
            R.id.device_name
        )
        deviceNameView.text = devicesList[position].name
        return row
    }

}