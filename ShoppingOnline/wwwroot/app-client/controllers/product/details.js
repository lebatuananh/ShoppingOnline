var ProductDetailController = function () {
    this.initialize = function () {
        registerEvents();
    }

    function registerEvents() {

        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                qty: {
                    required: true,
                    number: true,
                    min: 1
                }
            }
        });

        $('#btnAddToCart').on('click', function (e) {
            e.preventDefault();

            if ($('#frmMaintainance').valid()) {
                var productId = parseInt($(this).data('id'));
                var sizeId = parseInt($('#ddlSizes').val());
                var colorId = parseInt($('#ddlColors').val());
                var quantity = parseInt($('#qty').val());

                $.ajax({
                    url: '/Cart/AddToCart',
                    type: 'post',
                    data: {
                        productId: productId,
                        colorId: colorId,
                        sizeId: sizeId,
                        quantity: quantity,
                    },
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function () {
                        loadHeaderCart();

                        core.notify('Product added to your cart', 'success');
                        core.stopLoading();
                    },
                    error: function () {
                        core.notify('Has an error', 'error');

                        core.stopLoading();

                    }
                });
            }
        });


    }

    function loadHeaderCart() {
        $("#headerCart").load("/AjaxContent/HeaderCart");
    }
}