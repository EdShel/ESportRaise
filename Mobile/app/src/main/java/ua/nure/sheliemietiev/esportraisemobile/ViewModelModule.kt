package ua.nure.sheliemietiev.esportraisemobile

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import dagger.Binds
import dagger.Module
import dagger.multibindings.IntoMap
import ua.nure.sheliemietiev.esportraisemobile.ui.connectIot.ConnectIotViewModel
import ua.nure.sheliemietiev.esportraisemobile.ui.login.LoginViewModel
import ua.nure.sheliemietiev.esportraisemobile.ui.main.MainViewModel
import ua.nure.sheliemietiev.esportraisemobile.ui.training.TrainingViewModel
import ua.nure.sheliemietiev.esportraisemobile.util.ViewModelFactory
import ua.nure.sheliemietiev.esportraisemobile.util.ViewModelKey

@Module
abstract class ViewModelModule {

    @Binds
    internal abstract fun bindViewModelFactory(factory: ViewModelFactory): ViewModelProvider.Factory

    @Binds
    @IntoMap
    @ViewModelKey(LoginViewModel::class)
    internal abstract fun provideLoginViewModel(viewModel: LoginViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(TrainingViewModel::class)
    internal abstract fun provideTrainingViewModel(viewModel: TrainingViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(MainViewModel::class)
    internal abstract fun provideMainViewModel(viewModel: MainViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(ConnectIotViewModel::class)
    internal abstract fun provideConnectIotViewModel(viewModel: ConnectIotViewModel): ViewModel
}