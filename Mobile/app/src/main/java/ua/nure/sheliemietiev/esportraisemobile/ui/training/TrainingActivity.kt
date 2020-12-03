package ua.nure.sheliemietiev.esportraisemobile.ui.training

import android.content.Context
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.View
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.Spinner
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import androidx.core.os.postDelayed
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import com.github.mikephil.charting.charts.CombinedChart
import com.github.mikephil.charting.charts.LineChart
import com.github.mikephil.charting.components.AxisBase
import com.github.mikephil.charting.components.Description
import com.github.mikephil.charting.components.YAxis
import com.github.mikephil.charting.data.CombinedData
import com.github.mikephil.charting.data.Entry
import com.github.mikephil.charting.data.LineData
import com.github.mikephil.charting.data.LineDataSet
import com.github.mikephil.charting.formatter.IAxisValueFormatter
import com.github.mikephil.charting.formatter.ValueFormatter
import com.google.android.youtube.player.YouTubeInitializationResult
import com.google.android.youtube.player.YouTubePlayer
import com.google.android.youtube.player.YouTubePlayerFragment
import ua.nure.sheliemietiev.esportraisemobile.App
import ua.nure.sheliemietiev.esportraisemobile.R
import java.text.SimpleDateFormat
import java.util.*
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
            if (stateData.stateRecords.count() == 0) {
                return@Observer
            }
            val baseDate = stateData.stateRecords[0].date.time

            val hrDataSet = LineDataSet(stateData.stateRecords.map {
                Entry(
                    ((it.date.time - baseDate) / 1000).toFloat(),
                    it.heartrate.toFloat()
                )
            }, "HR")
            hrDataSet.axisDependency = YAxis.AxisDependency.LEFT
            hrDataSet.color =
                ContextCompat.getColor(this, R.color.heartRate) // TODO: replace ints with constants from styles
            val temperatureDataSet = LineDataSet(stateData.stateRecords.map {
                Entry(
                    ((it.date.time - baseDate) / 1000).toFloat(),
                    it.temperature
                )
            }, "Temperature")
            temperatureDataSet.axisDependency = YAxis.AxisDependency.RIGHT
            temperatureDataSet.color =
                ContextCompat.getColor(this, R.color.temperature) // TODO: replace ints with constants from styles
            lineChart.data = LineData(hrDataSet, temperatureDataSet)
//            lineChart.notifyDataSetChanged()
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
        })

        trainingViewModel.broadcastUrl.observe(this, Observer {
            if (youtubePlayer == null || it == null) {
                return@Observer
            }
            youtubePlayer!!.loadVideo(it)
            youtubePlayer!!.play()
        })
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