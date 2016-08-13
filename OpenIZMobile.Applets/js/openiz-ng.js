﻿/// <reference path="angular.min.js"/>

/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-7-18
 */

/// <reference path="openiz.js"/>

/**
 * Open IZ Localization for angular
 */

angular.module('openiz', [])
    // Localization service
    .provider('localize', function localizeProvider() {

        this.$get = ['$rootScope', '$filter', function ($rootScope, $filter) {
            var localize = {
                dictionary: OpenIZ.Localization.getStrings(OpenIZ.Localization.getLocale()),
                /**
                 * @summary Sets the locale of the user interface 
                 */
                setLanguage: function (locale) {
                    if (OpenIZ.Localization.getLocale() != locale) {
                        OpenIZ.Localization.setLocale(locale);
                        localize.dictionary = OpenIZ.Localization.getStrings(locale);
                        //$rootScope.$broadcast('localizeResourcesUpdated');
                        //$window.location.reload();
                        //$state.reload();
                        //$rootScope.$applyAsync();
                    }
                },
                /**
                 * @summary Gets the specified locale key
                 */
                getString: function (key) {

                    // make sure we always have the latest locale
                    //localize.dictionary = OpenIZ.Localization.getStrings(OpenIZ.Localization.getLocale());

                    var entry = localize.dictionary[key];
                    if (entry != null)
                        return entry;
                    else {
                        var oiz = OpenIZ.Localization.getString(key);
                        if (oiz == null)
                            return key;
                        return oiz;
                    }
                }
            };
            return localize;
        }];
    })
    /** 
     * @summary Filter for localization
     * @use {{ KEY | i18n }}
     */
    .filter('i18n', ['$rootScope', 'localize', function ($rootScope, localize) {
        var filterFn = function (key) {
            return localize.getString(key);
        };
        filterFn.$stateful = false;
        return filterFn;
    }])
    .filter('oizEntityIdentifier', function () {
        return function (modelValue) {
            if (modelValue.NID !== undefined)
                return modelValue.NID.value;
            else
                for (var k in modelValue)
                    return modelValue.NID;

        };
    })
    .filter('oizEntityName', function () {
        return function (modelValue) {
            return OpenIZ.Util.renderName(modelValue);
        }
    })
    .directive('oizTag', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ctrl) {
                ctrl.$parsers.unshift(tagParser);
                ctrl.$formatters.unshift(tagFormatter);
                function tagParser(viewValue) {
                    return String(viewValue).split(',');
                }
                function tagFormatter(viewValue) {
                    if (typeof (viewValue) === Array)
                        return viewValue.join(viewView)
                    return viewValue;
                }
            }
        }
    })
    .directive('oizEntitysearch', function ($timeout) {
        return {
            link: function (scope, element, attrs, ctrl) {
                $timeout(function () {


                    var modelType = $(element).attr('oiz-entitysearch');
                    var filterString = $(element).attr('data-filter');

                    var filter = {};
                    if (filterString !== undefined)
                        filter = JSON.parse(filterString);
                    filter.statusConcept = 'C8064CBD-FA06-4530-B430-1A52F1530C27'

                    // Add appropriate styling so it looks half decent
                    $(element).attr('style', 'width:100%; height:100%');
                    // Bind select 2 search
                    $(element).select2({
                        ajax: {
                            url: "/__ims/" + modelType,
                            dataType: 'json',
                            delay: 500,
                            method: "GET",
                            data: function (params) {
                                filter["name.component.value"] = "~" + params.term;
                                filter["_count"] = 5;
                                filter["_offset"] = 0;
                                return filter;
                            },
                            processResults: function (data, params) {
                                params.page = params.page || 0;
                                return {
                                    results: $.map(data.item, function (o) {
                                        o.text = o.text || OpenIZ.Util.renderName(o.name.OfficialRecord);
                                        return o;
                                    }),
                                    pagination: {
                                        more: data.offset + data.count < data.total
                                    }
                                };
                            },
                            cache: true
                        },
                        escapeMarkup: function (markup) { return markup; }, // Format normally
                        minimumInputLength: 4,
                        templateSelection: function (result) {
                            if (result.name != null)
                                return OpenIZ.Util.renderName(result.name.OfficialRecord);
                            else
                                return result.text;
                        },
                        templateResult: function (result) {
                            if (result.name != null)
                                return "<div class='label label-default'>" +
                                    result.typeConceptModel.name[OpenIZ.Localization.getLocale()] + "</div> " + OpenIZ.Util.renderName(result.name.OfficialRecord);
                            else
                                return result.text;
                        }
                    });
                });
            }
        };
    })
    .directive('oizCollapseindicator', function () {
        return {
            link: function (scope, element, attrs, ctrl) {
                $(element).on('hide.bs.collapse', function () {
                    var indicator = $(this).attr('data-oiz-chevron');
                    $(indicator).removeClass('glyphicon-chevron-down');
                    $(indicator).addClass('glyphicon-chevron-right');
                });
                $(element).on('show.bs.collapse', function () {
                    var indicator = $(this).attr('data-oiz-chevron');
                    $(indicator).addClass('glyphicon-chevron-down');
                    $(indicator).removeClass('glyphicon-chevron-right');
                });
            }
        };
    });