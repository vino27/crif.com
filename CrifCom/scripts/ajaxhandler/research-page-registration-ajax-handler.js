$(document).ready(function () {
    $('#RegistrationSuccessMessage').hide();
    $('#RegistrationFailedMessage').hide();
    $('#RegistrationAlreadyExistsMessage').hide();
    $('#loader').hide();

    $('#researchPageRegistrationForm').on('submit', function (event) {
        event.preventDefault();
        var isChecked = $('#checkbox2').is(":checked");

        if (isChecked) {
            if ($(this).valid()) {
                var formdata = new FormData($(this).get(0));
                //var thankYouUrl = $('#thankYouPageTxt').val();
                //console.log(thankYouUrl);
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
                            //$('#RegistrationSuccessMessage').show();
                            //$('#RegistrationFailedMessage').hide();
                            //$('#RegistrationAlreadyExistsMessage').hide();
                            //$('#nameTxt').val('');
                            //$('#surnameTxt').val('');
                            //$('#emailTxt').val('');
                            //$('#marketTxt').selectpicker('refresh');
                            //$('#companyTxt').val('');
                            //$('#roleTxt').val('');
                            //$('#nationTxt').val('');
                            //$('#passwordTxt').val('');
                            //$('#conformTxt').val('');
                            window.location.href = result.Url;

                        } else if (result.Status == "AlreadyExists") {
                            $('#RegistrationAlreadyExistsMessage').show();
                            $('#RegistrationSuccessMessage').hide();
                            $('#RegistrationFailedMessage').hide();
                        } else {
                            $('#RegistrationSuccessMessage').hide();
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
    $('#checkbox2').change(function () {
        if ($(this).is(":checked")) {
            $("#terms-chkbx").removeClass("spot");
        }
    });
});

$(document).ready(function (e) {
    $('#RegistrationModal .close').click(function () { 
        $('#nameTxt').val('');
        $('#surnameTxt').val('');
        $('#emailTxt').val('');
        $('#marketTxt').selectpicker('refresh');
        $('#companyTxt').val('');
        $('#roleTxt').val('');
        $('#nationTxt').val('');
        $('#passwordTxt').val('');
        $('#conformTxt').val('');
        $('#RegistrationSuccessMessage').hide();
        $('#RegistrationAlreadyExistsMessage').hide();
        $('#RegistrationFailedMessage').hide();
    })
})