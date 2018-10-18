var productController = function () {
    var quantityManagement = new QuantityManagement();
    var imageManagement = new ImageManagement();
    var wholePriceManagement = new WholePriceManagement();

    this.initialize = function () {

        loadData();
        loadCategories();
        registerEvents();
        registerControls();

        imageManagement.initialize();
        quantityManagement.initialize();
        wholePriceManagement.initialize();

    };

    function registerEvents() {

        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                txtNameM: {required: true},
                ddlCategoryIdM: {required: true},
                txtPriceM: {
                    required: true,
                    number: true,
                    min: 0
                },
                txtOriginalPriceM: {
                    required: true,
                    number: true,
                    min: 0
                },
                txtPromotionPriceM: {
                    number: true,
                    min: 0
                }
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
            data.append("height", "360");
            data.append("width", "360");

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
                        $('#txtImage').val(path);
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

        $('#btn-import').on('click', function () {
            initTreeDropDownCategory();
            $('#modal-import-excel').modal('show');
            $(".combo").css("width", "20%");
            $('.textbox-text').css("width", "100%")
        });

        $('#btnImportExcel').on('click', function () {
            importExcel();

        });

        $('#btn-export').on('click', function () {
            exportExcel();
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
                categoryId: $('#ddlCategorySearch').val(),
                keyword: $('#txtKeyword').val(),
                page: core.configs.pageIndex,
                pageSize: core.configs.pageSize
            },
            url: '/admin/product/GetAllPaging',
            dataType: 'json',
            success: function (res) {
                if (res.Results.length > 0) {
                    $.each(res.Results, function (i, item) {
                        render += Mustache.render(template, {
                            Id: item.Id,
                            Name: item.Name,
                            Image: item.Image == null ? '<img src="/admin-side/images/user.png" width=25' : '<img src="' + item.Image + '" width=25 />',
                            CategoryName: item.ProductCategory.Name,
                            Price: core.formatNumber(item.Price, 0),
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
                    core.notify('Loaded ' + res.RowCount + ' success product', 'success');
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

    function loadCategories() {
        $.ajax({
            type: 'GET',
            url: '/admin/ProductCategory/GetAll',
            dataType: 'json',
            success: function (res) {
                var render = '<option>---Select category---</option>';
                $.each(res, function (i, item) {
                    render += '<option value="' + item.Id + '">' + item.Name + '</option>';
                });
                $('#ddlCategorySearch').html(render);
            },
            error: function (status) {
                console.log(status);
                core.notify('Cannot loading product category data', 'error');
            }
        });
    }

    function initTreeDropDownCategory(isSelected) {

        $.ajax({

            url: '/Admin/ProductCategory/GetAll',
            type: 'GET',
            dataType: 'JSON',
            async: false,
            success: function (res) {
                var data = [];

                $.each(res, function (i, item) {

                    data.push({
                        id: item.Id,
                        text: item.Name,
                        parentId: item.ParentId,
                        sortOrder: item.SortOrder
                    });

                });


                var arr = core.unflattern(data);

                $('#ddlCategoryIdM').combotree({
                    data: arr
                });

                $('#ddlCategoryIdImportExcel').combotree({
                    data: arr
                });

                if (isSelected != undefined) {
                    $('#ddlCategoryIdM').combotree('setValue', isSelected);
                }
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
        initTreeDropDownCategory('');
        $('#txtDescM').val('');
        $('#txtUnitM').val('');

        $('#txtImage').val('');
        CKEDITOR.instances.txtContentM.setData('');

        $('#txtPriceM').val('');
        $('#txtOriginalPriceM').val('');
        $('#txtPromotionPriceM').val('');
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
            var categoryId = $('#ddlCategoryIdM').combotree('getValue');

            var description = $('#txtDescM').val();
            var unit = $('#txtUnitM').val();

            var price = $('#txtPriceM').val();
            var originalPrice = $('#txtOriginalPriceM').val();
            var promotionPrice = $('#txtPromotionPriceM').val();

            var image = $('#txtImage').val();

            var tags = $('#txtTagM').val();
            var seoKeyword = $('#txtMetakeywordM').val();
            var seoMetaDescription = $('#txtMetaDescriptionM').val();
            var seoPageTitle = $('#txtSeoPageTitleM').val();
            var seoAlias = $('#txtSeoAliasM').val();

            var content = CKEDITOR.instances.txtContentM.getData();

            var status = $('#ckStatusM').prop('checked') == true ? 1 : 0;
            var hot = $('#ckHotM').prop('checked');
            var showHome = $('#ckShowHomeM').prop('checked');

            $.ajax({
                type: 'POST',
                url: '/Admin/Product/SaveEntity',
                dataType: 'JSON',
                data: {
                    Id: id,
                    Name: name,
                    CategoryId: categoryId,
                    Image: image,
                    Price: price,
                    OriginalPrice: originalPrice,
                    PromotionPrice: promotionPrice,
                    Description: description,
                    Content: content,
                    HomeFlag: showHome,
                    HotFlag: hot,
                    Tags: tags,
                    Unit: unit,
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
            url: "/Admin/Product/GetById",
            data: {id: that},
            dataType: "json",
            beforeSend: function () {
                core.startLoading();
            },
            success: function (res) {
                $('#hidIdM').val(res.Id);
                $('#txtNameM').val(res.Name);
                initTreeDropDownCategory(res.CategoryId);

                $('#txtDescM').val(res.Description);
                $('#txtUnitM').val(res.Unit);

                $('#txtPriceM').val(res.Price);
                $('#txtOriginalPriceM').val(res.OriginalPrice);
                $('#txtPromotionPriceM').val(res.PromotionPrice);

                $('#txtImage').val(res.Image);
                $('#productImage').attr("src", res.Image);

                $('#txtTagM').val(res.Tags);
                $('#txtMetakeywordM').val(res.SeoKeywords);
                $('#txtMetaDescriptionM').val(res.SeoDescription);
                $('#txtSeoPageTitleM').val(res.SeoPageTitle);
                $('#txtSeoAliasM').val(res.SeoAlias);

                CKEDITOR.instances.txtContentM.setData(res.Content);
                $('#ckStatusM').prop('checked', res.Status == 1);
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
                url: '/Admin/Product/Delete',
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

    function exportExcel() {
        $.ajax({
            type: "POST",
            url: "/Admin/Product/ExportExcel",
            beforeSend: function () {
                core.startLoading();
            },
            success: function (response) {
                window.location.href = response;
                core.stopLoading();
            },
            error: function () {
                core.notify('Has an error in progress', 'error');
                core.stopLoading();
            }
        });
    }

    function importExcel() {
        var fileUpload = $("#fileInputExcel").get(0);
        var files = fileUpload.files;
        // Create FormData object  
        var fileData = new FormData();
        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append("files", files[i]);
        }
        // Adding one more key to FormData object  
        fileData.append('categoryId', $('#ddlCategoryIdImportExcel').combotree('getValue'));
        $.ajax({
            url: '/Admin/Product/ImportExcel',
            type: 'POST',
            data: fileData,
            processData: false,  // tell jQuery not to process the data
            contentType: false,  // tell jQuery not to set contentType
            success: function (data) {
                $('#modal-import-excel').modal('hide');
                loadData();
                core.notify('Import success product','success')
            },
            error:function (status) {
                core.notify('Import failed','error');
                console.log(status)
            }
        });
        return false;
        $('#modal-import-excel').modal('hide');
    }

    function clearFileInput() {
        $('#fileImage').val(null);
    }
}