var ChangePassword = function () {

    this.initialize = function () {
        registerEvents();
    }

    function registerEvents() {

        //Init validation
        $('#frmChangePassword').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                txtCurrentPassword: {
                    required: true,
                    minlength: 6
                },
                txtPassword: { required: true },
                txtConfirmPassword: {
                    equalTo: "#txtPassword"
                }
            }
        });


        $("#btn-change-password").on('click', function () {

            $('#modal-change-password').modal('show');

        });

        $('#btn-save-change-password').on('click', function () {

            if ($('#frmChangePassword').valid()) {
                var currentPassword = $('#txtCurrentPassword').val();
                var password = $('#txtPassword').val();
                var confirmPassword = $('#txtConfirmPassword').val();

                $.ajax({
                    type: "POST",
                    url: "/Admin/account/ChangePassword",
                    dataType: 'JSON',
                    data: {
                        CurrentPassword: currentPassword,
                        Password: password,
                        ConfirmPassword: confirmPassword
                    },
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function (res) {
                        if (res.Success) {
                            window.location.href = "/Admin/Login/Index";
                        } else {
                            core.notify('Please check current password', 'Error');
                        }
                        core.stopLoading();
                    },
                    error: function () {
                        core.notify('Has an error', 'error');
                        core.stopLoading();
                    }
                });
            }
            return false;
        });


    };

}