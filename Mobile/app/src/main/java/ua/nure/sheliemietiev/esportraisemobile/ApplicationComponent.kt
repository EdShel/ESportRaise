package ua.nure.sheliemietiev.esportraisemobile

import dagger.Component
import ua.nure.sheliemietiev.esportraisemobile.api.ApiModule
import ua.nure.sheliemietiev.esportraisemobile.ui.login.LoginActivity

@Component(modules = [ApiModule::class])
interface ApplicationComponent {
    fun inject(loginActivity: LoginActivity)
}

