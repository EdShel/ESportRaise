package ua.nure.sheliemietiev.esportraisemobile.api

import dagger.Module
import dagger.Provides
import ua.nure.sheliemietiev.esportraisemobile.BuildConfig
import ua.nure.sheliemietiev.esportraisemobile.util.SecureStorage

@Module
class ApiModule {
    @Provides
    fun provideAuthorizationInfo(storage : SecureStorage): AuthorizationInfo {
        val authInfo = AuthorizationInfo(storage)
        authInfo.loadFromStorage()
        return authInfo
    }

    @Provides
    fun provideRequestBuilder(authInfo: AuthorizationInfo) : ApiRequestBuilder{
        return ApiRequestBuilder(BuildConfig.API_URL, authInfo)
    }
}