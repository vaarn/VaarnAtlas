var GetURL = {
    
    GetURLFromPage: function () {
        var returnStr = (window.location != window.parent.location) ? document.referrer : document.location.href;
        var buffer = _malloc(lengthBytesUTF8(returnStr) + 1);
		stringToUTF8(returnStr, buffer, returnStr.length + 1);
        return buffer;
    }
};

mergeInto(LibraryManager.library, GetURL);