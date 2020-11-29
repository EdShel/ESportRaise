package ua.nure.sheliemietiev.esportraisemobile.util

import android.content.Context
import android.content.SharedPreferences
import dagger.Module
import dagger.Provides

@Module
class StorageModule(private val context: Context) {

    @Provides
    fun provideContext(): Context {
        return context
    }

    @Provides
    fun provideSharedPreferences(context: Context): SharedPreferences {
        return context.getSharedPreferences(
            "ESportRaise",
            Context.MODE_PRIVATE
        )
    }
}