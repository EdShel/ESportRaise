package ua.nure.sheliemietiev.esportraisemobile.models

import ua.nure.sheliemietiev.esportraisemobile.R
import ua.nure.sheliemietiev.esportraisemobile.api.Api
import ua.nure.sheliemietiev.esportraisemobile.api.ApiResponse
import ua.nure.sheliemietiev.esportraisemobile.api.AuthorizationInfo
import ua.nure.sheliemietiev.esportraisemobile.api.StatusCode
import ua.nure.sheliemietiev.esportraisemobile.ui.login.AuthResultView
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

        return when (response.statusCode) {
            StatusCode.OK.code ->
                createSuccessfulAuthorizationResult(response, email)
            StatusCode.BAD_GATEWAY.code ->
                OperationResult.error(R.string.server_not_accessible)
            else ->
                OperationResult.error(R.string.wrong_credentials)
        }
    }

    private fun createSuccessfulAuthorizationResult(
        response: ApiResponse,
        email: String
    ): OperationResult<AuthResultView> {
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