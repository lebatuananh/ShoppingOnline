var BlogController = function () {
    var sourceImage;

    var options = {
        aspectRatio: 1 / 1,
        minCropBoxWidth: 473,
        minCropBoxHeight: 473,
        zoomOnWheel: false,
        autoCrop: false
    };

    this.initialize = function () {
        loadData();
        registerEvents();
        registerControls();
    };

    function registerEvents() {
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                txtNameM: {required: true},
            }
        });

        //TODO: binding event to controll

        $('#ddlShowPage').on('change', function () {
            core.configs.pageSize = $(this).val();
            core.configs.pageIndex = 1;
            loadData(true);
        });

        $('#btnSearch').on('click', function () {
            core.configs.pageIndex = 1;
            loadData();
            $('#paginationUL').twbsPagination('destroy');
        });

        $('#txtKeyword').on('keypress', function (e) {
            if (e.which == 13) {
                e.preventDefault();
                core.configs.pageIndex = 1;
                loadData();
                $('#paginationUL').twbsPagination('destroy');
            }
        });

        $('#btnCreate').on('click', function (e) {
            e.preventDefault();
            $('#modal-add-edit').modal('show');
            resetFormMaintainance();
            $(".combo").css("width", "100%");
        });

        $('body').on('click', '.btnDelete', function (e) {
            e.preventDefault();

            var that = $(this).data('id');

            deleteItem(that);

        });

        $('#btnSave').on('click', function () {
            saveProduct();
        });

        $('body').on('click', '.btnEdit', function (e) {
            e.preventDefault();

            var that = $(this).data('id');

            loadDetails(that);
        });

        $('#btnSelectImg').on('click', function () {
            $('#fileInputImage').click();
        });

        $("#fileInputImage").on('change', function () {

            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();
            var ValidImageTypes = ["image/gif", "image/jpeg", "image/png"];

            if ($.inArray(files[0].type, ValidImageTypes) > 0) {

                for (var i = 0; i < files.length; i++) {
                    data.append(files[i].name, files[i]);
                }

                $.ajax({
                    type: "POST",
                    url: "/Admin/Upload/UploadImage",
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (path) {
                        sourceImage = path;
                        $('#source-image').html('<img data-path="' + path + '" src="' + path + '"  id="image" style="max-width: 100%">');
                        $('#source-image').show();
                        cropImage();
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
    }

    function registerControls() {
        CKEDITOR.replace('txtContentM', {});

        //Fix: cannot click on element ck in modal
        $.fn.modal.Constructor.prototype.enforceFocus = function () {
            $(document)
                .off('focusin.bs.modal') // guard against infinite focus loop
                .on('focusin.bs.modal', $.proxy(function (e) {
                    if (
                        this.$element[0] !== e.target && !this.$element.has(e.target).length
                        // CKEditor compatibility fix start.
                        && !$(e.target).closest('.cke_dialog, .cke').length
                    // CKEditor compatibility fix end.
                    ) {
                        this.$element.trigger('focus');
                    }
                }, this));
        };
    }

    function loadData(isPageChanged) {
        var template = $('#table-template').html();
        var render = '';
        $.ajax({
            type: 'GET',
            data: {
                keyword: $('#txtKeyword').val(),
                page: core.configs.pageIndex,
                pageSize: core.configs.pageSize
            },
            url: '/admin/blog/GetAllPaging',
            dataType: 'json',
            success: function (res) {
                if (res.Results.length > 0) {
                    $.each(res.Results, function (i, item) {
                        render += Mustache.render(template, {
                            Id: item.Id,
                            Name: item.Name,
                            Image: item.Image == null ? '<img src="/admin-side/images/user.png" width=25' : '<img src="' + item.Image + '" width=25 />',
                            Description: item.Description,
                            CreatedDate: core.dateFormatSubStr(item.DateCreated),
                            Status: core.getStatus(item.Status)
                        });

                        $('#lblTotalRecords').text(res.RowCount);

                        if (render != '') {
                            $('#tbl-content').html(render);
                        }

                        wrapPaging(res.RowCount, function () {
                            loadData();
                        }, isPageChanged);

                    });
                } else {
                    core.notify('Not found item', 'error');
                }
            },
            error: function (status) {
                console.log(status);
                core.notify('Cannot loading data', 'error');
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

    function resetFormMaintainance() {

        $('#hidIdM').val(0);
        $('#txtNameM').val('');
        $('#txtDescM').val('');

        $('#txtImage').val('');
        $('#source-image').html('');
        $('#source-thumbnail').html('');
        $('#source-image').hide();
        $('#source-thumbnail').hide();
        CKEDITOR.instances.txtContentM.setData('');

        $('#txtSeoPageTitleM').val('');

        $('#txtSeoAliasM').val('');
        $('#txtMetakeywordM').val('');
        $('#txtMetaDescriptionM').val('');
        $('#txtTagM').val('');

        $('#ckStatusM').attr('checked', true);
        $('#ckHotM').attr('checked', false);
        $('#ckShowHomeM').attr('checked', true);

        $(".combo").css("width", "100%");

    }

    function saveProduct() {
        if ($('#frmMaintainance').valid()) {
            var id = $('#hidIdM').val();
            var name = $('#txtNameM').val();

            var description = $('#txtDescM').val();

            var image = $('#txtImage').val();

            var tags = $('#txtTagM').val();
            var seoKeyword = $('#txtMetakeywordM').val();
            var seoMetaDescription = $('#txtMetaDescriptionM').val();
            var seoPageTitle = $('#txtSeoPageTitleM').val();
            var seoAlias = $('#txtSeoAliasM').val();

            var content = CKEDITOR.instances.txtContentM.getData();

            var status = $('#ckStatusM').prop('checked') === true ? 1 : 0;
            var hot = $('#ckHotM').prop('checked');
            var showHome = $('#ckShowHomeM').prop('checked');

            $.ajax({
                type: 'POST',
                url: '/Admin/Blog/SaveEntity',
                dataType: 'JSON',
                data: {
                    Id: id,
                    Name: name,
                    Image: image,
                    Description: description,
                    Content: content,
                    HomeFlag: showHome,
                    HotFlag: hot,
                    Tags: tags,     
                    Status: status,
                    SeoPageTitle: seoPageTitle,
                    SeoAlias: seoAlias,
                    SeoKeywords: seoKeyword,
                    SeoDescription: seoMetaDescription
                },
                beforeSend: function () {
                    core.startLoading();
                },
                success: function (res) {

                    core.notify('Save or update success', 'success');

                    core.stopLoading();
                    $('#paginationUL').twbsPagination('destroy');
                    loadData();
                    resetFormMaintainance();
                    $('#modal-add-edit').modal('hide');

                },
                error: function (res) {
                    core.notify('Has a error in save product progress', 'error');
                    core.stopLoading();
                }
            });
            return false;
        }
    }

    function loadDetails(that) {

        $.ajax({
            type: "GET",
            url: "/Admin/Blog/GetById",
            data: {id: that},
            dataType: "json",
            beforeSend: function () {
                core.startLoading();
            },
            success: function (res) {
                $('#hidIdM').val(res.Id);
                $('#txtNameM').val(res.Name);
                $('#txtDescM').val(res.Description);
                $('#txtImage').val(res.Image);
                $('#source-image').html('');
                $('#source-image').hide();
                $('#source-thumbnail').html('<img src = "' + res.Image + '" />');
                $('#source-thumbnail').show();

                $('#txtTagM').val(res.Tags);
                $('#txtMetakeywordM').val(res.SeoKeywords);
                $('#txtMetaDescriptionM').val(res.SeoDescription);
                $('#txtSeoPageTitleM').val(res.SeoPageTitle);
                $('#txtSeoAliasM').val(res.SeoAlias);

                CKEDITOR.instances.txtContentM.setData(res.Content);
                $('#ckStatusM').prop('checked', res.Status === 1);
                $('#ckHotM').prop('checked', res.HotFlag);
                $('#ckShowHomeM').prop('checked', res.HomeFlag);

                $('#modal-add-edit').modal('show');
                core.stopLoading();
                $(".combo").css("width", "100%");
                $(".textbox-text").css("width", "100%");

            },
            error: function (status) {
                core.notify('Có lỗi xảy ra', 'error');
                core.stopLoading();
            }
        });
    }

    function deleteItem(that) {
        core.confirm('Are you sure to delete?', function () {

            $.ajax({
                url: '/Admin/Blog/Delete',
                type: 'POST',
                data: {
                    id: that
                },
                beforeSend: function () {
                    core.startLoading();
                },
                success: function () {
                    core.notify('Deleted success', 'success');
                    core.stopLoading();
                    $('#paginationUL').twbsPagination('destroy');
                    loadData();
                },
                error: function () {
                    core.notify('Deleted fail', 'error');
                    core.stopLoading();
                }
            });

        });
    }

    function clearFileInput() {
        $('#fileImage').val(null);
    }

    function cropImage() {
        var $image = $('#image');
        var x = 0;
        var y = 0;
        var w = 0;
        var h = 0;
        $image.on({
            crop: function (event) {
                x = event.detail.x;
                y = event.detail.y;
                w = event.detail.width;
                h = event.detail.height;
            },
            cropend: function (e) {
                updateCoords(x, y, w, h);
            }
        }).cropper(options);
    }

    function updateCoords(x, y, w, h) {
        var data = new FormData();
        data.append('x', x);
        data.append('y', y);
        data.append('h', h);
        data.append('w', w);
        data.append('source', sourceImage);

        $.ajax({
            type: "POST",
            url: "/Admin/Upload/Thumbnail",
            contentType: false,
            processData: false,
            data: data,
            success: function (res) {
                var img = '<img src="' + res + '"/>';
                $('#source-thumbnail').html(img);
                $('#txtImage').val(res);
                $('#source-thumbnail').show();
            },
            error: function () {
                core.notify('Has an error in process', 'error');
            }
        });

    }
};