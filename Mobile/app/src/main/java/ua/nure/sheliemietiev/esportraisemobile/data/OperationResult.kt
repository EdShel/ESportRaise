package ua.nure.sheliemietiev.esportraisemobile.data

open class OperationResult<out T> constructor(
    private val value: T?
) {
    val isSuccess: Boolean get() = value != null

    val isFailure: Boolean get() = value == null

    fun getOrNull(): T? =
        when {
            isFailure -> null
            else -> value
        }

    fun getOrThrow(): T {
        if (this is OperationFailure){
            throw this.exception;
        }
        if (value == null){
            throw Exception("Value is null!")
        }
        return this.value
    }

    companion object {
        fun <T> success(value: T): OperationResult<T> =
            OperationResult(value)

        fun <T> failure(error : Throwable?): OperationResult<T> =
            OperationFailure(error ?: Exception("Operation failure!"))
    }

    private class OperationFailure<T>(
        internal val exception : Throwable
    ) : OperationResult<T>(null)
}