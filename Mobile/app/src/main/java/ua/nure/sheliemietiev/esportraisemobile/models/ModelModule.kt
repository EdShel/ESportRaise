package ua.nure.sheliemietiev.esportraisemobile.models

import dagger.Module
import dagger.Provides
import kotlin.math.sqrt

@Module
class ModelModule {
    @Provides
    fun provideStressFinder(): StressFinder {
        return StdevStressFinder()
    }
}


interface StressFinder {
    fun isNervous(stateIndicatorValues: Iterable<Float>): Boolean
}

class StdevStressFinder : StressFinder {
    override fun isNervous(stateIndicatorValues: Iterable<Float>): Boolean {
        val average = stateIndicatorValues.average()
        val sumOfSquares = stateIndicatorValues.reduce { sumOfSquareDeviations, currentValue ->
            val deviation = currentValue - average
            (sumOfSquareDeviations + deviation * deviation).toFloat()
        }
        val stdev = sqrt(sumOfSquares / stateIndicatorValues.count())
        val currentStateIndicatorValue = stateIndicatorValues.last()
        return currentStateIndicatorValue > average + stdev * 2
    }
}
