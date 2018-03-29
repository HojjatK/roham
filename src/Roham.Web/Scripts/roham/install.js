/// <reference path="../toastr.js" />
$(function () {
    console.log('Install ready called');

    /* Form submit */
    $(document).on('submit', 'form', function () {
        var buttons = $(this).find('[type="submit"]');
        if ($(this).valid()) {
            buttons.each(function (btn) {
                $(buttons[btn]).prop('disabled', true);
            });
        } else {
            buttons.each(function (btn) {
                $(buttons[btn]).prop('disabled', false);
            });
        }
    });

    /* Setup toast, popover and tooltip */
    toastr.options.closeButton = true;
    toastr.options.positionClass = 'toast-bottom-right';

    $('[data-toggle="popover"]').popover();
    $('[data-toggle="tooltip"]').tooltip();
    
    /* Site settings */
    $('#ConfirmAdminPassword').focusout(validatePasswords);
    $('#AdminPassword').focusout(validatePasswords);

    function validatePasswords() {
        var passEl = $('#AdminPassword');
        var confirmPassEl = $('#ConfirmAdminPassword');
        if (passEl.val().length >= 8 && confirmPassEl.val().length >= 8 && passEl.val() === confirmPassEl.val()) {
            passEl.css('color', 'green');
            confirmPassEl.css('color', 'green');
        } else {
            passEl.css('color', 'red');
            confirmPassEl.css('color', 'red');
        }
    }

    /* Database provider settings */
    $('#ddl-dbprovider').change(function () {
        changeDbProvider();
    });
    initDbProvider();

    function initDbProvider() {
        if ($('#chk-db-advanced').is(':checked')) {
            $('#dbsettings-advanced').show();
            $('#dbsettings').hide();
        } else {
            $('#dbsettings-advanced').hide();
            $('#dbsettings').show();
        }

        if ($('#db-connstr').is(":visible")) {            
            $('#dbsettings input.required').removeAttr('required');
        } else {
            $('#dbsettings input.required').attr('required', 'required');
        }

        $('#ddl-sqlauth').change(function () {
            changeDbProvider();
        });

        $('#chk-db-advanced').click(function () {
            $('#dbsettings-advanced').toggle();
            $('#dbsettings').toggle();

            if ($('#db-connstr').is(":visible")) {
                buildDbConnectionString();
                $('#dbsettings input.required').removeAttr('required');
            } else {
                $('#dbsettings input.required').attr('required', 'required');
            }
        });

        if ($('#db-connstr').is(":visible")) {
            buildDbConnectionString();
        }
    }

    function changeDbProvider() {        
        $.ajax({
            url: '/admin/install/dbprovider',
            data: $('#install-form').serialize(),
            type: 'POST',
            dataType: 'html'
        })
        .success(function (result) {
            $('#db-provider').html(result);            
            initDbProvider();            
        })
        .error(function (xhr, status) {
            notifyError(xhr, 'Changing database provider failed');
        })
    }

    function buildDbConnectionString() {
        $.ajax({
            url: '/admin/install/dbconnstr',
            data: $('#install-form').serialize(),
            type: 'POST',
            dataType: 'html'
        })
        .success(function (result) {
            $('#db-connstr').val(result);
        })
        .error(function (xhr, status) {
            notifyError(xhr, 'Building connection string failed');            
        })
    }

    /* Cache provider settings*/
    $('#ddl-cacheprovider').change(function () {
        changeCacheProvider();
    });
    initCacheProvider();

    function initCacheProvider() {
        if ($('#chk-cache-advanced').is(':checked')) {
            $('#cachesettings-advanced').show();
            $('#cachesettings').hide();
        } else {
            $('#cachesettings-advanced').hide();
            $('#cachesettings').show();
        }

        if ($('#cache-connstr').is(":visible")) {            
            $('#cachesettings input.required').removeAttr('required');
        } else {
            $('#cachesettings input.required').attr('required', 'required');
        }

        $('#chk-cache-advanced').click(function () {
            $('#cachesettings-advanced').toggle();
            $('#cachesettings').toggle();

            if ($('#cache-connstr').is(":visible")) {
                buildCacheConnectionString();
                $('#cachesettings input.required').removeAttr('required');
            } else {
                $('#cachesettings input.required').attr('required', 'required');
            }
        });

        $('#test-cache-connect').click(function () {
            testCacheConnection();
        });
    }

    function changeCacheProvider() {
        var cacheprovider = $('#ddl-cacheprovider').val();
        $.ajax({
            url: '/admin/install/cacheprovider',
            data: $('#install-form').serialize(),
            type: 'POST',
            dataType: 'html'
        })
        .success(function (result) {
            $('#cache-provider').html(result);
            initCacheProvider();
        })
        .error(function (xhr, status) {
            notifyError(xhr, 'Changing cache provider failed');            
        })
    }

    function buildCacheConnectionString() {
        $.ajax({
            url: '/admin/install/cacheconnstr',
            data: $('#install-form').serialize(),
            type: 'POST',
            dataType: 'html'
        })
        .success(function (result) {
            $('#cache-connstr').val(result);
        })
        .error(function (xhr, status) {
            notifyError(xhr, 'Building cache connection string failed');
        })
    }

    function testCacheConnection() {
        $("#install-busy").show();
        $.ajax({
            url: '/admin/install/cacheconnect',
            data: $('#install-form').serialize(),
            type: 'POST',
            dataType: 'html'
        })
      .success(function (result) {
          $("#install-busy").hide();
          $('#test-cache-connect').removeClass("btn-default btn-success btn-danger");
          $('#test-cache-connect').addClass("btn-success");
      })
      .error(function (xhr, status) {
          $("#install-busy").hide();
          $('#test-cache-connect').removeClass("btn-default btn-success btn-danger");
          $('#test-cache-connect').addClass("btn-danger");
          notifyError(xhr, 'Cache connection failed');
      })
    }

    /*smtp settings*/
    initSmtpSettings();
    $('#ddl-emailprovider').change(function () {
        initSmtpSettings();
    });    
    $('#test-smtp-connect').click(function () {
        testSmtpConnection();
    });
    $('#send-email').click(function () {
        testSmtpSendEmail();
    });

    function initSmtpSettings() {
        var emailProvider = $('#ddl-emailprovider').val();
        if (emailProvider == 'smtp') {
            $('#smtp-settings').show();
        } else {
            $('#smtp-settings').hide();
        }
        if ($('#smtp-settings').is(":visible")) {
            $('#smtp-settings input.required').attr('required', 'required');
        } else {
            $('#smtp-settings input.required').removeAttr('required');
        }
    }    
    
    function testSmtpConnection() {
        $("#install-busy").show();
        $.ajax({
            url: '/admin/install/smtpconnect',
            data: $('#install-form').serialize(),
            type: 'POST',
            dataType: 'html'
        })
      .success(function (result) {
          $("#install-busy").hide();
          $('#test-smtp-connect').removeClass("btn-default btn-success btn-danger");
          $('#test-smtp-connect').addClass("btn-success");
      })
      .error(function (xhr, status) {
          $("#install-busy").hide();
          $('#test-smtp-connect').removeClass("btn-default btn-success btn-danger");
          $('#test-smtp-connect').addClass("btn-danger");
          notifyError(xhr, 'Smtp connection failed');
      })
    }

    function testSmtpSendEmail() {
        $("#install-busy").show();
        $.ajax({
            url: '/admin/install/smtptestemail',
            data: $('#install-form').serialize(),
            type: 'POST',
            dataType: 'html'
        })
      .success(function (result) {
          $("#install-busy").hide();
          $('#test-smtp-email').removeClass("btn-default btn-success btn-danger");
          $('#test-smtp-email').addClass("btn-success");
      })
      .error(function (xhr, status) {
          $("#install-busy").hide();
          $('#test-smtp-email').removeClass("btn-default btn-success btn-danger");
          $('#test-smtp-email').addClass("btn-danger");
          notifyError(xhr, 'Sending test email failed');
      })
    }

    function notifyError(xhr, title) {
        console.log(xhr.statusText + ': ' + xhr.responseText);
        toastr.error(xhr.statusText + '<br/> ' + xhr.responseText, title);
    }
});