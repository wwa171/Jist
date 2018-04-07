/// <reference path="tshock.d.ts" />

declare module 'commands' {
    export type CommandCallbackFunc = (args: TShockAPI.CommandArgs) => void;
    export type OnCooldownFunc = (player: TShockAPI.TSPlayer, timeLeft: number) => void;

    /** Describes a TShock command  */
    export interface JistCommand {
        /** The name of the command */
        name: string;

        /** String array of permissions required to execute this command.  If not provided, anyone may run this command. */
        permissions?: string[];

        /** (optional) Flag indicating whether to remove existing commands before adding this one. True by default */
        removeExisting?: boolean;
    }

    /**
     * Adds a TShock command which will execute a JavaScript function.
     * @param commandName The name of the command
     * @param permissions A string array of permissions required to execute this command
     * @param callback The callback to run when this command is executed by a player or the console.
     */
    export function addCommand(command: JistCommand, callback: CommandCallbackFunc): void;

    /**
     * Removes a command by its name.
     * @param name The name of the command to remove.
     */
    export function removeCommand(name: string): number;
}