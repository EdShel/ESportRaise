package ua.nure.sheliemietiev.esportraisemobile

import android.app.Application
import ua.nure.sheliemietiev.esportraisemobile.util.StorageModule

class App : Application() {

    val components: ApplicationComponent = DaggerApplicationComponent.builder()
        .storageModule(StorageModule(this))
        .build();

    override fun onCreate() {
        super.onCreate()
    }
}

