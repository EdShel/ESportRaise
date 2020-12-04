package ua.nure.sheliemietiev.esportraisemobile.ui.main

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import ua.nure.sheliemietiev.esportraisemobile.api.AuthorizationInfo
import ua.nure.sheliemietiev.esportraisemobile.models.TeamModel
import ua.nure.sheliemietiev.esportraisemobile.models.TrainingModel
import java.lang.Exception
import javax.inject.Inject

class MainViewModel @Inject constructor(
    private val teamModel: TeamModel,
    private val trainingModel: TrainingModel,
    private val authorizationInfo: AuthorizationInfo
) : ViewModel() {
    val teamMemberData: LiveData<TeamMemberStatus>

    init {
        teamMemberData = object : LiveData<TeamMemberStatus>() {
            override fun onActive() {
                viewModelScope.launch {
                    var teamId: Int? = null
                    var trainingId: Int? = null

                    try {
                        val teamIdResult = teamModel.getTeamId()
                        if (teamIdResult.isSuccess) {
                            teamId = teamIdResult.getOrThrow()

                            val trainingResult = trainingModel
                                .getCurrentTrainingId(teamId)
                            if (trainingResult.isSuccess) {
                                trainingId = trainingResult.getOrThrow()
                            }
                        }
                    } catch (e: Exception) {
                        teamId = null
                        trainingId = null
                    } finally {
                        value = TeamMemberStatus(teamId, trainingId)
                    }
                }
            }
        }
    }

    fun clearAuthorizationInfo() {
        authorizationInfo.deauthorize()
    }
}