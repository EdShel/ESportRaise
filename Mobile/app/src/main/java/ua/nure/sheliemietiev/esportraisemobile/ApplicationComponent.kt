package ua.nure.sheliemietiev.esportraisemobile

import dagger.Component
import ua.nure.sheliemietiev.esportraisemobile.api.ApiModule
import ua.nure.sheliemietiev.esportraisemobile.ui.login.LoginActivity
import javax.inject.Singleton

@Singleton
@Component(modules = [ApiModule::class, ViewModelModule::class])
interface ApplicationComponent {
    fun inject(loginActivity: LoginActivity)
}

