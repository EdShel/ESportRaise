package ua.nure.sheliemietiev.esportraisemobile.models

import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.data.StateRecord
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import ua.nure.sheliemietiev.esportraisemobile.util.iso8601ToDate
import javax.inject.Inject

class PhysStateCollectingModel @Inject constructor(
    private val api: Api
) {
    suspend fun getPhysStateForLastSeconds(
        seconds: Int,
        trainingId: Int
    ): OperationResult<List<StateRecord>> {
        val response = api.get(
            "stateRecord/last",
            mapOf(
                "trainingId" to trainingId.toString(),
                "timeInSecs" to seconds.toString()
            )
        )
        if (response.statusCode == StatusCode.OK.code) {
            val json = response.asJsonMap()
            val records = json["records"].asJsonArray.map {
                val stateJson = it.asJsonObject
                val userId = stateJson["teamMemberId"].asInt
                val heartRate = stateJson["heartRate"].asInt
                val temperature = stateJson["temperature"].asFloat
                val dateFormatted = stateJson["createTime"].asString
                StateRecord(
                    userId,
                    heartRate,
                    temperature,
                    iso8601ToDate(
                        dateFormatted
                    )
                )
            }
            return OperationResult.success(
                records
            )
        }
        return OperationResult.error(
            0
        )
    }
}