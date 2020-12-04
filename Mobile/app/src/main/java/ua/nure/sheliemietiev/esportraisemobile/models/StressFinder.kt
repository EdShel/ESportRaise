package ua.nure.sheliemietiev.esportraisemobile.models

interface StressFinder {
    fun isNervous(stateIndicatorValues: Iterable<Float>): Boolean
}