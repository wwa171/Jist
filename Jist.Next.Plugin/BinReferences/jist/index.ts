/*
This is the main module for Jist.  It gets run every time Jist
loads or reloads for the first time.

You may import other files from this index file.

For Jist's documentation, please see <DOC URL>.  For information
on how to use TypeScript, please see https://typescriptlang.org

For information about the import syntax and to import other files,
please see https://www.typescriptlang.org/docs/handbook/modules.html
*/

import * as tshock from 'tshock';
import * as tsplayer from 'tsplayer';
import * as jist from 'jist';

jist.on('GameInitialize', () => {
    jist.log('game init');
});


jist.on('ServerJoin', args => {
    const player = tshock.Players[args.Who];

    if (player) {
        jist.log(`statics ${tsplayer.Server.Name}`);
    }
})

jist.on('ServerChat', args => {
    const player = tshock.Players[args.Who];
    jist.log(`${player.Name} said some shit`);
})

jist.log("Hello world!");