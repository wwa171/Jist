/// <reference path="./jist.d.ts" />


/**
 * Imports a CLR namespace into the Jint script.
 * @param clrNamespace The CLR namespace to import
 */
declare function importNamespace(clrNamespace: string): any;

declare function require(id: string): any;