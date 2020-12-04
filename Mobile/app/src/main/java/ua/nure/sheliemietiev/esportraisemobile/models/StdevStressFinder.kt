package ua.nure.sheliemietiev.esportraisemobile.models

import kotlin.math.sqrt

class StdevStressFinder :
    StressFinder {
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