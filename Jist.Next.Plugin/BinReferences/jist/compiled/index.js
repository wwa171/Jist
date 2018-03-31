"use strict";
/*
This is the main module for Jist.  It gets run every time Jist
loads or reloads for the first time.

You may import other files from this index file.

For Jist's documentation, please see <DOC URL>.  For information
on how to use TypeScript, please see https://typescriptlang.org

For information about the import syntax and to import other files,
please see https://www.typescriptlang.org/docs/handbook/modules.html
*/
Object.defineProperty(exports, "__esModule", { value: true });
var tshock = require("tshock");
var jist = require("jist");
jist.on('GameInitialize', function () {
    jist.log('game init');
});
jist.on('ServerJoin', function (args) {
    var player = tshock.Players[args.Who];
    if (player) {
        jist.log("jist: " + player.Name + " has joined.  Testing hooks.");
    }
});
jist.log("Hello world!");
