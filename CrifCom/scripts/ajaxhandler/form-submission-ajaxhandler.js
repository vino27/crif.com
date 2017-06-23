$(document).ready(function () {
    $('#RegistrationSuccessMessage').hide();
    $('#RegistrationFailedMessage').hide();
    $('#RegistrationAlreadyExistsMessage').hide();
    $('#alert-reste-password-failure').hide();
    $('#alert-reset-password-success').hide();
    $('#loader').hide(); 
    $('#registrationform').on('submit', function (event) {
        event.preventDefault();
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
                        $('#loader').show();
                    },
                    success: function (result) {
                     
                        $('#loader').hide();
                        if (result.Status == "Success") {
                            window.location.href = result.Url;
                           
                        }  else if (result.Status == "AlreadyExists") {
                                $('#RegistrationAlreadyExistsMessage').show();
                              //  $('#RegistrationSuccessMessage').hide();
                                $('#RegistrationFailedMessage').hide();
                        } else if (result.Status = "AlreadyExist") {
                         
                            //$('#RegistrationSuccessMessage').hide();
                            $('#RegistrationFailedMessage').hide();
                            $('#RegistrationAlreadyExistsMessage').show();
                        } else {
                           // $('#RegistrationSuccessMessage').hide();
                            $('#RegistrationAlreadyExistsMessage').hide();
                            $('#RegistrationFailedMessage').show();
                        }
                         
                    },
                    complete: function () {
                        $('#loader').hide();
                    }
                });
            }
        } else {
           
            $("#terms-chkbx").addClass("spot");
        }
        return false;

    })

    $('#userProfile-updation').on('submit', function (event){
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
                    $('.loader').show();
                },
                success: function (result) { 
                    if (result.Status == "Success") {
                        $('#nameTxt').val('');
                        $('#surnameTxt').val('');
                        $('#emailTxt').val('');
                        $('#marketTxt').selectpicker('refresh');
                        $('#companyTxt').val('');
                        $('#roleTxt').val('');
                        $('#nationTxt').val('');
                        $('#passwordTxt').val('');
                        $('#conformTxt').val(''); 
                        $('#RegistrationFailedMessage').hide();
                        $('#RegistrationSuccessMessage').show();
                    }                
                    else { 
                        $('#RegistrationSuccessMessage').hide();                       
                        $('#RegistrationFailedMessage').show();
                    }
                },
                complete: function () {
                    $('.loader').hide();
                }
            });
        }
        return false;
    })

    $('#change-password').on('submit', function (event) {
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
                    $('.loader').show();
                },
                success: function (result) { 
                    if (result.Status == "Success") {
                    
                        $('#passwordTxt').val('');
                        $('#conformTxt').val('');
                        $('#alert-reset-password-success').show();
                        $('#alert-reste-password-failure').hide();
                    }
                    else {
                        $('#alert-reste-password-failure').show();
                        $('#alert-reset-password-success').hide();
                        //$('#RegistrationSuccessMessage').hide();
                        //$('#RegistrationFailedMessage').show();
                    }
                },
                complete: function () { 
                    $('.loader').hide();
                    
                }
            });
        }
        return false;
    }) 
    $('#checkbox1').change(function () {
        if ($(this).is(":checked")) {
            $("#terms-chkbx").removeClass("spot");
        } 
    }); 
});

