package ua.nure.sheliemietiev.esportraisemobile.ui.login

import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.AuthorizationInfo
import ua.nure.sheliemietiev.esportraisemobile.util.OperationResult
import javax.inject.Inject

class LoginModel @Inject constructor(
    var api: Api,
    var authInfo: AuthorizationInfo
) {
    suspend fun signIn(email: String, password: String): OperationResult<AuthResultView> {
        val response = api.post(
            "auth/login", null, mapOf<String, Any>(
                "emailOrUserName" to email,
                "password" to password
            )
        )

        if (response.statusCode != 200){
            return OperationResult.error(R.string.wrong_credentials)
        }
        val json = response.asJsonMap()
        val userName = json["userName"].asString
        val token = json["token"].asString
        val refreshToken = json["refreshToken"].asString
        authInfo.setUser(
            userName,
            email,
            token,
            refreshToken
        )
        return OperationResult.success(
            AuthResultView(
                userName
            )
        )
    }
}