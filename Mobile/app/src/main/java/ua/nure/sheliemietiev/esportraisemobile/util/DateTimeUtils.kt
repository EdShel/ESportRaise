package ua.nure.sheliemietiev.esportraisemobile.util

import android.text.format.DateUtils
import java.text.DateFormat
import java.text.ParseException
import java.text.SimpleDateFormat
import java.time.Instant
import java.time.OffsetDateTime
import java.time.ZonedDateTime
import java.util.*

fun Iso8601ToDate(formatted : String) : Date {
    return try{
        SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'").parse(formatted)!!
    }
    catch(e : ParseException){
        SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'").parse(formatted)!!
    }
}