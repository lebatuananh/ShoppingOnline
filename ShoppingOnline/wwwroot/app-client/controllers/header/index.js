var HeaderController = function () {
    this.initialize = function () {
        registerEvents();
    };
    function registerEvents() {
        $.widget("custom.catcomplete", $.ui.autocomplete, {
            _create: function () {
                this._super();
                this.widget().menu("option", "items", "> :not(.ui-autocomplete-category)");
            },
            _renderMenu: function (ul, items) {
                var that = this,
                    currentCategory = "";
                $.each(items, function (index, item) {
                    var li;
                    if (item.CategoryAlias != currentCategory) {
                        ul.append("<li class='ui-autocomplete-category'>" + item.Category + "</li>");
                        currentCategory = item.CategoryAlias;
                    }
                    li = that._renderItemData(ul, item);
                    if (item.CategoryAlias) {
                        li.find('div').remove();
                        li.attr("aria-label", item.CategoryAlias + " : " + item.Name);
                        $('<div>' + item.Name + '</div>').appendTo(li);
                    }
                });
            }
        });

        $('input[name="keyword"]').catcomplete({
            minLength: 1,
            source: function (request, response) {
                $.ajax({
                    url: "/Home/SearchQuery/",
                    dataType: "json",
                    data: {
                        q: request.term
                    },
                    success: function (data) {
                        response(data);
                    }
                });
            },
            select: function (event, ui) {
                $('input[name="keyword"]').val(ui.item.Name)
                window.location.href = location.protocol + "//" + location.host + "/" + ui.item.Alias + "-p." + ui.item.Id + ".html"
                return false;
            }
        })
    }
};