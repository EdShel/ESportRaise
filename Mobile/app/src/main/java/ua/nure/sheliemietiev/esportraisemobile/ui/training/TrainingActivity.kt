package ua.nure.sheliemietiev.esportraisemobile.ui.training

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.util.AttributeSet
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
import java.text.SimpleDateFormat
import javax.inject.Inject


class TrainingActivity : AppCompatActivity() {

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    lateinit var trainingViewModel: TrainingViewModel

    private lateinit var physStateRefreshHandler: Handler

    private var youtubePlayer: YouTubePlayer? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        (applicationContext as App).components.inject(this)
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_training)
        trainingViewModel = ViewModelProvider(this, viewModelFactory)
            .get(TrainingViewModel::class.java)

        setupYouTubePlayer()

        val lineChart = findViewById<LineChart>(R.id.physStateChart)

        val descr = Description()
        descr.text = "Physical state"
        lineChart.description = descr

        lineChart.animateX(2000)
        lineChart.invalidate()

        trainingViewModel.playerStateData.observe(this, Observer { stateData ->
            val trainingData = trainingViewModel.trainingData.value
            if (trainingData != null) {
                updateStressIcons(trainingData, stateData)
            }

            val viewedStateData = stateData.viewedUserStates

            if (viewedStateData == null || viewedStateData.count() == 0) {
                return@Observer
            }

            val baseDate = viewedStateData.first().date.time

            val hrDataSet = LineDataSet(viewedStateData.map {
                Entry(
                    ((it.date.time - baseDate) / 1000).toFloat(),
                    it.heartRate.toFloat()
                )
            }, "HR")
            hrDataSet.axisDependency = YAxis.AxisDependency.LEFT
            hrDataSet.color =
                ContextCompat.getColor(
                    this,
                    R.color.heartRate
                ) // TODO: replace ints with constants from styles
            val temperatureDataSet = LineDataSet(viewedStateData.map {
                Entry(
                    ((it.date.time - baseDate) / 1000).toFloat(),
                    it.temperature
                )
            }, "Temperature")
            temperatureDataSet.axisDependency = YAxis.AxisDependency.RIGHT
            temperatureDataSet.color =
                ContextCompat.getColor(
                    this,
                    R.color.temperature
                ) // TODO: replace ints with constants from styles
            lineChart.data = LineData(hrDataSet, temperatureDataSet)
            val xAxis = lineChart.xAxis
            xAxis.granularity = 10f
            xAxis.valueFormatter = object : ValueFormatter() {
                override fun getFormattedValue(value: Float): String {
                    val time = baseDate + (value * 1000).toLong()
                    return SimpleDateFormat("HH:mm:ss")
                        .format(time) //TODO: take the  format from from R.string
                }
            }
            lineChart.invalidate()
        })

        val memberSelector = findViewById<Spinner>(R.id.memberSelectSpinner)
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

        trainingViewModel.broadcastUrl.observe(this, Observer {
            if (youtubePlayer == null || it == null) {
                return@Observer
            }
            youtubePlayer!!.loadVideo(it)
            youtubePlayer!!.play()
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
            val allTeamMembers = trainingData.teamMembers
            playersContainer.removeAllViews()
            for (playerId in activePlayersIds) {
                val playerButton = Button(this@TrainingActivity)
                playerButton.text = allTeamMembers[playerId]!!.userName
                playerButton.tag = playerId
                playersContainer.addView(playerButton)
            }
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
        trainingViewModel.youtubeKeyData.observe(this@TrainingActivity, Observer<String>() {
            if (it == "") {
                Toast.makeText(
                    this@TrainingActivity,
                    "Unable to retrieve YouTube API key",
                    Toast.LENGTH_LONG
                ).show()
            } else {
                val playerFragment = fragmentManager
                    .findFragmentById(R.id.trainingVideo) as YouTubePlayerFragment
                playerFragment.initialize(it, object : YouTubePlayer.OnInitializedListener {
                    override fun onInitializationSuccess(
                        provider: YouTubePlayer.Provider?,
                        player: YouTubePlayer?,
                        hz: Boolean
                    ) {
                        if (player == null) {
                            return;
                        }
                        youtubePlayer = player
                        if (trainingViewModel.broadcastUrl.value != null) {
                            player.loadVideo(trainingViewModel.broadcastUrl.value);
                        }
//                        player.pause()
//                        player.play();
                    }

                    override fun onInitializationFailure(
                        p0: YouTubePlayer.Provider?,
                        p1: YouTubeInitializationResult?
                    ) {
                        Toast.makeText(
                            this@TrainingActivity,
                            "Error while initializing player",
                            Toast.LENGTH_LONG
                        ).show();
                    }
                })
            }
        })
    }
}