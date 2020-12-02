package ua.nure.sheliemietiev.esportraisemobile.ui.main

import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import javax.inject.Inject

class TeamModel @Inject constructor(
    private val api: Api
) {
    suspend fun getTeamId(): OperationResult<Int> {
        val response = api.get("teamMember/me", null);
        if (response.statusCode == 200) {
            val json = response.asJsonMap()
            val teamId = json["teamId"].asInt
            return OperationResult.success(
                teamId
            )
        }
        return OperationResult.error(
            R.string.have_no_team
        )
    }
}