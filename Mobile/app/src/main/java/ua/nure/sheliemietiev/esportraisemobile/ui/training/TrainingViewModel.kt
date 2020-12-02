package ua.nure.sheliemietiev.esportraisemobile.ui.training

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.ui.main.TeamModel
import ua.nure.sheliemietiev.esportraisemobile.ui.main.TrainingModel
import ua.nure.sheliemietiev.esportraisemobile.util.Iso8601ToDate
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import java.util.*
import javax.inject.Inject

data class StateRecord(
    val heartrate: Int,
    val temperature: Float,
    val date: Date
)

class PlayerStateData(
    val userId: Int,
    val updateTime: Date,
    val stateRecords: List<StateRecord>
)

class PhysStateCollectingModel @Inject constructor(
    private val api: Api
) {
    suspend fun getPhysStateForLastSeconds(
        seconds: Int,
        trainingId: Int,
        userId: Int
    ): OperationResult<List<StateRecord>> {
        val response = api.get(
            "stateRecord/last",
            mapOf(
                "trainingId" to trainingId.toString(),
                "timeInSecs" to seconds.toString(),
                "userId" to userId.toString()
            )
        )
        if (response.statusCode == StatusCode.OK.code) {
            val json = response.asJsonMap()
            val records = json["records"].asJsonArray.map {
                val stateJson = it.asJsonObject
                val heartrate = stateJson["heartrate"].asInt
                val temperature = stateJson["temperature"].asFloat
                val dateFormatted = stateJson["createTime"].asString
                StateRecord(
                    heartrate,
                    temperature,
                    Iso8601ToDate(dateFormatted)
                )
            }
            return OperationResult.success(records)
        }
        return OperationResult.error(0)
    }
}

const val CHART_SECONDS_LENGTH = 30

class TrainingViewModel @Inject constructor(
    private val youTubeModel: YouTubeModel,
    private val physStateCollector: PhysStateCollectingModel,
    private val trainingModel: TrainingModel,
    private val teamModel: TeamModel
) : ViewModel() {
    private val _youtubeKeyData: LiveData<String>;
    val youtubeKeyData get() = _youtubeKeyData

    private val _playerStateData: MutableLiveData<PlayerStateData>
    val playerStateData: LiveData<PlayerStateData>
        get() = _playerStateData

    init {
        _youtubeKeyData = object : LiveData<String>() {
            override fun onActive() {
                viewModelScope.launch {
                    value = youTubeModel.getYoutubePlayerKey()
                }
            }
        }
        _playerStateData = object : MutableLiveData<PlayerStateData>() {
            override fun onActive() {
                updatePlayerState()
            }
        }
    }

    fun updatePlayerState() {
        val secondsToRetrieve: Int = if (_playerStateData.value != null) {
            val currentTimeMs = Date().time
            val lastUpdateMs = _playerStateData.value!!.updateTime.time
            ((currentTimeMs - lastUpdateMs) / 1000).toInt()
        } else {
            CHART_SECONDS_LENGTH
        }

        viewModelScope.launch {
            val teamIdResult = teamModel.getTeamId()
            if (teamIdResult.isFailure) {
                return@launch
            }
            val teamId = teamIdResult.getOrThrow()

            val trainingIdResult = trainingModel.getCurrentTrainingId(teamId)
            if (trainingIdResult.isFailure) {
                return@launch;
            }

            val physicalRecords = physStateCollector.getPhysStateForLastSeconds(
                seconds = secondsToRetrieve,
                trainingId = trainingIdResult.getOrThrow(),
                userId = 4
            )
            if (physicalRecords.isFailure) {
                return@launch
            }
            _playerStateData.value = PlayerStateData(
                4,
                Date(),
                physicalRecords.getOrThrow()
            )
        }
    }
}