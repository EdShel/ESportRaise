package ua.nure.sheliemietiev.esportraisemobile

import android.app.Application

class App : Application() {

    private var components : ApplicationComponent = DaggerApplicationComponent.create()

    override fun onCreate() {
        super.onCreate()
    }
}

