var loginController = function () {
    this.initialize = function () {
        registerEvents();
    };

    var registerEvents = function () {

        $('#frmLogin').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                username: {
                    required: true
                },
                password: {
                    required: true
                }
            },
            messages: {
                username: {
                    required: 'Username is required'
                },
                password: {
                    required: 'Password is required'
                }
            }
        });

        $('#btnLogin').on('click', function (e) {
            if ($('#frmLogin').valid()) {
                e.preventDefault();
                var username = $('#txtUsername').val();
                var password = $('#txtPassword').val();
                login(username, password);
            }
        });

    };

    var login = function (username, password) {
        $.ajax({
            type: 'POST',
            data: {
                Username: username,
                Password: password
            },
            dataType: 'JSON',
            url: '/Admin/Login/Authen',
            success: function (res) {
                if (res.Success) {
                    var urlRedirect = core.getParamUrl('ReturnUrl');
                    // debugger;
                    console.log(urlRedirect);
                    window.location.href = core.getParamUrl('ReturnUrl') || '/Admin/Home/Index';
                    // window.location.href = '/Admin/Home/Index';
                }
                else {
                    core.notify('Login Failed', 'error');
                }

            }
        });
    };
};