$(document).ready(function () { 
    $('.forgot-email-error').hide();
    $('.forgot-invalid-email-error').hide();
    $('.resetSuccessMessage').hide();
    $('.loader-reset-password').hide();
    $('.reset-failed-message').hide();
    $('#forgotPasswordForm').on('submit', function (event) {  
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
                  //  $('.loader-reset-password').show();
                },
                success: function (result) {
                    if (result.Status == "Success") {
                        $('.reset-failed-message').hide();
                        $('.forgot-email-error').hide();
                        $('.resetSuccessMessage').show();
                        $('.loader-reset-password').hide();
                        $('.forgot-invalid-email-error').hide();
                    } else {
                        $('.reset-failed-message').show();
                        $('.forgot-email-error').hide();
                        $('.resetSuccessMessage').hide();
                        $('.loader-reset-password').hide();
                        $('.forgot-invalid-email-error').hide();
                    }
                   
                },
                complete: function () {
                    $('.forgot-email-error').hide();
                    $('.loader-reset-password').hide();
                    $('.forgot-invalid-email-error').hide();
                }
            });
        } else {
            if($('#emailLoginTxt').val().trim()==""){
                $('.forgot-email-error').show();
                $('.forgot-invalid-email-error').hide();
            } else { 
                $('.forgot-email-error').hide();
                $('.forgot-invalid-email-error').show(); 
            }
           
            $('.loader-reset-password').hide();
        }
        return false;

    })
   
});