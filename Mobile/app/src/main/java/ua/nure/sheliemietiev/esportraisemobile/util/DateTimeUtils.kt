package ua.nure.sheliemietiev.esportraisemobile.util

import android.content.Context
import ua.nure.sheliemietiev.esportraisemobile.R
import java.text.ParseException
import java.text.SimpleDateFormat
import java.util.*

fun iso8601ToDate(formatted: String): Date {
    return try {
        SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", Locale.ENGLISH).parse(formatted)!!
    } catch (e: ParseException) {
        SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.ENGLISH).parse(formatted)!!
    }
}

fun Date.toLocaleTimeString(context: Context): String {
    val timeFormat = context.getString(R.string.locale_time)
    val dateFormat = SimpleDateFormat(timeFormat, Locale.ENGLISH)
    dateFormat.timeZone = TimeZone.getDefault()
    return dateFormat.format(dateFormat.format(this))
}