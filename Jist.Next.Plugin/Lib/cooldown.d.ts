
declare module 'cooldown' {
    /**
     * Sets a cooldown on any unique key.  Check whether a player is on cooldown with the
     * @see checkCooldown function.
     */
    export function setCooldown(key: string, player: TShockAPI.TSPlayer, seconds: number);

    /**
     * Removes a cooldown for the provided player.
     */
    export function removeCooldown(key: string, player: TShockAPI.TSPlayer);

    /**
     * Checks whether a player is in cooldown the provided key, and returns the number of seconds
     * they are in cooldown for.  0 indicates the player is not in cooldown for the provided key.
     */
    export function checkCooldown(key: string, player: TShockAPI.TSPlayer);
}