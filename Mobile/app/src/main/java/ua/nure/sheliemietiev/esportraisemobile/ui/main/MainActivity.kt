package ua.nure.sheliemietiev.esportraisemobile.ui.main

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import ua.nure.sheliemietiev.esportraisemobile.App
import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.ui.training.TrainingActivity
import ua.nure.sheliemietiev.esportraisemobile.ui.training.TrainingViewModel
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
            return OperationResult.success(teamId)
        }
        return OperationResult.error(R.string.have_no_team)
    }
}

class TrainingCheckModel @Inject constructor(
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
            return OperationResult.success(trainingId)
        }
        return OperationResult.error()
    }
}

class TeamMemberStatus(
    val teamId: Int?,
    val currentTrainingId: Int?
)

class MainViewModel @Inject constructor(
    private val teamModel: TeamModel,
    private val trainingCheckModel: TrainingCheckModel
) : ViewModel() {
    var teamMemberData: LiveData<TeamMemberStatus>
        get() = field

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

                            val trainingResult = trainingCheckModel
                                .getCurrentTrainingId(teamId)
                            if (trainingResult.isSuccess) {
                                trainingId = trainingResult.getOrThrow()
                            }
                        }
                    } finally {
                        value = TeamMemberStatus(teamId, trainingId)
                    }
                }
            }
        }
    }
}

class MainActivity : AppCompatActivity() {

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    private lateinit var mainViewModel: MainViewModel

    private lateinit var viewTrainingButton: Button

    private lateinit var startTrainingButton: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        (applicationContext as App).components.inject(this)
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        mainViewModel = ViewModelProvider(this, viewModelFactory)
            .get(MainViewModel::class.java)

        viewTrainingButton = findViewById(R.id.viewCurrentTrainingButton)
        startTrainingButton = findViewById(R.id.startTrainingButton)

        viewTrainingButton.setOnClickListener {
            val trainingActivity = Intent(this, TrainingActivity::class.java)
            startActivity(trainingActivity)
        }

        mainViewModel.teamMemberData.observe(this, Observer {
            if (it.teamId == null) {
                viewTrainingButton.isEnabled = false
                startTrainingButton.isEnabled = false
            } else if (it.currentTrainingId == null) {
                viewTrainingButton.isEnabled = false
                startTrainingButton.isEnabled = true
            } else {
                viewTrainingButton.isEnabled = true
                startTrainingButton.isEnabled = true
            }
        })
    }
}