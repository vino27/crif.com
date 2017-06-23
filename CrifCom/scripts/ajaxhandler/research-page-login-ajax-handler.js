$(document).ready(function () {
    $('.login-invalid').hide();
    $('.researchuserNameTxt-error').hide();
    $('.researchpasswordTxthome-error').hide();
    $('.loader-sub-login').hide();
    $('#loginFormResearch').on('submit', function (event) { 
        event.preventDefault();
        if ($(this).valid()) {
            var formdata = new FormData($(this).get(0));
            $.ajax({
                url: this.action,
                type: this.method,
                data: formdata,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    $('.loader-sub-login').show();
                },
                success: function (result) {
                    if (result.Status == "Success") {
                        $('.login-invalid').hide(); 
                        location.reload();
                        $('.loader-sub-login').hide();
                    }
                    else if (result.Status == "Pending") {
                        $('.login-invalid').show();
                        $('.loader-sub-login').hide();
                    }
                    else {
                        $('.login-invalid').show();
                        $('.loader-sub-login').hide();
                    }
                },
                complete: function () {
                    $('.loader-sub-login').hide();
                }
            });
        } else {
            //$('.login-invalid').hide();
            //if ($('#userNameTxtResearch').val().trim() == "") {
            //    $('.researchuserNameTxt-error').show();
            //} else {
            //    $('.researchuserNameTxt-error').hide();
            //}
            //if ($('#passwordTxthomeResearch').val().trim() == "") {
            //    $('.researchpasswordTxthome-error').show();
            //} else {
            //    ('.researchpasswordTxthome-error').hide();
            //}
        }
        return false;

    })
   
});
$(document).ready(function () {
    $('.forgot-email-error-research').hide();
    $('.resetSuccessMessage').hide();
    $('.reset-failed-message').hide();
    $('.loader-sub-password').hide();
    $('.forgot-invalid-email-error').hide();
    $('#forgotPasswordFormResearch').on('submit', function (event) { 
        event.preventDefault();
        if ($(this).valid()) { 
            var formdata = new FormData($(this).get(0));
            $.ajax({
                url: this.action,
                type: this.method,
                data: formdata,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    //$('.loader-sub-password').show();
                },
                success: function (result) {
                    if (result.Status == "Success") {
                        $('.reset-failed-message').hide();
                        $('.forgot-email-error-research').hide();
                        $('.resetSuccessMessage').show();
                        $('.loader-sub-password').hide();
                        $('.forgot-invalid-email-error').hide();
                    } else {
                        $('.reset-failed-message').show();
                        $('.forgot-email-error-research').hide();
                        $('.resetSuccessMessage').hide();
                        $('.loader-sub-password').hide();
                        $('.forgot-invalid-email-error').hide();
                    }
                   
                },
                complete: function () { 
                }
            });
        } else {
            if ($('#emailLoginTxt').val().trim() == "") {
                $('.forgot-email-error-research').show();
                $('.forgot-invalid-email-error-research').hide();
            } else {
                $('.forgot-email-error-research').show();
                $('.forgot-invalid-email-error-research').show();
            } 
            //$('.loader-sub-password').hide();
        }
        return false;

    })

});