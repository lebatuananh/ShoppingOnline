var AccountController = function () {

    var changePassword = new ChangePassword();

    var cacheObj = {
        account: null
    }

    this.initialize = function () {
        loadData();
        registerEvents();
        changePassword.initialize();

    }

    function registerEvents() {


        //Init validation
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                txtFullname: { required: true },
                txtUsername: { required: true },
                txtEmail: {
                    required: true,
                    email: true
                }
            }
        });


        $("#btn-update").on('click', function () {

            var acc = cacheObj.account;

            $('#hidId').val(acc.Id);
            $('#txtUsername').val(acc.UserName);
            $('#txtFullname').val(acc.FullName);
            $('#txtEmail').val(acc.Email);

            $('#dtBirthday').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                setDate: new Date()
            }).datepicker("update", core.dateFormatSubStr(acc.BirthDay));

            $('#txtAddress').val(acc.Address);
            $('#ckGender').prop('checked', acc.Gender);
            $('#txtPhoneNumber').val(acc.PhoneNumber);


            $('#modal-add-edit').modal('show');

        });

        $('#btnSave').on('click', function () {

            if ($('#frmMaintainance').valid()) {
                var id = $('#hidId').val();
                var fullname = $('#txtFullname').val();
                var username = $('#txtUsername').val();
                var email = $('#txtEmail').val();
                var address = $('#txtAddress').val();
                var phoneNumber = $('#txtPhoneNumber').val();
                var gender = $('#ckGender').prop('checked');

                var status = 1;

                var birthday = $("#dtBirthday").data('datepicker').getFormattedDate('yyyy-mm-dd');


                $.ajax({
                    type: "POST",
                    url: "/Admin/account/update",
                    dataType: 'JSON',
                    data: {
                        Id: id,
                        FullName: fullname,
                        UserName: username,
                        Email: email,
                        PhoneNumber: phoneNumber,
                        Status: status,
                        Gender: gender,
                        BirthDay: birthday,
                        Address: address
                    },
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function (res) {
                        if (res.Success) {
                            loadData();
                            core.notify('Save user succesful', 'success');
                            $('#modal-add-edit').modal('hide');
                        } else {
                            core.notify('Please check username and email', 'Error');
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

    function loadData() {
        $.ajax({
            type: "GET",
            url: "/admin/account/account",
            dataType: "json",
            beforeSend: function () {
                core.startLoading();
            },
            success: function (res) {
                cacheObj.account = res;
                var template = $('#table-template').html();
                var render = '';
                if (res !== null) {
                    render = Mustache.render(template, {
                        Username: res.UserName,
                        FullName: res.FullName,
                        Birthday: core.dateFormatSubStr(res.BirthDay),
                        Email: res.Email,
                        PhoneNumber: res.PhoneNumber,
                        Address: res.Address,
                        Gender: res.Gender === 1 ? 'Male' : 'Female'
                    });

                    $('#tbl-content').html(render);
                }
                else {
                    core.notify('Can not load data', 'error');
                }
                core.stopLoading();
            },
            error: function () {
                core.notify('Can not load data', 'error');
                core.stopLoading();
            }
        });
    };
}