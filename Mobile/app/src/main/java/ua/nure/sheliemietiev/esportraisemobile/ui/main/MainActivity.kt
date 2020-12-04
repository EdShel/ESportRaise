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
import ua.nure.sheliemietiev.esportraisemobile.ui.language.LanguageActivity
import ua.nure.sheliemietiev.esportraisemobile.ui.login.LoginActivity
import ua.nure.sheliemietiev.esportraisemobile.ui.training.TrainingActivity
import javax.inject.Inject

class MainActivity : AppCompatActivity() {

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    private lateinit var mainViewModel: MainViewModel

    private lateinit var viewTrainingButton: Button

    private lateinit var startTrainingButton: Button

    private lateinit var changeLanguageButton: Button

    private lateinit var goToWebButton: Button

    private lateinit var logoutButton: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        (applicationContext as App).components.inject(this)
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        mainViewModel = ViewModelProvider(this, viewModelFactory)
            .get(MainViewModel::class.java)

        viewTrainingButton = findViewById(R.id.view_current_training_button)
        startTrainingButton = findViewById(R.id.start_training_button)
        changeLanguageButton = findViewById(R.id.language_button)
        goToWebButton = findViewById(R.id.goToWebButton)
        logoutButton = findViewById(R.id.logout_button)

        viewTrainingButton.setOnClickListener {
            val trainingActivity = Intent(this, TrainingActivity::class.java)
            startActivity(trainingActivity)
        }

        startTrainingButton.setOnClickListener {
            val connectIotActivity = Intent(this, ConnectIotActivity::class.java)
            startActivity(connectIotActivity)
        }

        changeLanguageButton.setOnClickListener {
            val languageActivity = Intent(this, LanguageActivity::class.java)
            startActivity(languageActivity)
        }

        goToWebButton.setOnClickListener {
            val browseIntent = Intent(Intent.ACTION_VIEW, Uri.parse(BuildConfig.WEB_URL))
            startActivity(browseIntent)
        }

        logoutButton.setOnClickListener {
            mainViewModel.clearAuthorizationInfo()
            val connectIotActivity = Intent(this, LoginActivity::class.java)
            finish()
            startActivity(connectIotActivity)
        }

        observeUserInfoChanged()
    }

    private fun observeUserInfoChanged() {
        mainViewModel.teamMemberData.observe(this, Observer {
            when {
                it.teamId == null -> {
                    viewTrainingButton.isEnabled = false
                    startTrainingButton.isEnabled = false
                }
                it.currentTrainingId == null -> {
                    viewTrainingButton.isEnabled = false
                    startTrainingButton.isEnabled = true
                }
                else -> {
                    viewTrainingButton.isEnabled = true
                    startTrainingButton.isEnabled = true
                }
            }
        })
    }
}