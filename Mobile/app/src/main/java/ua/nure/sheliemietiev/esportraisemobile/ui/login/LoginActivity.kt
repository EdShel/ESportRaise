package ua.nure.sheliemietiev.esportraisemobile.ui.login

import android.app.Activity
import android.content.SharedPreferences
import androidx.lifecycle.Observer
import android.os.Bundle
import androidx.annotation.StringRes
import androidx.appcompat.app.AppCompatActivity
import android.text.Editable
import android.text.TextWatcher
import android.view.View
import android.view.inputmethod.EditorInfo
import android.widget.Button
import android.widget.EditText
import android.widget.ProgressBar
import android.widget.Toast
import androidx.lifecycle.ViewModelProvider
import ua.nure.sheliemietiev.esportraisemobile.App

import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.api.AuthorizationInfo
import javax.inject.Inject

class LoginActivity : AppCompatActivity() {

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    @Inject
    lateinit var authorizationInfo: AuthorizationInfo

    override fun onCreate(savedInstanceState: Bundle?) {
        (applicationContext as App).components.inject(this)

        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        val loginViewModel = ViewModelProvider(this, viewModelFactory)
            .get(LoginViewModel::class.java)

        val username = findViewById<EditText>(R.id.username)
        val password = findViewById<EditText>(R.id.password)
        val loginButton = findViewById<Button>(R.id.login)
        val loading = findViewById<ProgressBar>(R.id.loading)



        username.setText(authorizationInfo.userName)
        password.setText("Qwerty12345@")

        loginViewModel.loginFormState.observe(this@LoginActivity, Observer {
            val loginState = it ?: return@Observer

            // disable login button unless both username / password is valid
            loginButton.isEnabled = loginState.isDataValid
            if (loginState.usernameError != null) {
                username.error = getString(loginState.usernameError)
            }
            if (loginState.passwordError != null) {
                password.error = getString(loginState.passwordError)
            }
        })

        loginViewModel.loginResult.observe(this@LoginActivity, Observer {
            val loginResult = it ?: return@Observer
            Toast.makeText(this, "Loool", Toast.LENGTH_LONG).show()
            loading.visibility = View.GONE
            if (loginResult.isFailure) {
                showLoginFailed(loginResult.getErrorCode())
            }
            if (loginResult.isSuccess) {
                updateUiWithUser(loginResult.getOrThrow())
            }
            setResult(Activity.RESULT_OK)

            //Complete and destroy login activity once successful
            finish()
        })

        username.afterTextChanged {
            loginViewModel.loginDataChanged(
                username.text.toString(),
                password.text.toString()
            )
        }

        password.apply {
            afterTextChanged {
                loginViewModel.loginDataChanged(
                    username.text.toString(),
                    password.text.toString()
                )
            }

            setOnEditorActionListener { _, actionId, _ ->
                when (actionId) {
                    EditorInfo.IME_ACTION_DONE ->
                        loginViewModel.loginButtonPressed(
                            username.text.toString(),
                            password.text.toString()
                        )
                }
                false
            }

            loginButton.setOnClickListener {
                loading.visibility = View.VISIBLE
                loginViewModel.loginButtonPressed(
                    username.text.toString(),
                    password.text.toString()
                )
            }
        }
    }

    private fun updateUiWithUser(model: AuthorizedView) {
        val welcome = getString(R.string.welcome)
        val displayName = model.userName
        // TODO : initiate successful logged in experience
        Toast.makeText(
            applicationContext,
            "$welcome $displayName",
            Toast.LENGTH_LONG
        ).show()
    }

    private fun showLoginFailed(@StringRes errorString: Int) {
        Toast.makeText(applicationContext, errorString, Toast.LENGTH_SHORT).show()
    }
}

/**
 * Extension function to simplify setting an afterTextChanged action to EditText components.
 */
fun EditText.afterTextChanged(afterTextChanged: (String) -> Unit) {
    this.addTextChangedListener(object : TextWatcher {
        override fun afterTextChanged(editable: Editable?) {
            afterTextChanged.invoke(editable.toString())
        }

        override fun beforeTextChanged(s: CharSequence, start: Int, count: Int, after: Int) {}

        override fun onTextChanged(s: CharSequence, start: Int, before: Int, count: Int) {}
    })
}