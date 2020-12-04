package ua.nure.sheliemietiev.esportraisemobile.ui.training

import android.os.Build
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.View
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import androidx.core.view.children
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import com.github.mikephil.charting.charts.LineChart
import com.github.mikephil.charting.components.Description
import com.github.mikephil.charting.components.YAxis
import com.github.mikephil.charting.data.Entry
import com.github.mikephil.charting.data.LineData
import com.github.mikephil.charting.data.LineDataSet
import com.github.mikephil.charting.formatter.ValueFormatter
import com.google.android.youtube.player.YouTubeInitializationResult
import com.google.android.youtube.player.YouTubePlayer
import com.google.android.youtube.player.YouTubePlayerFragment
import ua.nure.sheliemietiev.esportraisemobile.App
import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.data.PlayerStateData
import ua.nure.sheliemietiev.esportraisemobile.data.StateRecord
import ua.nure.sheliemietiev.esportraisemobile.data.TrainingData
import ua.nure.sheliemietiev.esportraisemobile.util.toLocaleTimeString
import java.lang.Exception
import java.util.*
import javax.inject.Inject

class TrainingActivity : AppCompatActivity() {

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    lateinit var trainingViewModel: TrainingViewModel

    private lateinit var lineChart: LineChart

    private lateinit var physStateRefreshHandler: Handler

    private lateinit var memberSelector: Spinner

