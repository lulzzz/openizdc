﻿/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('AppointmentSchedulerController', ['$scope', '$stateParams', '$rootScope', function ($scope, $stateParams, $rootScope) {
    $scope._isCalendarInitialized = false;

    // Scheduling assistant shown
    $('a[data-target="#schedulingAssistant"]').on('shown.bs.tab', function () {
        if (!$scope._isCalendarInitialized) {
            $scope._isCalendarInitialized = true;
            $("#schedulingAssistantCalendar").fullCalendar({
                defaultView: 'month',
                defaultDate: $scope.appointment != null ? $scope.appointment.actTime : OpenIZ.Util.toDateInputString($rootScope.page.loadTime),
                displayEventTime: false,
                editable: true,
                weekends: false,
                navLinks: false,
                height: "parent",
                businessHours: {
                    dow: [1, 2, 3, 4, 5],
                    start: '09:00',
                    end: '16:00'
                },
                header: {
                    left: "prev, today, next",
                    center: "title",
                    right:null
                },
                eventLimit: false, // allow "more" link when too many events
                eventSources: [
                    {
                        events: $scope.renderAppointments
                    }
                ]
            });
        }
    });

    // Modal shown
    $("#appointmentScheduler").on('show.bs.modal', function () {
        
        
        // Gather the care plan
        OpenIZ.CarePlan.getCarePlanAsync({
            query: "_patientId=" + $stateParams.patientId + "&_appointments=true",
            minDate: new Date().tomorrow(),
            maxDate: new Date().addDays(90),
            /** @param {OpenIZModel.Bundle} proposals */
            continueWith: function (proposals) {
                if (!proposals.item) // There are no proposals, at the end of the planning process
                {
                    OpenIZ.CarePlan.getActTemplateAsync({
                        templateId: "act.patientencounter.appointment",
                        continueWith: function (appointment) {
                            $scope.appointment = appointment;
                        },
                        onException: function (ex) {
                            if (ex.message)
                                alert(ex.message);
                            else
                                console.error(ex);
                        }
                    });
                }
                else {
                    // Grab the first appointment
                    $scope.appointments = proposals.item;
                    $scope.appointments.sort(
                              function (a, b) {
                                  return a.actTime > b.actTime ? 1 : -1;
                              }
                            );
                    $scope.appointment = $scope.appointments[0];
                    if (Array.isArray($scope.appointment.relationship.HasComponent))
                        $.each($scope.appointment.relationship.HasComponent, function (i, e) {
                            e._enabled = true;
                        });
                    else {
                        $scope.appointment.relationship.HasComponent = [
                            $scope.appointment.relationship.HasComponent
                        ];
                        $scope.appointment.relationship.HasComponent[0]._enabled = true;
                    }
                }

                // Updates the scheduling assistant view
                $scope.$watch('appointment.actTime', function (newvalue, oldvalue) {
                    if (newvalue != null && $scope._isCalendarInitialized &&
                        newvalue != oldvalue)
                        $("#schedulingAssistantCalendar").fullCalendar('select', newvalue);
                });

                $scope.$apply();
            },
            onException: function (ex) {
                if (ex.message)
                    alert(ex.message);
                else
                    console.error(ex);
            }
        });

    });

    $scope.renderAppointments = function (start, end, timezone, callback) {
        if ($scope.appointment == null) return;

        var appointments = [{
            id: $scope.appointment.id,
            title: OpenIZ.Localization.getString("locale.encounters.appointment.recommended"),
            backgroundColor: "rgba(75, 192, 192, 0.6)",
            textColor: "#fff",
            start: OpenIZ.Util.toDateInputString($scope.appointment.startTime),
            end: OpenIZ.Util.toDateInputString($scope.appointment.stopTime),
            rendering: 'background'
        },
        {
            id: $scope.appointment.id,
            title: OpenIZ.Localization.getString("locale.encounters.appointment.proposed"),
            backgroundColor: "rgba(75, 192, 192, 1.0)",
            textColor: "#fff",
            start: OpenIZ.Util.toDateInputString($scope.appointment.actTime),
            end: OpenIZ.Util.toDateInputString($scope.appointment.actTime)
        }];

        // Use the datawarehouse data
        OpenIZ.Material.findMaterialAsync({
            continueWith: function (materials) {
                var refs = {};
                for (var i in materials.item)
                    refs[materials.item[i].id] = OpenIZ.Util.renderName(materials.item[i].name.Assigned);

                OpenIZWarehouse.Adhoc.query({
                    martId: "oizcp",
                    queryId: "bymonth",
                    parameters: {
                        "act_date": [">" + OpenIZ.Util.toDateInputString($scope.appointment.startTime), "<" + OpenIZ.Util.toDateInputString($scope.appointment.stopTime)],
                        "location_id": $rootScope.session.entity.relationship.DedicatedServiceDeliveryLocation.target
                    },
                    /** @param {OpenIZModel.Material} state */
                    continueWith: function (data) {
                        var aptDates = {};
                        for (var apt in data) {
                            if (data[apt].product_id) {
                                var dateStr = OpenIZ.Util.toDateInputString(data[apt].act_date);
                                var dateApt = aptDates[dateStr];
                                if (dateApt)
                                    dateApt.title += "\r\n" + data[apt].acts + " " + OpenIZ.Util.renderName(refs[data[apt].product_id]);
                                else {
                                    dateApt = {
                                        title: OpenIZ.Localization.getString("locale.encounters.appointment.planned") + "\r\n" + data[apt].acts + " " + OpenIZ.Util.renderName(refs[data[apt].product_id]),
                                        start: OpenIZ.Util.toDateInputString(data[apt].act_date),
                                        end: OpenIZ.Util.toDateInputString(data[apt].act_date)
                                    };
                                    appointments.push(dateApt);
                                    aptDates[dateStr] = dateApt;
                                }
                            }
                        }

                        callback(appointments);
                    }
                });
            }
        });
        

    };

    /**
     * Schedule an appointment
     */
    $scope.scheduleAppointment = function (form) {
        if (form.$invalid) return;

        OpenIZ.App.showWait();
        try {
            var bundle = new OpenIZModel.Bundle();

            // schedule the appointment
            var encounter = new OpenIZModel.PatientEncounter($scope.appointment);
            encounter.moodConcept = OpenIZModel.ActMoodKeys.Appointment;
            encounter.startTime = encounter.stopTime = null;
            encounter.participation.Location = new OpenIZModel.ActParticipation({
                player: $rootScope.session.entity.relationship.DedicatedServiceDeliveryLocation.target
            });
            encounter.participation.Authororiginator = new OpenIZModel.ActParticipation({
                player: $rootScope.session.entity.id
            });

            bundle.item = [ encounter ];

            // Iterate through the has-components and turn them to INT or remove them if not selected
            if (!Array.isArray(encounter.relationship.HasComponent))
                encounter.relationship.HasComponent = [encounter.relationship.HasComponent];

            for (var i in encounter.relationship.HasComponent) {
                /** @type {OpenIZModel.ActParticipation} */
                var actRelationship = encounter.relationship.HasComponent[i];
                if (!actRelationship._enabled) {
                    encounter.relationship.HasComponent.splice(i, 1);
                }
                else {
                    actRelationship.targetModel.moodConcept = OpenIZModel.ActMoodKeys.Intent;
                    actRelationship.targetModel.actTime = encounter.actTime;
                    //actRelationship.targetModel.startTime = actRelationship.targetModel.stopTime = null;
                    actRelationship.targetModel.participation.RecordTarget = encounter.participation.RecordTarget;
                    actRelationship.targetModel.participation.Location = encounter.participation.Location;
                    actRelationship.targetModel.participation.Authororiginator = encounter.participation.Authororiginator;
                    if (actRelationship.targetModel.participation && actRelationship.targetModel.participation.Product)
                        delete (actRelationship.targetModel.participation.Product.playerModel);

                    delete (actRelationship.targetModel.moodConceptModel);
                    // Now push the act relationship into the submission bundle
                    bundle.item.push(actRelationship.targetModel);
                    delete (actRelationship.targetModel);
                }
            }

            // Is there an encounters collection above us? if so we should probably push this up
            if ($scope.$parent.encounters)
                $scope.$parent.encounters.push(encounter);

            // Now submit the bundle
            OpenIZ.Bundle.insertAsync({
                data: bundle,
                continueWith: function (data) {
                    $("#appointmentScheduler").modal("hide");
                },
                onException: function (ex) {
                    if (ex.message)
                        alert(ex.message);
                    else
                        console.error(ex);
                },
                finally: function () {
                    OpenIZ.App.hideWait();
                }
            });
        }
        catch (e) {
            console.error(e);
        }
        finally {
            OpenIZ.App.hideWait();
        }
    }
}]);