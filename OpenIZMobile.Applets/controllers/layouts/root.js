﻿/// <reference path="~/js/openiz-model.js"/>

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
var layoutApp = angular.module('layout', ['openiz']).run(function ($rootScope) {

    $rootScope.system = {};
    $rootScope.system.config = {};
    $rootScope.system.config.realmName = OpenIZ.Configuration.getRealm();
    $rootScope.page = {
        title: OpenIZ.App.getCurrentAssetTitle(),
        loadTime: new Date()
    };

    // Get current session
    $rootScope.session = OpenIZ.Authentication.getSession();

  
});

// Configure the safe ng-urls
layoutApp.config(['$compileProvider', function ($compileProvider) {
    $compileProvider.aHrefSanitizationWhitelist(/^\s*(http|tel):/);
    $compileProvider.imgSrcSanitizationWhitelist(/^\s*(http|tel):/);
}]);

angular.element(document).ready(function () {
    $("[data-toggle=popover]").popover({ container: 'body' });
    $('[data-toggle=popover]').on('shown.bs.popover', function () {
        $('[data-toggle=popover]').not(this).popover('hide');
    })
    $('#initialBlock').remove();
    $('#waitModal').removeClass('in');
    $('#waitModal').removeAttr('style');
    //OpenIZ.locale = OpenIZ.Localization.getLocale();
});