    private var youtubePlayer: YouTubePlayer? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        (applicationContext as App).components.inject(this)
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_training)
        trainingViewModel = ViewModelProvider(this, viewModelFactory)
            .get(TrainingViewModel::class.java)

        setupYouTubePlayer()

        lineChart = findViewById(R.id.physStateChart)
        memberSelector = findViewById(R.id.memberSelectSpinner)

        setupPhysicalStateChart()

        observePhysicalStateDataCame()

        observeVideoStreamerChanged()

        observeTrainingInfoChanged()

        observeVideoUrlChanged()
    }

    private fun setupPhysicalStateChart() {
        val chartDescription = Description()
        chartDescription.text = getString(R.string.phys_state_chart_name)
        lineChart.description = chartDescription
        lineChart.animateX(2000)
        lineChart.setNoDataText(getString(R.string.no_recent_data))
        lineChart.invalidate()
    }

    private fun observePhysicalStateDataCame() {
        trainingViewModel.playerStateData.observe(this, Observer { stateData ->
            val trainingData = trainingViewModel.trainingData.value
            if (trainingData != null) {
                updateStressIcons(trainingData, stateData)
            }

            val currentlyViewedState = stateData.viewedUserStates
            if (currentlyViewedState == null || currentlyViewedState.count() == 0) {
                return@Observer
            }

            val baseDate = currentlyViewedState.first().date.time
            val hrDataSet = getHeartRateDataSet(currentlyViewedState, baseDate)
            val temperatureDataSet = getTemperatureDataSet(currentlyViewedState, baseDate)
            lineChart.data = LineData(hrDataSet, temperatureDataSet)

            setupChartXAxisLabels(baseDate)

            lineChart.invalidate()
        })
    }

    private fun getHeartRateDataSet(
        currentlyViewedState: Iterable<StateRecord>,
        baseDate: Long
    ): LineDataSet {
        val hrDataSet = LineDataSet(currentlyViewedState.map {
            Entry(
                ((it.date.time - baseDate) / 1000).toFloat(),
                it.heartRate.toFloat()
            )
        }, getString(R.string.heart_rate))
        hrDataSet.axisDependency = YAxis.AxisDependency.LEFT
        hrDataSet.color =
            ContextCompat.getColor(
                this,
                R.color.heartRate
            )
        hrDataSet.setDrawValues(false)
        return hrDataSet
    }

    private fun getTemperatureDataSet(
        currentlyViewedState: Iterable<StateRecord>,
        baseDate: Long
    ): LineDataSet {
        val temperatureData = getLocaleDependantTemperatureData(currentlyViewedState, baseDate)
        val temperatureDataSet = LineDataSet(temperatureData, getString(R.string.temperature))
        temperatureDataSet.axisDependency = YAxis.AxisDependency.RIGHT
        temperatureDataSet.color = ContextCompat.getColor(
            this,
            R.color.temperature
        )
        temperatureDataSet.setDrawValues(false)
        return temperatureDataSet
    }

    private fun getLocaleDependantTemperatureData(
        currentlyViewedState: Iterable<StateRecord>,
        baseDate: Long
    ): List<Entry> {
        val culture = if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            resources.configuration.locales[0].language
        } else {
            resources.configuration.locale.language
        }
        val isFahrenheit = culture == "en"
        return if (isFahrenheit) {
            currentlyViewedState.map {
                Entry(
                    ((it.date.time - baseDate) / 1000).toFloat(),
                    (it.temperature * 1.8F) + 32F
                )
            }
        } else {
            currentlyViewedState.map {
                Entry(
                    ((it.date.time - baseDate) / 1000).toFloat(),
                    it.temperature
                )
            }
        }
    }

    private fun setupChartXAxisLabels(baseDate: Long) {
        val xAxis = lineChart.xAxis
        xAxis.granularity = 10f
        xAxis.valueFormatter = object : ValueFormatter() {
            override fun getFormattedValue(value: Float): String {
                val time = baseDate + (value * 1000).toLong()
                return Date(time).toLocaleTimeString(this@TrainingActivity)
            }
        }
    }

    private fun observeVideoStreamerChanged() {
        memberSelector.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(
                adapter: AdapterView<*>?,
                spinner: View?,
                index: Int,
                id: Long
            ) {
                trainingViewModel.selectedBroadcastChanged(index)
            }

            override fun onNothingSelected(p0: AdapterView<*>?) {}
        }
    }

    private fun observeTrainingInfoChanged() {
        trainingViewModel.trainingData.observe(this, Observer {
            memberSelector.adapter = ArrayAdapter(
                this@TrainingActivity,
                R.layout.support_simple_spinner_dropdown_item,
                it.videoStreams.toList()
            )

            val stateRecordsData = trainingViewModel.playerStateData.value
            if (stateRecordsData != null)
                updateStressIcons(it, stateRecordsData)
        })
    }

    private fun observeVideoUrlChanged() {
        trainingViewModel.broadcastUrl.observe(this, Observer { videoId ->
            val player = youtubePlayer
            if (player == null || videoId == null) {
                return@Observer
            }
            player.loadVideo(videoId)
            player.play()
        })
    }

    private fun updateStressIcons(
        trainingData: TrainingData,
        stateData: PlayerStateData
    ) {
        val playersContainer = findViewById<LinearLayout>(R.id.activePlayersList)

        val playerButtons = playersContainer.children
        val displayedPlayersIds = playerButtons.map { b -> b.tag as Int }.toSet()
        val activePlayersIds = stateData.stateRecords.keys
        if (displayedPlayersIds != activePlayersIds) {
            recreateStressIcons(trainingData, playersContainer, activePlayersIds)
        }

        for (button in playerButtons) {
            val playerId: Int = button.tag as Int
            button.setBackgroundColor(
                ContextCompat.getColor(
                    this, if (stateData.nervousUsersIds.contains(playerId)) {
                        R.color.colorAccent
                    } else {
                        R.color.design_default_color_surface
                    }
                )
            )
        }
    }

    private fun recreateStressIcons(
        trainingData: TrainingData,
        playersContainer: LinearLayout,
        activePlayersIds: Set<Int>
    ) {
        val allTeamMembers = trainingData.teamMembers
        playersContainer.removeAllViews()
        for (playerId in activePlayersIds) {
            val playerButton = Button(this@TrainingActivity)
            playerButton.text = allTeamMembers[playerId]!!.userName
            playerButton.tag = playerId
            playerButton.setOnClickListener { trainingViewModel.viewStateOfUser(playerId) }
            playersContainer.addView(playerButton)
        }
    }

    override fun onResume() {
        super.onResume()
        val periodMilliseconds = 10 * 1000L
        physStateRefreshHandler = Handler(Looper.getMainLooper())
        physStateRefreshHandler.post(object : Runnable {
            override fun run() {
                trainingViewModel.updatePlayerState()
                physStateRefreshHandler.postDelayed(this, periodMilliseconds)
            }
        })
    }

    override fun onPause() {
        super.onPause()
        physStateRefreshHandler.removeCallbacksAndMessages(null)
    }

    private fun TrainingActivity.setupYouTubePlayer() {
        trainingViewModel.youtubeKeyData.observe(
            this@TrainingActivity,
            Observer<String> { apiKey ->
                if (apiKey == "") {
                    Toast.makeText(
                        this@TrainingActivity,
                        getString(R.string.cant_retrieve_api_key),
                        Toast.LENGTH_LONG
                    ).show()
                } else {
                    try {
                        initPlayerWithApiKey(apiKey)
                    } catch (e: Exception) {
                        onPlayerInitFailed()
                    }
                }
            })
    }

    private fun initPlayerWithApiKey(apiKey: String) {
        val playerFragment = fragmentManager
            .findFragmentById(R.id.trainingVideo) as YouTubePlayerFragment
        playerFragment.initialize(apiKey, object : YouTubePlayer.OnInitializedListener {
            override fun onInitializationSuccess(
                provider: YouTubePlayer.Provider?,
                player: YouTubePlayer?,
                b: Boolean
            ) {
                if (player == null) {
                    return;
                }
                youtubePlayer = player
                if (trainingViewModel.broadcastUrl.value != null) {
                    player.loadVideo(trainingViewModel.broadcastUrl.value);
                }
            }

            override fun onInitializationFailure(
                provider: YouTubePlayer.Provider?,
                result: YouTubeInitializationResult?
            ) {
                onPlayerInitFailed()
            }
        })
    }

    private fun onPlayerInitFailed() {
        Toast.makeText(
            this@TrainingActivity,
            getString(R.string.player_init_error),
            Toast.LENGTH_LONG
        ).show()
    }
}