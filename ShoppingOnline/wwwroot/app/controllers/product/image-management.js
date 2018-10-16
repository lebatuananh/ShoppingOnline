var ImageManagement = function () {

    //var images = [];

    this.initialize = function () {
        registerEvents();
    };

    function registerEvents() {

        $('body').on('click', '.btn-images', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            $('#hidId').val(id);
            clearFileInput();
            loadImages();
            $('#modal-image-manage').modal('show');
        });

        $('#fileImage').on('change', function () {
            var fileUploads = $(this).get(0);
            var files = fileUploads.files;
            var data = new FormData();

            var ValidImageTypes = ["image/gif", "image/jpeg", "image/png"];
            if ($.inArray(files[0].type, ValidImageTypes) > 0) {
                for (var i = 0; i < files.length; i++) {
                    data.append(files[i].name, files[i]);
                }
                $.ajax({
                    type: 'post',
                    url: '/Admin/Upload/UploadImage',
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (res) {
                        clearFileInput();
                        $('#image-list').append('<div class="col-md-3"><img width="100" data-path="' + res + '" src="' + res + '"><br/><button class="btn-delete-image"><i class="fa fa-trash" aria-hidden="true"></i></button></div>')
                        core.notify('Image uploaded successfully', 'success');
                    },
                    error: function () {
                        core.notify('There was error uploading files!', 'error');
                        clearFileInput();
                    }
                });
            }
            else {
                core.notify('There was error uploading files!', 'error');
                clearFileInput();
            }
        });

        $('#btnSaveImages').on('click', function () {

            var imageList = [];
            $.each($('#image-list').find('img'), function (i, item) {
                imageList.push($(this).data('path'));
            });
            $.ajax({
                url: '/Admin/Product/SaveImages',
                type: 'post',
                data: {
                    productId: $('#hidId').val(),
                    paths: imageList
                },
                success: function () {
                    $('#modal-image-manage').modal('hide');
                    $('#image-list').html('');
                    clearFileInput();
                    core.notify('Save successfully', 'success');
                }

            });
        });

        $('body').on('click', '.btn-delete-image', function (e) {

            e.preventDefault();
            $(this).closest('div').remove();
        });
    }

    function loadImages() {
        $.ajax({
            url: '/Admin/Product/GetImages',
            type: 'get',
            dataType: 'json',
            data: {
                productId: $('#hidId').val()
            },
            success: function (res) {
                var render = '';

                $.each(res, function (i, item) {
                    render += '<div class="col-md-3"><img width="100" data-path="' + item.Path + '" src="' + item.Path + '"><br/><button class="btn-delete-image"><i class="fa fa-trash" aria-hidden="true"></i></button></div>';
                });

                $('#image-list').html(render);
                clearFileInput();

            }
        });
    }

    function clearFileInput() {
        $('#fileImage').val(null);
    }
}