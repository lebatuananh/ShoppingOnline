var ShipperController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    };

    function registerEvents() {
        //Init validation
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'en',
            rules: {
                txtNameM: {required: true},
                txtComM: {required: true},
                txtPhoneM: {required: true}
            }
        });

        $('#txt-search-keyword').keypress(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                core.configs.pageIndex = 1;
                loadData();
                $('#paginationUL').twbsPagination('destroy');
            }
        });

        $("#btn-search").on('click', function () {
            core.configs.pageIndex = 1;
            loadData();
            $('#paginationUL').twbsPagination('destroy');
        });

        $("#ddl-show-page").on('change', function () {
            core.configs.pageSize = $(this).val();
            core.configs.pageIndex = 1;
            loadData(true);
        });

        $("#btn-create").on('click', function () {
            resetFormMaintainance();
            $('#modal-add-edit').modal('show');

        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "GET",
                url: "/Admin/Shipper/GetById",
                data: {id: that},
                dataType: "json",
                beforeSend: function () {
                    core.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidIdM').val(data.Id);
                    $('#txtNameM').val(data.Name);

                    $('#txtComM').val(data.CompanyName);
                    $('#txtPhoneM').val(data.Phone);
                    $('#ckStatusM').prop('checked', data.Status === 1);

                    $('#modal-add-edit').modal('show');
                    core.stopLoading();

                },
                error: function () {
                    core.notify('Has an error', 'error');
                    core.stopLoading();
                }
            });
        });

        $('#btnSave').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidIdM').val();
                var name = $('#txtNameM').val();
                var companyName = $('#txtComM').val();
                var phone = $('#txtPhoneM').val();
                var status = $('#ckStatusM').prop('checked') === true ? 1 : 0;

                $.ajax({
                    type: "POST",
                    url: "/Admin/Shipper/SaveEntity",
                    data: {
                        Id: id,
                        Name: name,
                        CompanyName: companyName,
                        Phone: phone,
                        Status: status
                    },
                    dataType: "json",
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function () {
                        core.notify('Update shipper successful', 'success');
                        $('#modal-add-edit').modal('hide');
                        resetFormMaintainance();

                        core.stopLoading();
                        loadData(true);
                        $('#paginationUL').twbsPagination('destroy');

                    },
                    error: function () {
                        core.notify('Have an error in progress', 'error');
                        core.stopLoading();
                    }
                });
                return false;
            }
            return false;
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            core.confirm('Are you sure to delete?', function () {
                $.ajax({
                    type: "POST",
                    url: "/Admin/Shipper/Delete",
                    data: {id: that},
                    dataType: "json",
                    beforeSend: function () {
                        core.startLoading();
                    },
                    error: function () {
                        core.notify('Delete shipper successful', 'success');
                        core.stopLoading();
                        loadData();
                        $('#paginationUL').twbsPagination('destroy');

                    },
                    success: function () {
                        core.notify('Have an error in progress', 'error');
                        core.stopLoading();
                    }
                });
            });
        });
    }

    function resetFormMaintainance() {
        $('#hidIdM').val(0);
        $('#txtNameM').val('');
        $('#txtComM').val('');
        $('#txtPhoneM').val();
        $('#ckStatusM').prop('checked', true);
    }

    function loadData(isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/Shipper/GetAllPaging",
            data: {
                keyword: $('#txt-search-keyword').val(),
                page: core.configs.pageIndex,
                pageSize: core.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
                core.startLoading();
            },
            success: function (response) {
                var template = $('#table-template').html();
                var render = "";
                if (response.RowCount > 0) {
                    $.each(response.Results, function (i, item) {
                        render += Mustache.render(template, {
                            Name: item.Name,
                            CompanyName: item.CompanyName,
                            Phone:item.Phone,
                            Id: item.Id,
                            Status: core.getStatus(item.Status),
                            CreatedDate: core.dateFormatSubStr(item.DateCreated)
                        });
                    });
                    $("#lbl-total-records").text(response.RowCount);
                    if (render != undefined) {
                        $('#tbl-content').html(render);

                    }
                    wrapPaging(response.RowCount, function () {
                        loadData();
                    }, isPageChanged);
                }
                else {
                    $('#tbl-content').html('');
                }
                core.stopLoading();
            },
            error: function (status) {
                console.log(status);
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
            prev: 'Prevous',
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

};