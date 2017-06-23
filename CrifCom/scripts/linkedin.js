

$(document).ready(function() { 
    $.getScript("https://platform.linkedin.com/in.js?async=true", function success() { 
        IN.init({ 
           api_key: $("#hdApiClientId").val(),
            authorize: true,
			onLoad : onLinkedInLoad
        }); 
    }); 
});

function liAuth() {
    IN.User.authorize(function () {
        onLinkedInAuth();
    });
}

// 2. Runs when the JavaScript framework is loaded
function onLinkedInLoad() {
    IN.Event.on(IN, "auth", getProfileData);
}

// 2. Runs when the viewer has authenticated
function onLinkedInAuth() {	
	IN.API.Profile("me")
              .fields(["id", "firstName", "lastName", "industry", "positions", "emailAddress", "location"]).result(display);
}

// 2. Runs when the Profile() API call returns successfully
function display(result) {
	var details = result.values[0];
              var countryname = getCountryName(details.location.country.code.toUpperCase()); 
              $('#nameTxt').val(details.firstName);
              $('#surnameTxt').val(details.lastName);
              $('#emailTxt').val(details.emailAddress);
              $('#companyTxt').val(details.positions.values[0].company.name);
              $('#roleTxt').val(details.positions.values[0].title);
              $('#nationTxt').val(countryname);
}
 // Handle the successful return from the API call
    function onSuccess(data) {
        console.log(data);
    }

    // Handle an error response from the API call
    function onError(error) {
        console.log(error);
    }

    // Use the API call wrapper to request the member's basic profile data
    function getProfileData() {
        IN.API.Raw("/people/~").result(onSuccess).error(onError);
    }
