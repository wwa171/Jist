/// <reference path="otapi.d.ts" />

declare namespace TShockAPI {
  class TShock {
    static Players: TSPlayer[];
    static Groups: GroupManager;
    static Utils: Utils;
  }

  class Utils {
    GetBuffByName(name: string): number[];
    GetBuffDescription(id: string): string;
    GetBuffName(id: string): string;

    [fn: string]: any;
  }

  class User {
    ID: number;
    Name: string;
    Password: string;
    UUID: string;
    Group: string;
    Registered: string;
    LastAccessed: string;
    KnownIps: string;

    VerifyPassword(password: string): boolean;

    CreateBCryptHash(password: string): void;
  }

  class Commands {
    static ChatCommands: Command[];
    static Specifier: string;
    static SilentSpecifier: string;

    static HandleCommand(player: TSPlayer, command: string): boolean;
  }

  class Command {
    constructor(permissions: string | string[], handler: CommandDelegate, ...names: string[]);
  }

  type CommandDelegate = (args: CommandArgs) => void;

  class CommandArgs {
    Message: string;
    Player: TSPlayer;
    Silent: boolean;
    Parameters: string[];
  }

  class Region {
    ID: number;
    Area: OTAPI.Rectangle; //TODO
    Name: string;
    Owner: string;
    DisableBuild: boolean;
    WorldID: string;
    AllowedIDs: number[];
    AllowedGroups: string[];
    Z: number;

    /**
     * Checks if a given (x, y) coordinate is in the region's area
     */
    InArea(point: OTAPI.Rectangle): boolean;

    /**
     * Checks if a given player has permission to build in the region
     * @param ply Player to check permissions with
     */
    HasPermissionToBuildInRegion(ply: TSPlayer): boolean;

    /**
     * Sets the user IDs which are allowed to use the region
     */
    SetAllowedIDs(ids: string): void;

	/** Sets the group names which are allowed to use the region */
    SetAllowedGroups(groups: string): void;

    /**
     * removes a user's access to the region
     * @param id User ID to remove
     */
    RemoveID(id: number): boolean;

    /**
     * Removes a group's access to the region
     */
    RemoveGroup(groupName: string);
  }

  class Point {
    static Zero: Point;

    X: number;
    Y: number;
  }

  enum EditAction {
    KillTile = 0,
    PlaceTile,
    KillWall,
    PlaceWall,
    KillTileNoItem,
    PlaceWire,
    KillWire,
    PoundTile,
    PlaceActuator,
    KillActuator,
    PlaceWire2,
    KillWire2,
    PlaceWire3,
    KillWire3,
    SlopeTile,
    FrameTrack,
    PlaceWire4,
    KillWire4
  }

  class Group {
    permissions: string[];
    negatedpermissions: string[];

    R: number;
    G: number;
    B: number;

    ChatColor: string;
    ParentName: string;
    Suffix: string;
    Prefix: string;
    Parent: Group;
    Name: string;
    TotalPermissions: string[];
    Permissions: string;

    AddPermission(permission: string): void;
    AssignTo(otherGroup: Group): void;
    HasPermission(permission: string): boolean;
    NegatePermission(permission: string): void;
    RemovePermission(permission: string): void;
    SetPermission(permission: string[]): void;
  }

  class GroupManager {
    public groups: Group[];

    AddGroup(name: string, parentname: string, permissions: string, chatcolor: string): void;

    AddPermissions(name: string, permissions: string[]): string;

    DeleteGroup(name: string, exceptions: boolean): string;

    DeletePermissions(name: string, permissions: string[]): string;

    GetGroupByName(name: string): Group;

    GroupExists(group: string): boolean;

    LoadPermission(): void;

    UpdateGroup(name: string, parentname: string, permissions: string, chatcolor: string, suffix: string, prefix: string): void;
  }

  class TSPlayer {
    static readonly Server: TSPlayer;
    static readonly All: TSPlayer;

    static FindByNameOrID(plr: string): TSPlayer[];

    TileKillThresold: number;

    TilePlaceThreshold: number;

    TileLiquidThreshold: number;

    PaintThreshold: number;

    ProjectileThreshold: number;

    HealOtherThreshold: number;

    Index: number;

    Country: string;

    Difficult: number;

    IsDisabledForSSC: boolean;

    IsDisabledForStackDetection: boolean;

    IsDisabledForBannedWearable: boolean;

    IsDisabledPendingTrashRemoval: boolean;

    IsLoggedIn: boolean;

    User: User | undefined;

    Group: Group;

    /** Whether the player is muted or not. */
    mute: boolean;

    IsBouncerThrottled(): boolean;

    /** Easy check if a player has any of IsDisabledForSSC, IsDisabledForStackDetection, IsDisabledForBannedWearable, or IsDisabledPendingTrashRemoval set. Or if they're not logged in and a login is required. */
    IsBeingDisabled(): boolean;

