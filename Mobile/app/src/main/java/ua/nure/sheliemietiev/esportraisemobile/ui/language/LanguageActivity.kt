package ua.nure.sheliemietiev.esportraisemobile.ui.language

import android.content.Intent
import android.content.res.Configuration
import android.content.res.Resources
import android.os.Build
import android.os.Bundle
import android.util.DisplayMetrics
import android.widget.ArrayAdapter
import android.widget.ListView
import androidx.appcompat.app.AppCompatActivity
import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.ui.main.MainActivity
import java.util.*


class LanguageActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_language)

        val supportedLanguages = resources.getStringArray(R.array.supported_languages)
        val supportedLocales = resources.getStringArray(R.array.supported_locales)
        val languagesList = findViewById<ListView>(R.id.languages_list)
        languagesList.adapter = ArrayAdapter(
            this,
            R.layout.support_simple_spinner_dropdown_item,
            supportedLanguages
        )
        languagesList.setOnItemClickListener { adapter, parent, position, id ->
            val localeName = supportedLocales[position]
            setAppLocale(localeName)
            val refresh = Intent(this, MainActivity::class.java)
            startActivity(refresh)
        }
    }

    private fun setAppLocale(locale: String) {
        val resources: Resources = resources
        val dm: DisplayMetrics = resources.displayMetrics
        val config: Configuration = resources.configuration
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
            config.setLocale(Locale(locale.toLowerCase()))
        } else {
            config.locale = Locale(locale.toLowerCase())
        }
        resources.updateConfiguration(config, dm)
    }
}