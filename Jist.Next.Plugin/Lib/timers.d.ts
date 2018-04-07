
declare module 'timers' {
    export type TimeoutCallbackFunc = () => void;

    /**
     * Schedules the specified callback to run after the provided timeout (in milliseconds) have elapsed.
     * @param callback The callback function to run after the specified timeout.
     * @param timeout The time, in milliseconds (ms), after which the callback will run.
     * @returns {string} Returns a unique identifier which can be used to cancel the interval when
     * provided to the @see setInterval function.
     */
    export function setTimeout(callback: TimeoutCallbackFunc, timeout: number): string;

    /**
     * Schedules the specified callback to run at the provided interval (in milliseconds), repeated
     * until stopped with the @see cancelTimeout function.
     * @param callback The callback function to run after the specified timeout.
     * @param timeout The time, in milliseconds (ms), after which the callback will run.
     * @returns {string} Returns a unique identifier which can be used to cancel the interval when
     * provided to the @see setInterval function.
     */
    export function setInterval(callback: TimeoutCallbackFunc, interval: number): string;

    /**
     * Cancels a timeout or interval.
     * @param id The unique identifier generated from calls to the @see setInterval or @see setTimeout
     * function.
     */
    export function cancelTimeout(id: string): void;
}