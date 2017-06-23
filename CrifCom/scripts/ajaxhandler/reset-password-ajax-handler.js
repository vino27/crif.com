$(document).ready(function () {
    $('#ResetSuccessMessage').hide();
    $('#ResetFailMessage').hide();
    $('#loader').hide();
    $('#resetPasswordForm').on('submit', function (event) {
    
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
                    $('#loader').show();
                    
                },
                success: function (result) {
                    $('#passwordTxt').val('');
                    $('#confirmPasswordTxt').val('');
                    if (result.Status == "Success") {
                        $('#ResetSuccessMessage').show();
                    }
                    else {
                        $('#ResetFailMessage').show();
                    }
                },
                complete: function () {
                    $('#loader').hide();
                }
            });
        } else {
            
        }
        return false;

    })

});