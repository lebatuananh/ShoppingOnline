var QuantityManagement = function () {

    var cacheObj = {
        colors: [],
        sizes: []
    }

    this.initialize = function () {

        loadColors();
        loadSizes();
        registerEvents();

    }

    function registerEvents() {

        $('body').on('click', '.btn-quantity', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $('#hidId').val(that);
            loadQuantities();
            $('#modal-quantity-management').modal('show');
        });

        $('#btn-add-quantity').on('click', function () {
            var template = $('#template-table-quantity').html();

            var render = Mustache.render(template, {
                Id: 0,
                Colors: getColorOptions(null),
                Sizes: getSizeOptions(null),
                Quantity: 0
            });

            $('#table-quantity-content').append(render);
        });

        $('body').on('click', '.btn-delete-quantity', function (e) {
            e.preventDefault();
            $(this).closest('tr').remove();
        });

        $('#btnSaveQuantity').on('click', function (e) {
            var quantityList = [];
            $.each($('#table-quantity-content').find('tr'), function (i, item) {
                quantityList.push({
                    Id: $(item).data('Id'),
                    ProductId: $('#hidId').val(),
                    Quantity: $(item).find('input.txtQuantity').first().val(),
                    SizeId: $(item).find('select.ddlSizeId').first().val(),
                    ColorId: $(item).find('select.ddlColorId').first().val(),
                });
            });
            $.ajax({
                url: '/Admin/Product/SaveQuantities',
                data: {
                    productId: $('#hidId').val(),
                    productQuantities: quantityList
                },
                type: 'POST',
                dataType: 'JSON',
                success: function (res) {
                    $('#modal-quantity-management').modal('hide');
                    $('#table-quantity-content').html('');
                    core.notify('Save successful', 'success');
                },
                error: function (res) {
                    core.notify('Has an error', 'error');
                }

            });
        });
    }

    function loadSizes() {
        $.ajax({
            url: '/Admin/Bill/GetSizes',
            type: 'Get',
            dataType: 'Json',
            success: function (res) {
                cacheObj.sizes = res;
            },
            error: function (res) {
                core.notify('Has an error', 'error');
            }
        });
    }

    function loadColors() {
        $.ajax({
            url: '/Admin/Bill/GetColors',
            type: 'Get',
            dataType: 'JSON',
            success: function (res) {
                cacheObj.colors = res;
            },
            error: function (res) {
                core.notify('Has an error', 'error');
            }
        });
    }

    function loadQuantities() {

        $.ajax({

            url: '/admin/product/GetQuantities',
            type: 'GET',
            dataType: 'JSON',
            data: {
                productId: $('#hidId').val()
            },
            success: function (res) {
                var render = '';
                var template = $('#template-table-quantity').html();
                $.each(res, function (i, item) {
                    render += Mustache.render(template, {
                        Id: item.Id,
                        Colors: getColorOptions(item.ColorId),
                        Sizes: getSizeOptions(item.SizeId),
                        Quantity: item.Quantity
                    });
                });
                $('#table-quantity-content').html(render);
            }

        });
    }

    function getColorOptions(selectedId) {
        var colors = '<select class="form-control ddlColorId" >';

        $.each(cacheObj.colors, function (i, item) {
            if (selectedId == item.Id) {
                colors += '<option value="' + item.Id + '" selected="select">' + item.Name + '</option>'
            } else {
                colors += '<option value="' + item.Id + '">' + item.Name + '</option>'
            }
        });
        colors += '</option>';
        return colors;
    }

    function getSizeOptions(selectedId) {

        var sizes = '<select class="form-control ddlSizeId">';

        $.each(cacheObj.sizes, function (i, item) {

            if (selectedId == item.Id)
                sizes += '<option selected="select" value="' + item.Id + '">' + item.Name + '</option>';
            else
                sizes += '<option value="' + item.Id + '">' + item.Name + '</option>';
        });

        sizes += '</select>'

        return sizes;
    }
}