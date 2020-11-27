package ua.nure.sheliemietiev.esportraisemobile.api

import dagger.Module
import dagger.Provides
import ua.nure.sheliemietiev.esportraisemobile.BuildConfig

@Module
class ApiModule {
    @Provides
    fun provideAuthorizationInfo(): AuthorizationInfo {
        return AuthorizationInfo(
            "Eduardo",
            "",
            "",
            ""
        )
    }

    @Provides
    fun provideApiClient(authInfo : AuthorizationInfo) : Api {
        return Api(
            BuildConfig.API_URL,
            authInfo
        )
    }
}