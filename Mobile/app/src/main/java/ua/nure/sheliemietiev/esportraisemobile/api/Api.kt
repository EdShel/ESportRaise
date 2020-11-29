package ua.nure.sheliemietiev.esportraisemobile.api

import android.annotation.SuppressLint
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.Request
import ru.gildor.coroutines.okhttp.await
import java.security.SecureRandom
import java.security.cert.CertificateException
import java.security.cert.X509Certificate
import javax.inject.Inject
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

class Api @Inject constructor(
    private val requestBuilder: ApiRequestBuilder,
    private val authorization: AuthorizationInfo
) {
    private val client: OkHttpClient = getCertIgnoringOkHttpClient().build()

    private fun getCertIgnoringOkHttpClient(): OkHttpClient.Builder {
        try {
            val trustAllCerts = arrayOf<TrustManager>(
                object : X509TrustManager {
                    @SuppressLint("TrustAllX509TrustManager")
                    @Throws(CertificateException::class)
                    override fun checkClientTrusted(
                        chain: Array<X509Certificate>,
                        authType: String
                    ) {
                    }

                    @SuppressLint("TrustAllX509TrustManager")
                    @Throws(CertificateException::class)
                    override fun checkServerTrusted(
                        chain: Array<X509Certificate>,
                        authType: String
                    ) {
                    }

                    override fun getAcceptedIssuers(): Array<X509Certificate> {
                        return arrayOf()
                    }
                }
            )
            val sslContext = SSLContext.getInstance("SSL")
            sslContext.init(null, trustAllCerts, SecureRandom())
            val sslSocketFactory = sslContext.socketFactory
            return OkHttpClient.Builder()
                .sslSocketFactory(sslSocketFactory, trustAllCerts[0] as X509TrustManager)
                .hostnameVerifier { _, _ -> true }
        } catch (e: Exception) {
            throw RuntimeException(e)
        }
    }

    private suspend fun sendAsync(request: Request): ApiResponse {
        return ApiResponse(client.newCall(request).await())
    }

    suspend fun post(
        relativeUrl: String,
        queryParams: Map<String, String>?,
        body: Any?
    ): ApiResponse {
        var request = requestBuilder.buildPostRequest(relativeUrl, queryParams, body)

        return withContext(Dispatchers.IO) {
            var response = sendAsync(request)
            if (response.statusCode == 401 && authorization.isAuthorized) {
                val refreshRequest = requestBuilder.buildRefreshRequest()
                val refreshResponse = sendAsync(refreshRequest)
                if (refreshResponse.statusCode == 200) {
                    val jsonBody = refreshResponse.asJsonMap()
                    authorization.changeAuth(
                        jsonBody["token"].asString,
                        jsonBody["refreshToken"].asString
                    )

                    request = requestBuilder.buildPostRequest(relativeUrl, queryParams, body)
                    response = sendAsync(request)
                }
            }

            return@withContext response
        }
    }

    suspend fun get(
        relativeUrl: String,
        queryParams: Map<String, String>?
    ): ApiResponse {
        var request = requestBuilder.buildGetRequest(relativeUrl, queryParams)

        return withContext(Dispatchers.IO) {
            var response = sendAsync(request)
            if (response.statusCode == 401 && authorization.isAuthorized) {
                val refreshRequest = requestBuilder.buildRefreshRequest()
                val refreshResponse = sendAsync(refreshRequest)
                if (refreshResponse.statusCode == 200) {
                    val jsonBody = refreshResponse.asJsonMap()
                    authorization.changeAuth(
                        jsonBody["token"].asString,
                        jsonBody["refreshToken"].asString
                    )

                    request = requestBuilder.buildGetRequest(relativeUrl, queryParams)
                    response = sendAsync(request)
                }
            }

            return@withContext response
        }
    }
}
