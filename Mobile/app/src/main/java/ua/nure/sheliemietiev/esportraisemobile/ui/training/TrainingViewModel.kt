package ua.nure.sheliemietiev.esportraisemobile.ui.training

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.models.StressFinder
import ua.nure.sheliemietiev.esportraisemobile.data.TeamMember
import ua.nure.sheliemietiev.esportraisemobile.models.TeamModel
import ua.nure.sheliemietiev.esportraisemobile.models.TrainingModel
import ua.nure.sheliemietiev.esportraisemobile.util.iso8601ToDate
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import java.util.*
import javax.inject.Inject

data class StateRecord(
    val userId: Int,
    val heartRate: Int,
    val temperature: Float,
    val date: Date
)

class PlayerStateData(
    val viewedUserId: Int?,
    val stateRecords: Map<Int, Iterable<StateRecord>>,
    val nervousUsersIds: Iterable<Int>
) {
    val viewedUserStates
        get() = if (viewedUserId == null) {
            emptyList()
        } else {
            stateRecords[viewedUserId]
        }
}

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
                    iso8601ToDate(dateFormatted)
                )
            }
            return OperationResult.success(records)
        }
        return OperationResult.error(0)
    }
}

class VideoStreamItem(
    val videoId: String,
    val teamMemberName: String
) {
    override fun toString(): String {
        return teamMemberName
    }
}

class TrainingData(
    val teamMembers: Map<Int, TeamMember>,
    val videoStreams: Iterable<VideoStreamItem>
)

const val CHART_SECONDS_LENGTH = 60 * 3

class TrainingViewModel @Inject constructor(
    private val youTubeModel: YouTubeModel,
    private val physStateCollector: PhysStateCollectingModel,
    private val trainingModel: TrainingModel,
    private val teamModel: TeamModel,
    private val stressFinder: StressFinder
) : ViewModel() {
    private val _youtubeKeyData: LiveData<String>
    val youtubeKeyData get() = _youtubeKeyData

    private val _playerStateData: MutableLiveData<PlayerStateData>
    val playerStateData: LiveData<PlayerStateData> get() = _playerStateData

    private val _trainingData: MutableLiveData<TrainingData>
    val trainingData get() = _trainingData

    private val _broadcastUrl = MutableLiveData<String?>(null)
    val broadcastUrl: LiveData<String?> get() = _broadcastUrl

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
        _trainingData = object : MutableLiveData<TrainingData>() {
            override fun onActive() {
                updateTrainingData()
            }
        }
    }

    fun updatePlayerState() {
        val secondsToRetrieve: Int = CHART_SECONDS_LENGTH

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
                trainingId = trainingIdResult.getOrThrow()
            )
            if (physicalRecords.isFailure) {
                return@launch
            }
            val statesByPlayers = physicalRecords.getOrThrow().groupBy { k ->
                k.userId
            }
            if (statesByPlayers.count() == 0) {
                _playerStateData.value =
                    PlayerStateData(null, emptyMap(), emptyList())
                return@launch
            }

            val viewedUserId = statesByPlayers.keys.first()
            _playerStateData.value = PlayerStateData(
                viewedUserId,
                statesByPlayers,
                statesByPlayers.filter { s -> isNervous(s.value) }.map { s -> s.key }
            )
        }
    }

    private fun isNervous(states: Iterable<StateRecord>): Boolean {
        val abnormalHeartRate = stressFinder.isNervous(states.map { s -> s.heartRate.toFloat() })
        val abnormalTemperature = stressFinder.isNervous(states.map { s -> s.temperature })
        return abnormalHeartRate || abnormalTemperature
    }

    fun viewStateOfUser(userId: Int) {
        val stateData = _playerStateData.value
        if (stateData == null || stateData.viewedUserId == userId) {
            return
        }

        _playerStateData.value = PlayerStateData(
            userId,
            stateData.stateRecords,
            stateData.nervousUsersIds
        )
    }


    fun updateTrainingData() {
        viewModelScope.launch {
            val teamIdResult = teamModel.getTeamId()
            if (teamIdResult.isFailure) {
                return@launch
            }
            val teamId = teamIdResult.getOrThrow()
            val teamMembersResult = teamModel.getTeamMembers(teamId)
            if (teamMembersResult.isFailure) {
                return@launch
            }
            val teamMembers = teamMembersResult.getOrThrow()

            val trainingIdResult = trainingModel.getCurrentTrainingId(teamId)
            if (trainingIdResult.isFailure) {
                return@launch;
            }
            val trainingId = trainingIdResult.getOrThrow()

            val videoStreamsResult = trainingModel.getBroadcasts(trainingId)
            val videoStreams = videoStreamsResult.getOrThrow().map {
                val memberName = teamMembers
                    .find { m -> m.id == it.memberId }!!
                    .userName
                VideoStreamItem(it.streamId, memberName)
            }
            _trainingData.value = TrainingData(
                teamMembers.associateBy { k -> k.id },
                videoStreams
            )
        }
    }

    fun selectedBroadcastChanged(broadcastIndex: Int) {
        val broadcasts = trainingData.value?.videoStreams ?: emptyList()
        if (broadcastIndex < 0 || broadcastIndex >= broadcasts.count()) {
            return
        }
        val selectedBroadcast = broadcasts.elementAt(broadcastIndex)
        _broadcastUrl.value = selectedBroadcast.videoId
    }
}