var FeedbackController = function () {
  this.initialize=function () {
      loadData();
      registerEvents();
  };  
  function registerEvents() {
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
  }

    function loadData(isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/feedback/GetAllPaging",
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
                            Email: item.Email,
                            Id: item.Id,
                            Message: item.Message,
                            CreatedDate: core.dateFormatSubStr(item.DateCreated)
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
};