package ua.nure.sheliemietiev.esportraisemobile.ui.main

import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import javax.inject.Inject

data class TeamMember(
    val id: Int,
    val userName: String
) {
    override fun toString(): String {
        return userName
    }
}

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

    suspend fun getTeamMembers(teamId: Int): OperationResult<Iterable<TeamMember>> {
        val response = api.get("team/full", mapOf("id" to teamId.toString()))
        if (response.statusCode == StatusCode.OK.code) {
            val json = response.asJsonMap()
            val membersJson = json["members"].asJsonArray
            val members = membersJson.map {
                val memberJson = it.asJsonObject
                TeamMember(
                    memberJson["id"].asInt,
                    memberJson["name"].asString)
            }
            return OperationResult.success(members)
        }
        return OperationResult.error()
    }
}