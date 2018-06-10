/// <reference path="tshock.d.ts" />

/**
 * The Jist JavaScript API.
 */
declare namespace Jist {

    export type HookHandler<T extends EventArgs = EventArgs> = (args: T) => void;
    export type HookType = keyof JistHooks;

    export interface JistHooks {
        GameInitialize;
        ServerJoin;
        ServerLeave;
        ServerChat;
        NpcSpawn;
    }

    export interface EventArgs {
    }

    export interface HandledEventArgs extends EventArgs {
        Handled: boolean;
    }

    export interface ServerJoinEventArgs extends HandledEventArgs {
        Who: number;
    }

    export interface ServerLeaveEventArgs extends HandledEventArgs {
        Who: number;
    }

    export interface ServerChatEventArgs extends HandledEventArgs {
        Who: number;
        Text: string;
        CommandId: any;
    }

    export interface NpcSpawnEventArgs extends HandledEventArgs {
        NpcId: number;
    }

    export interface NpcKilledEventArgs extends HandledEventArgs {
        NPC: any; //TODO: Type terraria.NPC
    }

    /**
     * Logs a message in the TShock console
     */
    export function log(...args: any[]): void;

    /**
     * Executes the provided command, optionally by the specified player.
     * @param command The text of the command to execute.
     * @param player (optional) a TSPlayer to execute the command as.  If null,
     * the command is executed under the server account.
     */
    export function executeCommand(command: string, player: TShockAPI.TSPlayer | null);

    export function on(type: HookType, callback: HookHandler);
    export function on(type: 'ServerJoin', callback: HookHandler<ServerJoinEventArgs>);
    export function on(type: 'ServerLeave', callback: HookHandler<ServerLeaveEventArgs>);
    export function on(type: 'ServerChat', callback: HookHandler<ServerChatEventArgs>);
    export function on(type: 'NpcSpawn', callback: HookHandler<NpcSpawnEventArgs>);
    export function on(type: 'NpcKilled', callback: HookHandler<NpcKilledEventArgs>);

    /** Generates a random number between the ranges specified, exclusive. */
    export function random(from: number, to: number): number;

    /** Generates a random number between the ranges specified, inclusive. */
    export function randomInclusive(from: number, to: number): number;

    /**
     * Gets a random clear tile within the specified range.
     * Mapped to TShockAPI.Utils.GetRandomClearTileWithInRange.
     * 
     * @param x The starting X coordinate
     * @param y The starting Y coordinate
     * @param tileXRange The X range
     * @param tileYRange The Y range
     * 
     * @returns a Point object containing the X and Y coordinates of the first clear tile within
     * the parameters specified.
     */
    export function getRandomClearTileWithinRange(x: number, y: number, tileXRange: number, tileYRange: number): OTAPI.Point;
}

declare module 'jist' {
    export = Jist;
}