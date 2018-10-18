var core = {
    getParamUrl: function getParamUrl(name, url) {
        if (!url) url = location.href;
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(url);
        return results == null ? null : decodeURIComponent(results[1]);
    },
    configs: {
        pageSize: 10,
        pageIndex: 1
    },
    notify: function (message, type) {
        $.notify(message, {
            // whether to hide the notification on click
            clickToHide: true,
            // whether to auto-hide the notification
            autoHide: true,
            // if autoHide, hide after milliseconds
            autoHideDelay: 5000,
            // show the arrow pointing at the element
            arrowShow: true,
            // arrow size in pixels
            arrowSize: 5,
            // position defines the notification position though uses the defaults below
            position: '...',
            // default positions
            elementPosition: 'top right',
            globalPosition: 'top right',
            // default style
            style: 'bootstrap',
            // default class (string or [string])
            className: type,
            // show animation
            showAnimation: 'slideDown',
            // show animation duration
            showDuration: 400,
            // hide animation
            hideAnimation: 'slideUp',
            // hide animation duration
            hideDuration: 200,
            // padding between element and notification
            gap: 2
        });
    },

    confirm: function (message, okCallback) {
        bootbox.confirm({
            message: message,
            buttons: {
                confirm: {
                    label: 'OK',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'Cancel',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                if (result === true) {
                    okCallback();
                }
            }
        });
    },

    dateFormatJson: function (datetime) {
        if (datetime == null || datetime == '')
            return '';
        var newdate = new Date(parseInt(datetime.substr(6)));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        var hh = newdate.getHours();
        var mm = newdate.getMinutes();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        if (hh < 10)
            hh = "0" + hh;
        if (mm < 10)
            mm = "0" + mm;
        return day + "/" + month + "/" + year;
    },

    dateFormatSubStr: function (datetime) {
        if (datetime == null || datetime == '')
            return '';
        var newdate = new Date(datetime.substr(0, 10));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        //var hh = newdate.getHours();
        //var mm = newdate.getMinutes();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        //if (hh < 10)
        //    hh = "0" + hh;
        //if (mm < 10)
        //    mm = "0" + mm;
        return day + "/" + month + "/" + year;
    },

    dateTimeFormatJson: function (datetime) {
        if (datetime == null || datetime == '')
            return '';
        var newdate = new Date(parseInt(datetime.substr(6)));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        var hh = newdate.getHours();
        var mm = newdate.getMinutes();
        var ss = newdate.getSeconds();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        if (hh < 10)
            hh = "0" + hh;
        if (mm < 10)
            mm = "0" + mm;
        if (ss < 10)
            ss = "0" + ss;
        return day + "/" + month + "/" + year + " " + hh + ":" + mm + ":" + ss;
    },

    startLoading: function () {
        if ($('.dv-loading').length > 0)
            $('.dv-loading').removeClass('hide');
    },

    stopLoading: function () {
        if ($('.dv-loading').length > 0)
            $('.dv-loading')
                .addClass('hide');
    },

    getStatus: function (status) {
        if (status == 1)
            return '<span class="badge bg-green">Active</span>';
        else
            return '<span class="badge bg-red">InActive</span>';
    },

    formatNumber: function (number, precision) {
        if (!isFinite(number)) {
            return number.toString();
        }

        var a = number.toFixed(precision).split('.');
        a[0] = a[0].replace(/\d(?=(\d{3})+$)/g, '$&,');
        return a.join('.');
    },

    unflattern: function (arr) {
        var map = {};
        var roots = [];
        for (var i = 0; i < arr.length; i += 1) {
            var node = arr[i];
            node.children = [];
            map[node.id] = i; // use map to look-up the parents
            if (node.parentId !== null) {
                arr[map[node.parentId]].children.push(node);
            } else {
                roots.push(node);
            }
        }
        return roots;
    },

    loadAnnouncement: function () {
        $.ajax({
            type: "GET",
            url: "/admin/announcement/GetAllPaging",
            data: {
                page: core.configs.pageIndex,
                pageSize: core.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
                core.startLoading();
            },
            success: function (response) {

                var template = $('#announcement-template').html();
                var render = "";
                console.log(response);
                if (response.RowCount > 0) {
                    $('#announcementArea').show();
                    $.each(response.Results, function (i, item) {
                        render += Mustache.render(template, {
                            Content: item.Content,
                            Id: item.Id,
                            Title: item.Title
                        });
                    });
                    render += $('#announcement-tag-template').html();
                    $("#totalAnnouncement").text(response.RowCount);
                    if (render != undefined) {
                        $('#annoncementList').html(render);
                    }
                }
                else {
                    $('#announcementArea').hide();
                    $('#annoncementList').html('');
                }
                core.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    }
}

$(document).ajaxSend(function (e, xhr, options) {
    if (options.type.toUpperCase() == "POST" || options.type.toUpperCase() == "PUT") {
        var token = $('form').find("input[name='__RequestVerificationToken']").val();
        xhr.setRequestHeader("RequestVerificationToken", token);
    }
});