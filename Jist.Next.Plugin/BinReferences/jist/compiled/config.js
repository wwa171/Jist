"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var jist = require("jist");
function maddog() {
    jist.log("Mad " + exports.world_regen_interval);
}
exports.maddog = maddog;
/** This is the world regen interval in hours */
exports.world_regen_interval = 12;
exports.ptsPlayers = [];
