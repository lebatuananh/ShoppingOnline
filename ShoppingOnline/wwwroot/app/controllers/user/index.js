var UserController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    };

    function registerEvents() {

        $('#txtBirthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            setDate: new Date()

        }).datepicker("update", new Date());

        //Init validation
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                txtFullName: { required: true },
                txtUserName: { required: true },
                txtPassword: {
                    required: true,
                    minlength: 6
                },
                txtConfirmPassword: {
                    equalTo: "#txtPassword"
                },
                txtEmail: {
                    required: true,
                    email: true
                }
            }
        });

        $('#ddl-show-page').on('change', function () {
            core.configs.pageSize = $(this).val();
            core.configs.page = 1;
            loadData(true);
        });

        $('#btn-search').on('click', function (e) {
            e.preventDefault();
            core.configs.pageIndex = 1;
            loadData();
            $('#paginationUL').twbsPagination('destroy');
        });

        $('#txt-search-keyword').on('keypress', function (e) {
            if (e.which == 13) {
                e.preventDefault();
                core.configs.pageIndex = 1;
                loadData();
                $('#paginationUL').twbsPagination('destroy');
            }
        });

        $('#btn-create').on('click', function () {
            resetFormMaintainance();
            initRoleList();
            $('#modal-add-edit').modal('show');
        });

        $('#btnSave').on('click', function () {
            if ($('#frmMaintainance').valid()) {

                var id = $('#hidId').val();
                var fullname = $('#txtFullName').val();
                var username = $('#txtUserName').val();
                var password = $('#txtPassword').val();
                var email = $('#txtEmail').val();
                var address = $('#txtAddress').val();
                var phoneNumber = $('#txtPhoneNumber').val();
                var gender = $('#ckGender').prop('checked');
                var roles = [];

                $.each($('input[name="ckRoles"]'), function (i, item) {
                    if ($(item).prop('checked') === true) {
                        roles.push($(item).prop('value'));
                    }
                });

                debugger

                if (roles.length === 0) {
                    core.notify('Please, check roles', 'error');
                    return false;
                }

                var status = $('#ckStatus').prop('checked') === true ? 1 : 0;

                var birthday = $("#txtBirthday").data('datepicker').getFormattedDate('yyyy-mm-dd');


                $.ajax({
                    type: "POST",
                    url: "/Admin/User/SaveEntity",
                    dataType: 'JSON',
                    data: {
                        Id: id,
                        FullName: fullname,
                        UserName: username,
                        Password: password,
                        Email: email,
                        PhoneNumber: phoneNumber,
                        Status: status,
                        Roles: roles,
                        Gender: gender,
                        BirthDay: birthday,
                        Address: address
                    },
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function (res) {
                        if (res.Success) {
                            core.notify('Save user succesful', 'success');
                            $('#modal-add-edit').modal('hide');
                            resetFormMaintainance();
                            core.stopLoading();
                            loadData(true);
                            $('#paginationUL').twbsPagination('destroy');
                        } else {
                            core.notify('Please, check username or email or phone number', 'Error');
                        }
                    },
                    error: function () {
                        core.notify('Has an error', 'error');
                        core.stopLoading();
                    }
                });
            }
            return false;
        });

        $('body').on('click', '.btnDelete', function () {

            var id = $(this).data('id');
            core.confirm('Are you sure to delete?', function () {

                $.ajax({

                    url: '/Admin/User/Delete',
                    type: 'post',
                    data: {
                        id: id
                    },
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function () {
                        core.notify('Delete successful', 'success');
                        core.stopLoading();
                        loadData();
                        $('#paginationUL').twbsPagination('destroy');

                    },
                    error: function () {
                        core.notify('Has an error', 'error');
                        core.stopLoading();
                    }

                });

            });
        });

        $('body').on('click', '.btnEdit', function () {

            var id = $(this).data('id');

            $.ajax({

                url: '/Admin/User/GetById',
                type: 'get',
                dataType: 'json',
                data: {
                    id: id
                },
                beforeSend: function () {
                    core.startLoading();
                },
                success: function (res) {

                    $('#hidId').val(res.Id);
                    $('#txtFullName').val(res.FullName);
                    $('#txtUserName').val(res.UserName);
                    $('#txtEmail').val(res.Email);
                    $('#txtPhoneNumber').val(res.PhoneNumber);
                    $('#ckStatus').prop('checked', res.Status === 1);
                    $('#txtAddress').val(res.Address);
                    $('#ckGender').prop('checked', res.Gender);

                    $('#txtBirthday').datepicker({
                        autoclose: true,
                        format: 'dd/mm/yyyy',
                        setDate: new Date()
                    }).datepicker("update", core.dateFormatSubStr(res.BirthDay));

                    initRoleList(res.Roles);

                    disableFieldEdit(true);
                    $('#modal-add-edit').modal('show');
                    core.stopLoading();

                },
                error: function (res) {
                    core.notify('Has a error', 'error');
                    core.stopLoading();
                }

            });

        });

        $('body').on('click', '.btnReset', function () {

            var id = $(this).data('id');


            core.confirm('Are you sure to reset password this account ?', function () {

                $.ajax({

                    url: '/Admin/User/ResetPassword',
                    type: 'post',
                    dataType: 'json',
                    data: {
                        userId: id
                    },
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function (res) {

                        if (res.Success) {
                            core.notify('Your password has been reset successfully', 'success');
                        } else {
                            core.notify('Reset Password Failed', 'error');
                        }
                        core.stopLoading();

                    },
                    error: function (res) {
                        core.notify('Has a error', 'error');
                        core.stopLoading();
                    }

                });
            });

        });
    }

    function loadData(isPageChanged) {

        $.ajax({

            url: '/Admin/User/GetAllPaging',
            type: 'GET',
            dataType: 'JSON',
            data: {
                keyword: $('#txt-search-keyword').val(),
                page: core.configs.pageIndex,
                pageSize: core.configs.pageSize
            },
            beforeSend: function () {
                core.startLoading();
            },
            success: function (res) {
                var template = $('#table-template').html();

                var render = '';

                if (res.RowCount > 0) {
                    $.each(res.Results, function (i, item) {
                        render += Mustache.render(template, {
                            FullName: item.FullName,
                            Id: item.Id,
                            UserName: item.UserName,
                            Avatar: ( item.Avatar == null || item.Avatar===undefined )? '<img src="/admin-site/images/user.png" width=25/>' : '<img src="' + item.Avatar + '" width=25/>',
                            DateCreated: core.dateFormatSubStr(item.DateCreated),
                            Status: core.getStatus(item.Status)
                        });
                    });

                    $('#lbl-total-records').text(res.RowCount);

                    if (render != undefined) {
                        $('#tbl-content').html(render);
                    }

                    wrapPaging(res.RowCount, function () {
                        loadData();
                    }, isPageChanged)
                } else {
                    $('#tbl-content').html('');
                }
                core.stopLoading();
            },
            error: function (status) {
                console.log(status)
            }

        });

    }

    function wrapPaging(recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / core.configs.pageSize);
        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }
        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: 'First',
            prev: 'Previous',
            next: 'Next',
            last: 'Last',
            onPageClick: function (event, p) {
                if (core.configs.pageIndex != p) {
                    core.configs.pageIndex = p;
                    setTimeout(callBack(), 200);
                }
            }
        });
    }

    function initRoleList(selectedRoles) {

        $.ajax({

            url: '/Admin/Role/GetAll',
            type: 'GET',
            dataType: 'JSON',
            async: false,
            success: function (res) {
                var template = $('#role-template').html();
                var render = '';

                $.each(res, function (i, item) {
                    var checked = '';
                    if (selectedRoles !== undefined && selectedRoles.indexOf(item.Name) !== -1) {
                        checked = 'checked';
                    }

                    render += Mustache.render(template, {
                        Name: item.Name,
                        Description: item.Description,
                        Checked: checked
                    });

                    $('#list-roles').html(render);

                });
            }

        });

    }

    function resetFormMaintainance() {
        disableFieldEdit(false);
        $('#hidId').val('');
        initRoleList();
        $('#txtFullName').val('');
        $('#txtAddress').val('');
        $('#txtUserName').val('');
        $('#txtPassword').val('');
        $('#txtConfirmPassword').val('');
        $('input[name="ckRoles"]').removeAttr('checked');
        $('#txtEmail').val('');
        $('#txtPhoneNumber').val('');
        $('#ckStatus').prop('checked', true);
        $('#ckGender').prop('checked', true);
        $('#txtBirthday').datepicker({
            autoclose: true,
            format: 'dd-mm-yyyy'
        }).datepicker("update", new Date());

    }

    function disableFieldEdit(disabled) {
        $('#txtUserName').prop('disabled', disabled);
        $('#txtPassword').prop('disabled', disabled);
        $('#txtConfirmPassword').prop('disabled', disabled);

    }
}