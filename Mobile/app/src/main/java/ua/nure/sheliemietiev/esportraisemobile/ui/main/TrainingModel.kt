package ua.nure.sheliemietiev.esportraisemobile.ui.main

import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
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
            val trainingId = json["id"].asInt
            return OperationResult.success(
                trainingId
            )
        }
        return OperationResult.error()
    }
}