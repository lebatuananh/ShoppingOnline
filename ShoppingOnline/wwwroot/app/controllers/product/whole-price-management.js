var WholePriceManagement = function () {
    this.initialize = function () {
        registerEvents();
    }

    function registerEvents() {

        $('body').on('click', '.btn-whole-price', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            $('#hidId').val(id);
            loadWholePrices();
            $('#modal-whole-price').modal('show');

        });

        $('#btn-add-whole-price').on('click', function () {

            var template = $('#template-table-whole-price').html();
            var render = Mustache.render(template, {
                Id: 0,
                FromQuantity: 0,
                ToQuantity: 0,
                Price: 0
            });

            $('#table-content-whole-price').append(render);
        });

        $('body').on('click', '.btn-delete-whole-price', function (e) {

            e.preventDefault();
            $(this).closest('tr').remove();

        });

        $('#btnSaveWholePrice').on('click', function () {

            var listWholePrices = [];

            $.each($('#table-content-whole-price').find('tr'), function (i, item) {
                listWholePrices.push({
                    Id: $(item).data('Id'),
                    ProductId: $('#hidId').val(),
                    FromQuantity: $(item).find('.txtQuantityFrom').val(),
                    ToQuantity: $(item).find('.txtQuantityTo').val(),
                    Price: $(item).find('.txtWholePrice').val()
                });
            });

            $.ajax({
                url: '/Admin/Product/SaveWholePrice',
                type: 'post',
                dataType: 'json',
                data: {
                    productId: $('#hidId').val(),
                    wholePrices: listWholePrices
                },
                success: function (res) {
                    $('#modal-whole-price').modal('hide');
                    $('#table-content-whole-price').html('');
                    core.notify('Save successfully', 'success');
                },
                error: function (res) {
                    core.notify('Has an error', 'error');
                }
            });
            return false;
        });

    }

    function loadWholePrices() {

        $.ajax({

            url: '/Admin/Product/GetWholePrices',
            type: 'get',
            dataType: 'json',
            data: {
                productId: $('#hidId').val()
            },
            success: function (res) {

                var render = '';
                var template = $('#template-table-whole-price').html();

                $.each(res, function (i, item) {

                    render += Mustache.render(template, {
                        Id: item.Id,
                        FromQuantity: item.FromQuantity,
                        ToQuantity: item.ToQuantity,
                        Price: item.Price
                    });
                });

                $('#table-content-whole-price').html(render);

            }
        });

    }
}