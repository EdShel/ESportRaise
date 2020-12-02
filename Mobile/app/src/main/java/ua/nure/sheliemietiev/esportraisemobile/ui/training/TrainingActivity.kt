package ua.nure.sheliemietiev.esportraisemobile.ui.training

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.core.os.postDelayed
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import com.github.mikephil.charting.charts.CombinedChart
import com.github.mikephil.charting.charts.LineChart
import com.github.mikephil.charting.components.Description
import com.github.mikephil.charting.data.CombinedData
import com.github.mikephil.charting.data.Entry
import com.github.mikephil.charting.data.LineData
import com.github.mikephil.charting.data.LineDataSet
import com.google.android.youtube.player.YouTubeInitializationResult
import com.google.android.youtube.player.YouTubePlayer
import com.google.android.youtube.player.YouTubePlayerFragment
import ua.nure.sheliemietiev.esportraisemobile.App
import ua.nure.sheliemietiev.esportraisemobile.R
import javax.inject.Inject


class TrainingActivity : AppCompatActivity() {

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    lateinit var trainingViewModel: TrainingViewModel

    private lateinit var physStateRefreshHandler: Handler

    override fun onCreate(savedInstanceState: Bundle?) {
        (applicationContext as App).components.inject(this)
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_training)
        trainingViewModel = ViewModelProvider(this, viewModelFactory)
            .get(TrainingViewModel::class.java)

        setupYouTubePlayer()

        val lineChart = findViewById<LineChart>(R.id.physStateChart)

        val yValues = java.util.ArrayList<Entry>()
        yValues.add(Entry(0f, 1f))
        yValues.add(Entry(1f, 2f))
        yValues.add(Entry(2f, 3f))
        yValues.add(Entry(3f, 4f))
        yValues.add(Entry(4f, 5f))
        val lineDataSet = LineDataSet(yValues, "HR")
        val yValues2 = java.util.ArrayList<Entry>()
        yValues2.add(Entry(0f, 10f))
        yValues2.add(Entry(1f, 20f))
        yValues2.add(Entry(2f, 30f))
        yValues2.add(Entry(3f, 40f))
        yValues2.add(Entry(4f, 50f))
        val lineDataSet2 = LineDataSet(yValues2, "t")

        val data = LineData(lineDataSet, lineDataSet2)

        lineChart.data = data
        val descr = Description()
        descr.text = "Physical state"
        lineChart.description = descr

        lineChart.animateX(2000)
        lineChart.invalidate()

        trainingViewModel.playerStateData.observe(this, Observer { stateData ->
            val hrDataSet = LineDataSet(stateData.stateRecords.map {
                Entry(it.date.time.toFloat(), it.heartrate.toFloat())
            }, "HR")
            val temperatureDataSet = LineDataSet(stateData.stateRecords.map {
                Entry(it.date.time.toFloat(), it.temperature)
            }, "Temperature")
            lineChart.data = LineData(hrDataSet, temperatureDataSet)
            lineChart.notifyDataSetChanged()
            lineChart.invalidate()
        })


    }

    override fun onResume() {
        super.onResume()
        val periodSeconds = 5000L
        physStateRefreshHandler = Handler(Looper.getMainLooper())
        physStateRefreshHandler.postDelayed(object : Runnable {
            override fun run() {
                trainingViewModel.updatePlayerState()
                physStateRefreshHandler.postDelayed(this, periodSeconds)
            }
        }, periodSeconds)
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
                        player.loadVideo("mM7C_Pw7OL8");
                        player.play();
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