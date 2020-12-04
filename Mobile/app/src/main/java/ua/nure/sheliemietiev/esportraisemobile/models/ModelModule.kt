package ua.nure.sheliemietiev.esportraisemobile.models

import dagger.Module
import dagger.Provides

@Module
class ModelModule {
    @Provides
    fun provideStressFinder(): StressFinder {
        return StdevStressFinder()
    }
}
