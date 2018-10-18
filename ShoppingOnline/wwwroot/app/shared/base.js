var BaseController = function () {

    this.initialize = function () {
        core.loadAnnouncement();
        registerEvents();
    }

    function registerEvents() {
        $('body').on('click', '.announcement-read', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "post",
                url: "/Admin/Announcement/MarkAsRead",
                data: {id: that},
                beforeSend: function () {
                    core.startLoading();
                },
                success: function () {
                    core.loadAnnouncement();
                    core.notify('Read', 'success');
                    core.stopLoading();

                },
                error: function () {
                    core.notify('Has an error progress', 'error');
                    core.stopLoading();
                }
            });
        });
    }
};