// hack for firefox 45
if((function(){
    var ua= navigator.userAgent, tem, 
    M= ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
    if(/trident/i.test(M[1])){
        tem=  /\brv[ :]+(\d+)/g.exec(ua) || [];
        return 'IE '+(tem[1] || '');
    }
    if(M[1]=== 'Chrome'){
        tem= ua.match(/\b(OPR|Edge)\/(\d+)/);
        if(tem!= null) return tem.slice(1).join(' ').replace('OPR', 'Opera');
    }
    M= M[2]? [M[1], M[2]]: [navigator.appName, navigator.appVersion, '-?'];
    if((tem= ua.match(/version\/(\d+)/i))!= null) M.splice(1, 1, tem[1]);
    return M.join(' ');
})() == "Firefox 45") $('.nav-tabs .slick-track').addClass('fix-tab-pos');

$(document).ready(function () {
    $('#loader-form-submision').hide();
    $('#loader').hide();
    var details;

    $('#forgot-password').click(function (event) {
        event.preventDefault();
        $('#login-form-main').hide();
        $('#reset-password').show()
        $('.resetSuccessMessage').hide();
        $('.reset-failed-message').hide();
        $("#forgotPasswordForm").clearValidation();
        cleartextbox();
    });
    $('#forgot-password-research').click(function (event) {
        event.preventDefault();
        $('#login-form-main-research').hide();
        $('#reset-password-research').show();
        $('.resetSuccessMessage').hide();
        clearmodaltextbox();
        $("#forgotPasswordFormResearch").clearValidation();

    });
    $('.popup-trigger-button').click(function (event) {
        event.preventDefault();
        $('#login-form-main').show();
        $('#reset-password').hide()
        $('.resetSuccessMessage').hide();
        $('.reset-failed-message').hide();
        cleartextbox();
        //$("#loginForm").clearValidation();
    });
    function cleartextbox() {
        $('#userNameTxt').val('');
        $('#passwordTxthome').val('');
        $('.login-error').hide();
        $('.invalid-login').hide();
        $('#emailLoginTxt').val('');
    }
    $(' .login-modal-button').click(function (event) {
        event.preventDefault();
        $('#login-form-main-research').show();
        $('#reset-password-research').hide();
        $('.login-error').hide();
        clearmodaltextbox();
        $("#loginFormResearch").clearValidation();
    });
    function clearmodaltextbox() {
        $('#userNameTxtResearch').val('');
        $('#passwordTxthomeResearch').val('');
        $('#emailLoginTxtResearch').val("");
        $('#login-invalid').hide();
        $('.resetSuccessMessage').hide();
        $('.reset-failed-message').hide();

    }
    $('.register-modal-button').click(function () {
        $("#researchPageRegistrationForm").clearValidation();
        $("#terms-chkbx").removeClass("spot");
        $('#telephoneTxt').val('');
    });
    $('.contact-modal-button').click(function () {
        $("#ContactModalForm").clearValidation();
        $("#terms-chkbx").removeClass("spot");
    });
    $.fn.clearValidation = function () { var v = $(this).validate(); $('[name]', this).each(function () { v.successList.push(this); v.showErrors(); }); v.resetForm(); v.reset(); };
    /*Get Member Details*/
    $('.form-download').click(function (e) {
        var currentMediaId = $(this).attr("data-id");
        $('#hidden-url').val(currentMediaId);
        $("#terms-chkbx").removeClass("spot");
        var member = $('.member-data').val();
		var email = $('.member-email').val(); 
        $.ajax({
            url: '/umbraco/surface/Registration/GetMemberByID/',
            data: { "member": member,"membermail": email },
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
            },
            success: function (result) {
                if (result.Status == "Success") {
                    $('#txtFirstName').val(result.Name);
                    $('#txtLastName').val(result.SurName);
                    $('#txtCompany').val(result.Company);
                    $('#txtRole').val(result.Role);
                    $('#txtEmail').val(result.Email);
                    $('#txtTelephone').val(result.Telephone);
                } else {
                    // $('#downloadFormModal').modal('hide');
                }
            },
            complete: function () {
            }
        });
    });
	/*researchPageRegistrationForm*/
	$('#researchPageRegistrationForm').on('submit', function (event) {
        event.preventDefault();
		var validateCaptcha = grecaptcha.getResponse();
        if (validateCaptcha.length == 0) {
            var errtext=$("#captchaerrordict").val();
            $("#captchaerror").text(errtext);
            return false;
        }
		$("#captchaerror").text('');
        var isChecked = $('#checkbox2').is(":checked");
        if (isChecked) {
            if ($(this).valid()) {
                var formdata = new FormData($(this).get(0));
                $.ajax({
                    url: this.action,
                    type: this.method,
                    data: formdata,
                    processData: false,
                    contentType: false,
                    beforeSend: function () {
                        $('#loader-form-submision').show();
                    },
                    success: function (result) {
                        if (result.Status == "Success") {
                            if (result.IsMedia == 1) {
                                window.location.href = "/download-confirmation/?s1=" + result.NodeId + "&s2=" + result.MediaId + "";
                            }
                            else {
                                window.location.href = "/download-confirmation/?s1=" + result.NodeId + "";
                            }
                            //$('#loader-form-submision').hide(); 
                            //$('#downloadFormModal').modal('hide');  
                        }
                        else {
                            $('#loader-form-submision').hide();
                            $('#researchPageRegistrationForm').modal('hide');
                        }
                    },
                    complete: function () {
                        $('#loader-form-submision').hide();
                        $('#researchPageRegistrationForm').modal('hide');
                        //$('.hidden-a-tag')[0].click();
                    }
                });
            }
        } else {
            $("#terms-chkbx").addClass("spot");
        }
        return false;

    })
    /*Document Submit*/
    $('#documentDownloadForm').on('submit', function (event) {
        event.preventDefault();
		var validateCaptcha = grecaptcha.getResponse();
        if (validateCaptcha.length == 0) {
            var errtext=$("#captchaerrordict").val();
            $("#captchaerror").text(errtext);
            return false;
        }
		$("#captchaerror").text('');
        var isChecked = $('#checkbox1').is(":checked");
        if (isChecked) {
            if ($(this).valid()) {
                var formdata = new FormData($(this).get(0));
                $.ajax({
                    url: this.action,
                    type: this.method,
                    data: formdata,
                    processData: false,
                    contentType: false,
                    beforeSend: function () {
                        $('#loader-form-submision').show();
                    },
                    success: function (result) {
                        if (result.Status == "Success") {
                            if (result.IsMedia == 1) {
                                window.location.href = "/download-confirmation/?s1=" + result.NodeId + "&s2=" + result.MediaId + "";
                            }
                            else {
                                window.location.href = "/download-confirmation/?s1=" + result.NodeId + "";
                            }
                            //$('#loader-form-submision').hide(); 
                            //$('#downloadFormModal').modal('hide');  
                        }
                        else {
                            $('#loader-form-submision').hide();
                            $('#downloadFormModal').modal('hide');
                        }
                    },
                    complete: function () {
                        $('#loader-form-submision').hide();
                        $('#downloadFormModal').modal('hide');
                        //$('.hidden-a-tag')[0].click();
                    }
                });
            }
        } else {
            $("#terms-chkbx").addClass("spot");
        }
        return false;

    })
	/*racaptcha validation*/
    $('#ContactForm').submit(function (event) {
		var validateCaptcha = grecaptcha.getResponse();
        if (validateCaptcha.length == 0) {
			var errtext=$("#captchaerrordict").val();
            $("#captchaerror").text(errtext);
            return false;
        }
        else {
			$("#captchaerror").text('');
            return true;
        }
    });
    $('#registrationform').submit(function (event) {
        var validateCaptcha = grecaptcha.getResponse();
        if (validateCaptcha.length == 0) {
            var errtext=$("#captchaerrordict").val();
            $("#captchaerror").text(errtext);
            return false;
        }
        else {
            return true;
			$("#captchaerror").text('');
        }
    });
    /*Show ActivationMessage*/
    $('#activationModal').modal('show');/*keep this in bottom */
    /*capitalize input string on blur*/
    $(function () {
		$('#registrationform input[type="text"],#researchPageRegistrationForm input[type="text"],#ContactForm input[type="text"], #ContactModalForm input[type="text"]').blur(function () {
            this.value = this.value.charAt(0).toUpperCase() + this.value.slice(1);
        });
    });
    // trims away the time part in the date wherever its found
    $(".date").each(function (i) {
        $(this).text().indexOf(":") != -1 ? $(this).text($(this).text().slice(0, $(this).text().length - 8)) : '';
    });
    
    window.clearListCookies =function() {
        var cookies = document.cookie.split(";");
        if (!document.cookie.match(/^(.*;)?cb-enabled=[^;]+(.*)?$/)) {
            for (var i = 0; i < cookies.length; i++) {
                var spcook = cookies[i].split("=");
                createCookie(spcook[0], "", -1);
            }
        }
    }
        function createCookie(name, value, days) {
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                var expires = "; expires=" + date.toGMTString();
            }
            else var expires = "";
            document.cookie = name + "=" + value + expires + "; path=/";
        }
	// fix for sliding tabs
    if($('.nav-tabs li.slick-slide').length){
		fixSlidingTabPos();
		$(window).on('resize', function(){
			$('.nav-tabs .slick-track').removeClass('fix-tab-pos');
			fixSlidingTabPos();
		});
		// helper function
		function fixSlidingTabPos(){
			$('.nav-tabs li.slick-slide')
				.toArray()
				.reduce(function(total, item){
					return (total === parseInt(total, 10)) ? 
						total+ $(item).outerWidth() : 
						$(total).outerWidth()+ $(item).outerWidth()
				},0) 
			<= $('.nav-tabs .slick-list').width() ?  
				$('.nav-tabs .slick-track').addClass('fix-tab-pos') : ''; 
		}
	}
});
$(window).unload(function() { $('select option').remove(); });
