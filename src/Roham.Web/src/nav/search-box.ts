import 'jquery';
import 'bootstrap';

export class SearchBox {
    query: string;

    constructor() {
        console.log('SearchBox initialize');
    }

    attached() {
        this.configureSearch(jQuery);
    }

    private configureSearch = ($: JQueryStatic) => {
        /*Configure Search Popover*/
        var elm: any = $('.search-box-trigger[data-toggle="popover"]');
        if (elm.length == 0) {
            console.log('search-box-trigger element not found');
        }

        $('body').on('click', function (e) {
            $('[data-toggle="popover"]').each(function () {
                //the 'is' for buttons that trigger popups
                //the 'has' for icons within a button that triggers a popup
                var $elm: any = $(this);
                if (!$elm.is(e.target) && $elm.has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
                    $elm.popover('hide');
                }
            });
        });

        elm.popover(
            {
                trigger: 'manual',
                container: 'body',
                html: true,
                template: '<div class="popover search-popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>',
                content: function () {
                    return $('#search-box-container').html();
                }
            }).click(function (e) {
                e.preventDefault();
                var self: any = $(this);

                self.popover('toggle');
                $('.popover').css({ 'left': '0px', 'width': '100%' });
                $('.search-box-popover input[type="search"]').focus();

                /* Event handlers*/
                $('.search-box-popover input[type="search"]').focusin(function () {
                    $('.search-box-popover .search-close').show();
                });

                $('.search-box-popover input[type="search"]').keyup(function (e) {
                    var keycode = (e.keyCode ? e.keyCode : e.which);
                    if (keycode == 13) { /*enter*/
                        var q = $.trim($(this).val());
                        if (q != undefined && q != '') {
                            $('.search-box-popover input[type="search"]').blur();
                            self.popover('toggle');
                            console.log('you searched ' + q);
                        }
                    }

                    if (keycode == 27) { /*escape*/
                        var q = $.trim($(this).val());
                        if (q == undefined || $.trim(q) == '') {
                            self.popover('toggle');
                        }
                    }
                });

                $('.search-box-popover .search-close').click(function () {
                    self.popover('toggle');
                });

            });
    }

}