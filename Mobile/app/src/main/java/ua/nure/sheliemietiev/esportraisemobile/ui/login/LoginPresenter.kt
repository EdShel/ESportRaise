package ua.nure.sheliemietiev.esportraisemobile.ui.login

import android.util.Patterns
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.data.OperationResult
import javax.inject.Inject

class LoginModel {
    fun signIn(email: String, password: String) : OperationResult<AuthorizedView> {
        return OperationResult.success(AuthorizedView("Eduardo!!!"))
    }
}

class AuthorizedView(val userName : String)

class LoginPresenter {
    @Inject
    lateinit var loginModel: LoginModel

    private val _loginFormState = MutableLiveData<LoginFormState>()
    val loginFormState: LiveData<LoginFormState> get() = _loginFormState

    private val _loginResult = MutableLiveData<LoginResult>()
    val loginResult: LiveData<LoginResult> get() = _loginResult

    fun login(email: String, password: String) {
        val result = loginModel.signIn(email, password)

        if (result.isSuccess) {
            val data = result.getOrThrow()
            _loginResult.value = LoginResult(success = LoggedInUserView(displayName = data.userName))
        } else {
            _loginResult.value = LoginResult(error = R.string.login_failed)
        }

        var s : Result<String> = Result.success("lol")
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