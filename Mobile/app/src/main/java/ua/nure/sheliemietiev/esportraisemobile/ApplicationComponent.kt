package ua.nure.sheliemietiev.esportraisemobile

import android.content.Context
import dagger.Component
import ua.nure.sheliemietiev.esportraisemobile.api.ApiModule
import ua.nure.sheliemietiev.esportraisemobile.ui.login.LoginActivity
import ua.nure.sheliemietiev.esportraisemobile.util.StorageModule
import javax.inject.Singleton

@Singleton
@Component(modules = [StorageModule::class, ApiModule::class, ViewModelModule::class])
interface ApplicationComponent {
    fun context() : Context

    fun inject(loginActivity: LoginActivity)
}

