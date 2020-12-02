package ua.nure.sheliemietiev.esportraisemobile.util

import java.text.DateFormat
import java.util.*

fun Iso8601ToDate(formatted : String) : Date {
    return DateFormat.getDateTimeInstance().parse(formatted)!!
//    return SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'")
//        .parse(formatted)!!
}