    /**
     * Checks to see if a player has hacked item stacks in their inventory, and messages them as it checks.
     */
    HasHackedItemStacks(shouldWarnPlayer?: boolean): boolean;

    PlayerData: any; // TODO: type

    /** Whether the player needs to specify a password upon connection( either server or user account ). */
    RequiresPassword: boolean;

    /**
     * Checks if a player is in range of a given tile if range checks are enabled.
     */
    IsInRange(x: number, y: number, range?: number): boolean;

    /**
     * Determines if the player can build on a given point.
     */
    HasBuildPermission(x: number, y: number, shouldWarnPlayer?: boolean);

    /**
     * Checks if a player can place ice, and if they can, tracks ice placements and removals.
     * @param x The x coordinate they want to paint at.
     * @param y The y coordinate they want to paint at.
     */
    HasPaintPermission(x: number, y: number): boolean;

    /**
     * Checks if a player can place ice, and if they can, tracks ice placements and removals.
     * @param x The x coordinate of the suspected ice block.
     * @param y The y coordinate of the suspected ice block.
     * @param tileType The tile type of the suspected ice block.
     * @param action The EditAction on the suspected ice block.
     */
    HasModifiedIceSuccessfully(
      x: number,
      y: number,
      tileType: number,
      action: EditAction
    );

    /** A list of points where ice tiles have been placed. */
    IceTiles: Point[];

    /**
     * The time in ms when the player has logged in.
     */
    LoginMS: number;

    /**
     * Player cant die, unless onehit
     */
    GodMode: boolean;

    /**
     * The last projectile type this player tried to kill.
     */
    LastKilledProjectile: number;

    /**
     * The current region this player is in, or null if none.
     */
    CurrentRegion: Region | null;

    /**
     * Checks if the player is active and not pending termination.
     */
    ConnectionAlive: boolean;

    /**
     * Gets the item that the player is currently holding.
     */
    SelectedItem: any; //TODO

    /**
     * Gets the player's Client State.
     */
    State: number;

    /**
     * Gets the player's UUID.
     */
    UUID: string;

    /**
     * Gets the player's IP.
     */
    IP: string;

    /**
     * Gets the player's accessories.
     */
    Accessories: any[]; //TODO: type Item

    /**
     * Saves the player's inventory to SSC
     */
    SaveServerCharacter(): boolean;

    /**
     * Sends the players server side character to client
     */
    SendServerCharacter(): boolean;

    /**
     * Gets the Terraria Player object associated with the player.
     */
    TPlayer: any; // TODO: Type Terraria.Player

    /**
     * Gets the player's name.
     */
    readonly Name: string;

    /**
     * Gets the player's active state.
     */
    readonly Active: boolean;

    /**
     * Gets the player's team.
     */
    readonly Team: number;

    /**
     * Gets the player's X coordinate.
     */
    readonly X: number;

    /**
     * Gets the player's Y coordinate.
     */
    readonly Y: number;

    /**
     * Player X coordinate divided by 16. Supposed X world coordinate.
     */
    readonly TileX: number;

    /**
     * Player Y cooridnate divided by 16. Supposed Y world coordinate.
     */
    readonly TileY: number;

    /**
     * Checks if the player has any inventory slots available.
     */
    readonly InventorySlotAvailable: boolean;


    /**
     * Determines whether the player's storage contains the given key.
     */
    ContainsData(key: string): boolean;

    /**
     * Determines whether the player's storage contains the given key.
     * @param key Key to test.
     */
    GetData<T = any>(key: string): T;

    /**
     * Stores an object on this player, accessible with the given key.
     * @param key Type of the object being stored.
     * @param data Object to store.
     */
    SetData(key: string, data: any): void;

    /**
     * Removes the stored object associated with the given key.
     * @param key Key with which to access the object.
     */
    RemoveData(key: string): void;

    /**
     * Logs the player out of an account.
     */
    Logout(): void;

    constructor(index: number);

    /**
     * Disconnects the player from the server.
     * @param reason 
     */
    Disconnect(reason: string): void;

    /**
     * Teleports the player to the given coordinates in the world.
     * @param x The X coordinate.
     * @param y The Y coordinate.
     * @param style The teleportation style.
     */
    Teleport(x: number, y: number, style?: number): boolean;

    /**
     * Heals the player.
     * @param health Heal health amount.
     */
    Heal(health?: number): void;


    /**
     * Spawns the player at his spawn point.
     */
    Spawn(): void;
    Spawn(tileX: number, tileY: number): void;

    /**
     * Checks to see if this player object has access rights to a given projectile. Used by projectile bans.
     * @param index The projectile index from Main.projectiles (NOT from a packet directly).
     * @param type The type of projectile, from Main.projectiles.
     */
    HasProjectilePermission(index: number, type: number): boolean;

