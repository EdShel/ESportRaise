package ua.nure.sheliemietiev.esportraisemobile.ui.training

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import javax.inject.Inject

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