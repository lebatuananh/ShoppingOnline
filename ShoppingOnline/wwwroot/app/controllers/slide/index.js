var SlideController = function () {

    this.initialize = function () {
        loadData();
        registerEvents();
    }

    function registerEvents() {
        //Init validation
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'en',
            rules: {
                txtNameM: {required: true},
                txtUrlM: {required: true},
                txtDisplayOrderM: {
                    required: true,
                    number: true
                },
                txtImageM: {required: true}
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
                url: "/Admin/slide/GetById",
                data: {id: that},
                dataType: "json",
                beforeSend: function () {
                    core.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidIdM').val(data.Id);
                    $('#txtNameM').val(data.Name);
                    $('#txtUrlM').val(data.Url);
                    $('#txtImageM').val(data.Image);

                    $('#ckStatusM').prop('checked', data.Status === true);
                    $('#txtDisplayOrderM').val(data.DisplayOrder);
                    $('#modal-add-edit').modal('show');

                    core.stopLoading();

                },
                error: function () {
                    core.notify('Has an error', 'error');
                    core.stopLoading();
                }
            });
        });

        $('#btnSaveM').on('click', function (e) {

            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidIdM').val();
                var name = $('#txtNameM').val();
                var image = $('#txtImageM').val();
                var url = $('#txtUrlM').val();
                var status = $('#ckStatusM').prop('checked');
                var sortOrder = $('#txtDisplayOrderM').val();
                var groupAlias = 'top';

                $.ajax({
                    type: "POST",
                    url: "/Admin/slide/SaveEntity",
                    data: {
                        Id: id,
                        Name: name,
                        Url: url,
                        Status: status,
                        Image: image,
                        Content: null,
                        Description: null,
                        DisplayOrder: sortOrder,
                        GroupAlias: groupAlias
                    },
                    dataType: "json",
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function () {
                        core.notify('Save slide successful', 'success');
                        $('#modal-add-edit').modal('hide');
                        resetFormMaintainance();

                        loadData(true);
                        $('#paginationUL').twbsPagination('destroy');
                        core.stopLoading();

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
                    url: "/Admin/slide/Delete",
                    data: {id: that},
                    dataType: "json",
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function () {
                        core.notify('Delete slide successful', 'success');
                        core.stopLoading();
                        loadData();
                        $('#paginationUL').twbsPagination('destroy');

                    },
                    error: function () {
                        core.notify('Have an error in progress', 'error');
                        core.stopLoading();
                    }
                });
            });
        });

        $('#btnSelectImg').on('click', function () {
            $('#fileInputImage').click();
        });

        $("#fileInputImage").on('change', function () {

            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();
            //data.append("height", "450");
            //data.append("width", "865");

            var ValidImageTypes = ["image/gif", "image/jpeg", "image/png"];

            if ($.inArray(files[0].type, ValidImageTypes) > 0) {

                for (var i = 0; i < files.length; i++) {
                    data.append(files[i].name, files[i]);
                }

                $.ajax({
                    type: "POST",
                    url: "/Admin/Upload/UploadSlide",
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (path) {
                        $('#txtImageM').val(path);
                        core.notify('Upload image succesful!', 'success');

                    },
                    error: function () {
                        core.notify('There was error uploading files!', 'error');
                    }
                });
            } else {
                core.notify('There was error uploading files!', 'error');
                clearFileInput();
            }
        });
    };

    function resetFormMaintainance() {
        $('#hidIdM').val(0);
        $('#txtNameM').val('');
        $('#txtUrlM').val('#');
        $('#txtDisplayOrderM').val(0);
        $('#ckStatusM').prop('checked', true);
        clearFileInput();
    }

    function loadData(isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/slide/GetAllPaging",
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
                            Image: item.Image,
                            Id: item.Id,
                            DisplayOrder: item.DisplayOrder,
                            Status: core.getStatus(item.Status)
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
    };

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

    function clearFileInput() {
        $('#txtImageM').val(null);
    }
}