    /**
     * Removes the projectile with the given index and owner.
     * @param index The projectile's index.
     * @param type The projectile's owner.
     */
    RemoveProjectile(index: number, type: number): void;

    /**
     * Sends a tile square at a location with a given size.
     * 
     * Typically used to revert changes by Bouncer through sending the "old"
     * version of modified data back to a client.
     * 
     * Prevents desync issues. 
     * @param x The x coordinate to send.
     * @param y The y coordinate to send.
     * @param size The size square set of tiles to send.
     */
    SendTileSquare(x: number, y: number, size?: number): boolean;

    /**
     * Gives an item to the player. Includes banned item spawn prevention to check if the player can spawn the item.
     * @param type The item ID.
     * @param name The item name.
     * @param stack The item stack.
     * @param prefix The item prefix.
     */
    GiveItemCheck(type: number, name: string, stack: number, prefix?: number): boolean;

    /**
     * Gives an item to the player.
     * @param type The item ID.
     * @param stack The item stack.
     * @param prefix The item prefix.
     */
    GiveItem(type: number, stack: number, prefix?: number): void;

    /**
     * Sends an information message to the player.
     * @param message The message.
     */
    SendInfoMessage(message: string): void;

    /**
     * Sends an information message to the player.
     */
    SendInfoMessage(format: string, ...args: object[]): void;

    /**
     * Sends a success message to the player.
     */
    SendSuccessMessage(msg: string): void;
    SendSuccessMessage(format: string, ...args: any[]): void;

    /**
     * Sends a warning message to the player.
     */
    SendWarningMessage(msg: string): void;
    SendWarningMessage(format: string, ...args: any[]): void;

    /**
     * Sends an error message to the player.
     */
    SendErrorMessage(msg: string): void;
    SendErrorMessage(format: string, ...args: any[]): void;

    /**
     * Sends a message with the specified color.
     * @param msg The message.
     * @param color The message color.
     */
    SendMessage(msg: string, color: any); //TODO: Type XNA Color

    /**
     * Sends a message with the specified color.
     * @param msg The message.
     */
    SendMessage(msg: string, r: number, g: number, b: number);

    /**
     * Sends a message to the player with the specified RGB color.
     * @param msg The message.
     * @param r The amount of red color to factor in. Max: 255.
     * @param g The amount of green color to factor in. Max: 255.
     * @param b The amount of blue color to factor in. Max: 255.
     * @param ply The player who receives the message.
     */
    SendMessageFromPlayer(msg: string, r: number, g: number, b: number, ply: number): void;

    /**
     * Sends the text of a given file to the player. Replacement of %map% and %players% if in the file.
     */
    SendFileAsTextMessage(file: string): void;

    /**
     * Wounds the player with the given damage.
     * @param damage The amount of damage the player will take.
     */
    DamagePlayer(damage: number): void;

    /**
     * Kills the player.
     */
    KillPlayer(): void;

    /**
     * Kills the player.
     * @param team 
     */
    SetTeam(team: number): void;

    readonly ActiveChest: number;

    /**
     * Disconnects this player from the server with a reason.
     * @param reason The reason to display to the user and to the server on kick.
     * @param force If the kick should happen regardless of immunity to kick permissions.
     * @param silent If no message should be broadcasted to the server.
     * @param adminUserName The originator of the kick, for display purposes.
     * @param saveSSI If the player's server side character should be saved on kick.
     */
    Kick(reason: string, force?: boolean, silent?: boolean, adminUserName?: string, saveSSI?: boolean): boolean;

    /**
     * Bans and disconnects the player from the server.
     * @param reason The reason to be displayed to the server.
     * @param force If the ban should bypass immunity to ban checks.
     * @param adminUserName The player who initiated the ban.
     */
    Ban(reason: string, force?: boolean, adminUserName?: string);

    /**
     * Applies a buff to the player.
     * @param type The buff type.
     * @param time The buff duration.
     */
    SetBuff(type: number, time?: number, bypass?: boolean): void;

    SendData(msgType: number, text?: string, number?: number, number2?: number, number3?: number, number4?: number, number5?: number): void;

    SendDataFromPlayer(msgType: number, ply: number, text?: string, number?: number, number2?: number, number3?: number, number4?: number, number5?: number): void;

    /**
     * Checks to see if a player has a specific permission.
     * @param permission The permission to check.
     */
    HasPermission(operand: string | any): boolean;
  }
}

declare module "tshock" {
  let tshock: typeof TShockAPI.TShock;
  export = tshock;
}

declare module "tsplayer" {
  let tsplayer: typeof TShockAPI.TSPlayer;
  export = tsplayer;
}