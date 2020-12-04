package ua.nure.sheliemietiev.esportraisemobile.models

import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.data.VideoStream
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import ua.nure.sheliemietiev.esportraisemobile.util.iso8601ToDate
import java.util.*
import javax.inject.Inject

class TrainingModel @Inject constructor(
    private val api: Api
) {
    suspend fun getCurrentTrainingId(teamId: Int): OperationResult<Int> {
        val response = api.get(
            "training/last",
            mapOf("id" to teamId.toString())
        )
        if (response.statusCode == StatusCode.OK.code) {
            val json = response.asJsonMap()
            val trainingBeginTime = iso8601ToDate(json["beginTime"].asString)

            if (isTrainingIdleTooLong(trainingBeginTime)) {
                return OperationResult.error()
            }

            val trainingId = json["id"].asInt
            return OperationResult.success(
                trainingId
            )
        }
        return OperationResult.error()
    }

    private fun isTrainingIdleTooLong(trainingBeginTime: Date): Boolean {
        val maxIdleMilliseconds = 1000L * 60L * 60L
        return (Date().time - trainingBeginTime.time) >= maxIdleMilliseconds
    }

    suspend fun getBroadcasts(trainingId: Int)
            : OperationResult<Iterable<VideoStream>> {
        val response = api.get(
            "training/broadcast",
            mapOf("id" to trainingId.toString())
        )
        if (response.statusCode == StatusCode.OK.code) {
            val json = response.asJsonMap()
            val streamsArray = json["streams"].asJsonArray
            val videoStreams = streamsArray.map {
                val videoStream = it.asJsonObject
                VideoStream(
                    videoStream["streamId"].asString,
                    videoStream["teamMemberId"].asInt
                )
            }
            return OperationResult.success(videoStreams)
        }
        return OperationResult.error()
    }
}