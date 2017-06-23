$(document).ready(function () {
    $('#invalid-login').hide();
    $('.userNameTxt-error').hide();
    $('.passwordTxthome-error').hide();
    $('.loader-login').hide();
    $('#loginForm').on('submit', function (event) {
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
                    $('.loader-login').show();
                },
                success: function (result) {
                    if (result.Status == "Success") {
                        $('#invalid-login').hide(); 
                        location.reload();
                        $('.loader-login').hide()
                    }
                    else if (result.Status == "Pending") {
                        $('#invalid-login').show();
                        $('.loader-login').hide()
                    }
                    else {
                        $('#invalid-login').show();
                        $('.loader-login').hide()
                    }
                },
                complete: function () {
                    // $('.loader-overlay').hide();
                    $('.loader-login').hide()
                }
            });
        } else {
            //$('#invalid-login').hide();
            //if ($('#userNameTxt').val().trim() == "") {
            //    $('.userNameTxt-error').show();
            //} else {
            //    $('.userNameTxt-error').hide();
            //}
            //if ($('#passwordTxthome').val().trim() == "") {
            //    $('.passwordTxthome-error').show();
            //} else {
            //    ('.passwordTxthome-error').hide();
            //}
        }
        return false;

    })
    $('#signOut').on('click', function (event) {
        console.log("signout")
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
                    // $('.loader-overlay').show();
                },
                success: function (result) {
                    location.reload();
                },
                complete: function () {
                    // $('.loader-overlay').hide();
                }
            });
        }
        return false;

    })
});