var ImageManagement = function () {

    //var images = [];

    var sourceImage;

    var options = {
        aspectRatio: 1 / 1,
        minCropBoxWidth: 473,
        minCropBoxHeight: 473,
        zoomOnWheel: false,
        autoCrop: false
    };

    this.initialize = function () {
        registerEvents();
    }

    function registerEvents() {

        $('body').on('click', '.btn-images', function (e) {
            e.preventDefault();
            $('#image-detail').html('');
            $('#image-detail').hide();
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
                    success: function (path) {
                        clearFileInput();
                        sourceImage = path;
                        $('#image-detail').html('<img data-path="' + path + '" src="' + path + '"  id="image" style="max-width: 100%">');
                        $('#image-detail').show();
                        cropImage();
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
            url: "/Admin/Upload/Details",
            contentType: false,
            processData: false,
            data: data,
            success: function (res) {
                var img = '<div class="col-md-3"><img width="100" data-path="' + res + '" src="' + res + '"><br/><button class="btn-delete-image"><i class="fa fa-trash" aria-hidden="true"></i></button></div>';
                $('#image-list').append(img);

            },
            error: function () {
                core.notify('Has an error in process', 'error');
            }
        });

    }
}