package ua.nure.sheliemietiev.esportraisemobile.models

import ua.nure.sheliemietiev.esportraisemobile.api.Api
import javax.inject.Inject

class YouTubeModel @Inject constructor(
    private val api: Api
) {
    suspend fun getYoutubePlayerKey(): String {
        val response = api.get("key/youtube", null)
        if (response.statusCode == 200) {
            val json = response.asJsonMap()
            return json["key"].asString
        }
        return ""
    }
}