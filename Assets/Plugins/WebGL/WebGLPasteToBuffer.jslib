mergeInto(LibraryManager.library, {
	CopyToClipboard: function (arg){
		var tempInput = document.createElement("input");
      		tempInput.value = Pointer_stringify(arg);
      		document.body.appendChild(tempInput);
      		tempInput.select();
      		document.execCommand("copy");
      		document.body.removeChild(tempInput); 
                 }
});