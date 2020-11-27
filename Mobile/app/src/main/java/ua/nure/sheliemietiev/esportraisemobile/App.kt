package ua.nure.sheliemietiev.esportraisemobile

import android.app.Application

class App : Application() {

    val components: ApplicationComponent = DaggerApplicationComponent.create()

    override fun onCreate() {
        super.onCreate()
    }
}

