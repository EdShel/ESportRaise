package ua.nure.sheliemietiev.esportraisemobile

import android.content.Context
import dagger.Component
import ua.nure.sheliemietiev.esportraisemobile.api.ApiModule
import ua.nure.sheliemietiev.esportraisemobile.models.ModelModule
import ua.nure.sheliemietiev.esportraisemobile.ui.connectIot.ConnectIotActivity
import ua.nure.sheliemietiev.esportraisemobile.ui.login.LoginActivity
import ua.nure.sheliemietiev.esportraisemobile.ui.main.MainActivity
import ua.nure.sheliemietiev.esportraisemobile.ui.training.TrainingActivity
import ua.nure.sheliemietiev.esportraisemobile.util.StorageModule
import javax.inject.Singleton

@Singleton
@Component(
    modules = [
        StorageModule::class,
        ApiModule::class,
        ViewModelModule::class,
        ModelModule::class
    ]
)
interface ApplicationComponent {
    fun context(): Context

    fun inject(loginActivity: LoginActivity)

    fun inject(trainingActivity: TrainingActivity)

    fun inject(mainActivity: MainActivity)

    fun inject(mainActivity: ConnectIotActivity)
}

