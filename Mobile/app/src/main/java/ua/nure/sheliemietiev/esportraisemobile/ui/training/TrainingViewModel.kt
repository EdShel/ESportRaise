package ua.nure.sheliemietiev.esportraisemobile.ui.training

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
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

class TrainingViewModel @Inject constructor(
    private val youTubeModel: YouTubeModel
) : ViewModel() {

    private val _youtubeKeyData: LiveData<String>;
    val youtubeKeyData get() = _youtubeKeyData

    init {
        _youtubeKeyData = object : LiveData<String>() {
            override fun onActive() {
                viewModelScope.launch {
                    value = youTubeModel.getYoutubePlayerKey()
                }
            }
        }
    }
}