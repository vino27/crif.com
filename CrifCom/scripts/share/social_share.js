function share_print(){
	window.print();
    return  false;
}

function share_email(){
	var subject = $("#subjectForEmailSharing").val();
	if(subject=="")
	{
		subject = "Crif Credit Solutions";
	}
	document.location.href = "mailto:?subject=" + encodeURIComponent(subject) + "&body="  + encodeURIComponent(window.location);
    return  false;
}

function share_fb(){
	var sharerURL = "https://www.facebook.com/sharer/sharer.php?s=100&p[url]=" + encodeURIComponent(window.location);
    window.open(sharerURL, 'facebookShareDialog', 'width=626,height=436'); 
    return  false;
}

function share_twitter(){
	var sharerURL = "https://twitter.com/share?url=" + encodeURIComponent(window.location);
    window.open(sharerURL, 'twitterShareDialog', 'width=626,height=436'); 
    return  false;
}

function share_google(){
	var sharerURL = "https://plus.google.com/share?url=" + encodeURIComponent(window.location);
    window.open(sharerURL, 'googlePlusDialog', 'width=626,height=436'); 
    return  false;
}

function share_linkedin(){
	var sharerURL = "https://www.linkedin.com/cws/share?url=" + encodeURIComponent(window.location);
    window.open(sharerURL, 'linkedinShareDialog', 'width=626,height=436'); 
    return  false;
}
