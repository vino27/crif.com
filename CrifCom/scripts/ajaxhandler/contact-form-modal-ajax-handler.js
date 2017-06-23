$(document).ready(function () { 
    $('#message-failed-contact').hide(); 
    $('#loader').hide();
    $('#ContactModalForm').on('submit', function (event) {
       
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
                        if (result.Status == "Success") { 
                            window.location.href = result.Url;
                            $('#loader').hide(); 
                        } else {
                            $('#message-failed-contact').show(); 
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
    $('#checkbox1').change(function () {
        if ($(this).is(":checked")) {
            $("#terms-chkbx").removeClass("spot");
        }
    });
});