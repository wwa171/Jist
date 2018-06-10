/**
 * Contains VERY loose type definitions for Terraria's Objects.
 */
declare namespace Terraria {

  /** Describes the Terraria.NPC object  */
  export interface NPC {
    TypeName: string;
    aiStyle: number;
    type: number;
    defense: number;
    lifeMax: number;
    noGravity: boolean;
    noTileCollide: boolean;
    friendly: boolean;
    townNPC: boolean;
    damage: number;
    SetDefaults(type: number, scaleoverride: number): void;
  }

  /** Describes the Terraria.Item object  */
  export interface Item {
    Name: string;
    stack: number;
    maxStack: number;
    type: number;
    width: number;
    height: number;
  }
}
