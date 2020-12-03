package ua.nure.sheliemietiev.esportraisemobile.ui.main

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import ua.nure.sheliemietiev.esportraisemobile.App
import ua.nure.sheliemietiev.esportraisemobile.BuildConfig
import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.ui.connectIot.ConnectIotActivity
import ua.nure.sheliemietiev.esportraisemobile.ui.training.TrainingActivity
import javax.inject.Inject

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
        startTrainingButton = findViewById(R.id.start_training_button)

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

        val goToWebButton = findViewById<Button>(R.id.goToWebButton)
        goToWebButton.setOnClickListener {
            val browseIntent = Intent(Intent.ACTION_VIEW, Uri.parse(BuildConfig.WEB_URL))
            startActivity(browseIntent)
        }

        val startTrainingButton = findViewById<Button>(R.id.start_training_button)
        startTrainingButton.setOnClickListener {
            val connectIotActivity = Intent(this, ConnectIotActivity::class.java)
            startActivity(connectIotActivity)
        }
    }
}