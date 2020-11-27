package ua.nure.sheliemietiev.esportraisemobile.api

import android.annotation.SuppressLint
import com.google.gson.Gson
import com.google.gson.JsonElement
import okhttp3.*
import java.io.IOException
import java.security.SecureRandom
import java.security.cert.CertificateException
import java.security.cert.X509Certificate
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

class AuthorizationInfo(
    var userName: String,
    var email: String,
    var token: String,
    var refreshToken: String
)

class Api(
    private val apiUrl: String,
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

    fun buildPost(relativeUrl: String, queryParams: Map<String, String>?, body: Any?): Request {
        val urlBuilder = HttpUrl.parse("$apiUrl/$relativeUrl")!!.newBuilder()
        queryParams?.forEach { (name, value) -> urlBuilder.addQueryParameter(name, value) }
        val bodyJson = Gson().toJson(body)
        val requestBuilder = Request.Builder()
            .url(urlBuilder.build())
            .post(RequestBody.create(MediaType.parse("application/json"), bodyJson))

        requestBuilder.addHeader("Authorization", "Bearer ${authorization.token}")

        return requestBuilder.build()
    }

    fun runAsync(request: Request, onSuccess: (Response) -> Unit, onError: (IOException) -> Unit) {
        client.newCall(request).enqueue(object : Callback {
            override fun onResponse(call: Call?, response: Response?) {
                if (response!!.code() == 401) {
                    refreshAndRetry(request, onSuccess, onError)
                } else {
                    onSuccess(response)
                }
            }

            override fun onFailure(p0: Call?, p1: IOException?) {
                onError(p1!!)
                p0!!.cancel()
            }
        })
    }

    private fun refreshAndRetry(
        request: Request,
        onSuccess: (Response) -> Unit,
        onError: (IOException) -> Unit
    ) {
        val refreshRequest = buildRefreshRequest()
        client.newCall(refreshRequest).enqueue(object : Callback {
            override fun onResponse(call: Call, response: Response) {
                val jsonBodyString = response.body()!!.string()
                val jsonBody = Gson().fromJson(jsonBodyString, JsonElement::class.java).asJsonObject
                authorization.token = jsonBody["token"].asString
                authorization.refreshToken = jsonBody["refreshToken"].asString

                val requestWithFreshToken = request.newBuilder()
                    .header("Authorization", "Bearer ${authorization.token}")
                    .build()

                runWithNoRefreshAsync(requestWithFreshToken, onSuccess, onError)
            }

            override fun onFailure(p0: Call?, p1: IOException?) {
                onError(p1!!)
                p0!!.cancel()
            }
        })
    }

    private fun buildRefreshRequest(): Request {
        return this.buildPost("auth/refresh", null, object {
            val refreshToken = authorization.refreshToken
        })
    }

    private fun runWithNoRefreshAsync(
        request: Request,
        onSuccess: (Response) -> Unit,
        onError: (IOException) -> Unit
    ) {
        client.newCall(request).enqueue(object : Callback {
            override fun onResponse(call: Call?, response: Response?) {
                onSuccess(response!!)
            }

            override fun onFailure(p0: Call?, p1: IOException?) {
                onError(p1!!)
                p0!!.cancel()
            }
        })
    }
}

