document.addEventListener('DOMContentLoaded', function() {
	document.getElementsByTagName("span")[0].innerHTML=location.search.split('id=')[1];
}, false);
