package ua.nure.sheliemietiev.esportraisemobile.ui.main

import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import javax.inject.Inject


data class VideoStream(
    val streamId: String,
    val memberId: Int
)

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
            val trainingId = json["id"].asInt
            return OperationResult.success(
                trainingId
            )
        }
        return OperationResult.error()
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
            val videoStreams = streamsArray.map{
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