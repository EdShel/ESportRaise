package ua.nure.sheliemietiev.esportraisemobile.ui.login

import android.util.Patterns
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import javax.inject.Inject

class LoginViewModel @Inject constructor(
    private val loginModel: LoginModel
) : ViewModel() {

    private val _loginFormState = MutableLiveData<LoginFormState>()
    val loginFormState: LiveData<LoginFormState> get() = _loginFormState

    private val _loginResult = MutableLiveData<OperationResult<AuthResultView>>()
    val loginResult: LiveData<OperationResult<AuthResultView>> get() = _loginResult

    init {
        if (loginModel.authInfo.isAuthorized) {
            _loginResult.value = OperationResult.success(
                AuthResultView(
                    loginModel.authInfo.userName
                )
            )
        }
    }

    fun loginButtonPressed(email: String, password: String) {
        viewModelScope.launch {
            val result = loginModel.signIn(email, password)
            _loginResult.value = result
        }
    }

    // TODO: validate
    fun loginDataChanged(username: String, password: String) {
        if (!isUserNameValid(username)) {
            _loginFormState.value = LoginFormState(usernameError = R.string.invalid_username)
        } else if (!isPasswordValid(password)) {
            _loginFormState.value = LoginFormState(passwordError = R.string.invalid_password)
        } else {
            _loginFormState.value = LoginFormState(isDataValid = true)
        }
    }

    // A placeholder username validation check
    private fun isUserNameValid(username: String): Boolean {
        return if (username.contains('@')) {
            Patterns.EMAIL_ADDRESS.matcher(username).matches()
        } else {
            username.isNotBlank()
        }
    }

    // A placeholder password validation check
    private fun isPasswordValid(password: String): Boolean {
        return password.length > 5
    }
}