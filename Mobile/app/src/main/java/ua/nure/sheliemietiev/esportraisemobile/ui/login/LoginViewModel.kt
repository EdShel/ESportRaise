package ua.nure.sheliemietiev.esportraisemobile.ui.login

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.models.LoginModel
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import java.util.regex.Pattern
import javax.inject.Inject

const val EMAIL_PATTERN =
    "^(([^<>()\\[\\]\\\\.,;:\\s@\"\"]+(\\.[^<>()\\[\\]\\\\.,;:\\s@\"\"]+)*)|(\"\".+\"\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))\$"

const val USERNAME_PATTERN = "^[A-Za-z0-9_]{3,20}\$"

const val PASSWORD_PATTERN =
    "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{6,20}\$"

class LoginViewModel @Inject constructor(
    private val loginModel: LoginModel
) : ViewModel() {

    private val _loginFormState = MutableLiveData<LoginFormState>()
    val loginFormState: LiveData<LoginFormState> get() = _loginFormState

    private val _loginResult = MutableLiveData<OperationResult<AuthResultView>>()
    val loginResult: LiveData<OperationResult<AuthResultView>> get() = _loginResult

    private val emailPattern = Pattern.compile(EMAIL_PATTERN)

    private val usernamePattern = Pattern.compile(USERNAME_PATTERN)

    private val passwordPattern = Pattern.compile(PASSWORD_PATTERN)

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

    fun loginDataChanged(username: String, password: String) {
        if (!isUserNameValid(username)) {
            _loginFormState.value = LoginFormState(usernameError = R.string.invalid_username)
        } else if (!isPasswordValid(password)) {
            _loginFormState.value = LoginFormState(passwordError = R.string.invalid_password)
        } else {
            _loginFormState.value = LoginFormState(isDataValid = true)
        }
    }

    private fun isUserNameValid(username: String): Boolean {
        return usernamePattern.matcher(username).matches()
                || emailPattern.matcher(username).matches()
    }

    private fun isPasswordValid(password: String): Boolean {
        return passwordPattern.matcher(password).matches()
    }
}