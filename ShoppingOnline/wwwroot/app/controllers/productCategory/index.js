var productCategoryController = function () {
    this.initialize = function () {
        loadData();
        registerEvent();
    }


    function registerEvent() {

        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'en',
            rules: {
                txtNameM: {required: true},
                txtOrderM: {number: true, required: true},
                txtHomeOrderM: {number: true, required: true}
            }
        });

        $('#btnCreate').off('click').on('click', function (e) {
            e.preventDefault();
            $('#modal-add-edit').modal('show');
            //initTreeDropDownCategory();
            resetFormMaintainance();
            $(".combo").css("width", "100%");
        });

        $('body').on('click', '#btnEdit', function (e) {
            e.preventDefault();

            var id = $('#hidIdM').val();

            $.ajax({
                type: 'GET',
                url: '/Admin/ProductCategory/GetById',
                data: {id: id},
                dataType: 'json',
                beforeSend: function () {
                    core.startLoading();
                },
                success: function (res) {

                    $('#hidIdM').val(id);
                    $('#txtNameM').val(res.Name);

                    initTreeDropDownCategory(res.ParentId);

                    $('#txtDescM').val(res.Description);

                    $('#txtImage').val(res.Image);

                    $('#txtSeoKeywordM').val(res.SeoKeywords);
                    $('#txtSeoDescriptionM').val(res.SeoDescription);
                    $('#txtSeoPageTitleM').val(res.SeoPageTitle);
                    $('#txtSeoAliasM').val(res.SeoAlias);

                    $('#ckStatusM').prop('checked', res.Status == 1);
                    $('#ckShowHomeM').prop('checked', res.HomeFlag);
                    $('#txtOrderM').val(res.SortOrder);
                    $('#txtHomeOrderM').val(res.HomeOrder);

                    $('#modal-add-edit').modal('show');
                    core.stopLoading();

                    $(".combo").css("width", "100%");
                    $('.textbox-text').css('width', '100%');
                },
                error: function (status) {
                    core.notify('Có lỗi xảy ra', 'error');
                    core.stopLoading();
                }
            });

        });

        $('#btnSave').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var id = parseInt($('#hidIdM').val());
                var name = $('#txtNameM').val();
                var parentId = $('#ddlCategoryIdM').combotree('getValue');
                var description = $('#txtDescM').val();

                var image = $('#txtImage').val();
                var order = parseInt($('#txtOrderM').val());
                var homeOrder = $('#txtHomeOrderM').val();

                var seoKeyword = $('#txtSeoKeywordM').val();
                var seoMetaDescription = $('#txtSeoDescriptionM').val();
                var seoPageTitle = $('#txtSeoPageTitleM').val();
                var seoAlias = $('#txtSeoAliasM').val();
                var status = $('#ckStatusM').prop('checked') == true ? 1 : 0;
                var showHome = $('#ckShowHomeM').prop('checked');
                $.ajax({
                    type: "POST",
                    url: "/Admin/ProductCategory/SaveEntity",
                    data: {
                        Id: id,
                        Name: name,
                        Description: description,
                        ParentId: parentId,
                        HomeOrder: homeOrder,
                        SortOrder: order,
                        HomeFlag: showHome,
                        Image: image,
                        Status: status,
                        SeoPageTitle: seoPageTitle,
                        SeoAlias: seoAlias,
                        SeoKeywords: seoKeyword,
                        SeoDescription: seoMetaDescription
                    },
                    dataType: "json",
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function (response) {
                        core.notify('Save or update success', 'success');
                        $('#modal-add-edit').modal('hide');

                        resetFormMaintainance();

                        core.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        core.notify('Has an error in update progress', 'error');
                        core.stopLoading();
                    }
                });
            }
            return false;

        });

        $('body').on('click', '#btnDelete', function (e) {
            e.preventDefault();
            var that = $('#hidIdM').val();
            core.confirm('Are you sure to delete?', function () {
                $.ajax({
                    type: "POST",
                    url: "/Admin/ProductCategory/Delete",
                    data: {id: that},
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function (response) {
                        core.notify('Deleted success', 'success');
                        core.stopLoading();
                        loadData();
                    },
                    error: function (status) {
                        core.notify('Has an error in deleting progress', 'error');
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
        });
    }

    function loadData() {
        $.ajax({
            url: '/admin/ProductCategory/GetAll',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                var data = [];
                $.each(res, function (i, item) {
                    data.push({
                        id: item.Id,
                        text: item.Name,
                        parentId: item.ParentId,
                        sortOrder: item.SortOrder,

                    });
                });
                var treeArr = core.unflattern(data);

                treeArr.sort(function (a, b) {
                    return a.sortOrder - b.sortOrder;
                });

                //var $tree = $('#treeProductCategory');
                $('#treeProductCategory').tree({
                    data: treeArr,
                    dnd: true,
                    onContextMenu: function (e, node) {
                        e.preventDefault();
                        // select the node
                        $('#hidIdM').val(node.id);
                        // display context menu
                        $('#contextMenu').menu('show', {
                            left: e.pageX,
                            top: e.pageY
                        });
                    },
                    onDrop: function (target, source, point) {
                        console.log(target);
                        console.log(source);
                        console.log(point);
                        var targetNode = $(this).tree('getNode', target);
                        if (point === 'append') {
                            var children = [];
                            $.each(targetNode.children, function (i, item) {
                                children.push({
                                    key: item.id,
                                    value: i
                                });
                            });

                            //update to database

                            $.ajax({
                                url: '/admin/ProductCategory/UpdateParentId',
                                type: 'post',
                                dataType: 'json',
                                data: {
                                    sourceId: source.id,
                                    targetId: targetNode.id,
                                    items: children
                                },
                                success: function (res) {
                                    loadData();
                                }
                            });
                        } else if (point == 'top' || point == 'bottom') {
                            $.ajax({
                                url: '/admin/ProductCategory/ReOrder',
                                type: 'post',
                                data: {
                                    sourceId: source.id,
                                    targetId: targetNode.id
                                },
                                success: function () {
                                    loadData();
                                }
                            });

                        }
                    }
                });
            }
        });
    }

    function initTreeDropDownCategory(selectId) {

        $.ajax({
            url: '/Admin/ProductCategory/GetAll',
            type: 'GET',
            dataType: 'json',
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

                if (selectId != undefined)
                    $('#ddlCategoryIdM').combotree('setValue', selectId);
            }
        });
    }

    function resetFormMaintainance() {
        $('#hidIdM').val(0);
        $('#txtNameM').val('');
        initTreeDropDownCategory('');

        $('#txtDescM').val('');
        $('#txtOrderM').val('');
        $('#txtHomeOrderM').val('');
        $('#txtImage').val('');

        $('#txtMetakeywordM').val('');
        $('#txtMetaDescriptionM').val('');
        $('#txtSeoPageTitleM').val('');
        $('#txtSeoAliasM').val('');

        $('#ckStatusM').prop('checked', true);
        $('#ckShowHomeM').prop('checked', false);
    }
}