/// <reference path="GlobalValues.ts" />

module vdb.functions {

    export function isNullOrWhiteSpace(str: string) {

        if (str == null || str.length == 0)
            return true;

        return !(/\S/.test(str));

    }

    export function mapAbsoluteUrl(relative: string) {

        return mergeUrls(vdb.values.baseAddress, relative);

    };

    export function mapFullUrl(relative: string) {

        return mergeUrls(vdb.values.hostAddress, relative);

    };

    export function mergeUrls(base: string, relative: string) {
        
        if (base.charAt(base.length - 1) == "/" && relative.charAt(0) == "/")
            return base + relative.substr(1);

        if (base.charAt(base.length - 1) == "/" && relative.charAt(0) != "/")
            return base + relative;

        if (base.charAt(base.length - 1) != "/" && relative.charAt(0) == "/")
            return base + relative;
        
        return base + "/" + relative;

    }

}