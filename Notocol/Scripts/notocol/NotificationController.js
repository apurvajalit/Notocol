
app.controller("NotificationController", function ($scope, $http, constructNotificationFilter, $sce) {
    $scope.notifications = [];
    $scope.numberOfUnreadNotifications = 0;
    $http({
        method: 'GET',
        url: '/User/GetUserNotifications'
    }).then(function successCallback(response) {
        var i;
        for (i = 0; i < response.data.length; i++) {
            
            $scope.notifications[$scope.notifications.length] = {
                id: response.data[i].id,
                text: $sce.trustAsHtml(constructNotificationFilter(response.data[i])),
                date: new Date(parseInt(response.data[i].notificationDate.substr(6))),
                readStatus: response.data[i].readStatus == 0 ? false : true,
                sourceUserID: response.data[i].sourceUserID,
                tags: response.data[i].tags
            }

            if (response.data[i].readStatus == 0) $scope.numberOfUnreadNotifications++;

        }
        
    }, function errorCallback(response) {

    });
    var MarkNotificationAsRead = function (index, syncServer) {
       $scope.notifications[index].readStatus = true;
       $scope.numberOfUnreadNotifications--;

       if (syncServer) {
            $http({
                method: 'POST',
                url: '/User/MarkUserNotificationAsRead/'+$scope.notifications[index].id
            })
        }
    }

    $scope.MarkAllRead = function () {
        console.log("Marking all notifications as read")
        $http({
            method: 'POST',
            url: '/User/MarkAllUserNotificationsAsRead'
        }).success(function (data, status, headers, config) {
            
            for (var i = 0; i < $scope.notifications.length; i++) {
                MarkNotificationAsRead(i, false);
            }
        }).error(function (data, status, headers, config) {
            //console.log('error', data, status);
        }).catch(function (error) {
            //console.log('catch', error);
        });

    }

    $scope.NotificationClickHandler = function (index) {
        //console.log("Click handler called for notification " + index);
        MarkNotificationAsRead(index, true);
        location.href = "/Source/SourceUserNotes?sourceUserID=" + $scope.notifications[index].sourceUserID
    }

});
app.filter('constructNotification', function (charactersFilter) {

    return function (notification) {

        var output = notification.notificationDetailText;
        output = output.replace("$$u", "<span class='user-notification-username'>" + notification.secondaryUserName + "</span>");
        output = output.replace("$$ti", "<span class='user-notification-title'>\"" + charactersFilter(notification.sourceTitle, 40) + "\"</span>");
        var tagString = "";
        if (notification.tags != null && notification.tags.length > 0) {
            var tags = notification.tags.split(",");
            for (var i = 0; i < tags.length; i++) {
                if(tags[i].length > 0) tagString += "<span class='user-notification-tag'>" + tags[i] + "</span>"
            }
            
        }
        
        output = output.replace("$$tg", tagString);
        output = output.replace("$$n", "<span class='user-notification-note'>\"" + charactersFilter(notification.note, 40) + "\"</span>");
        
        return output;

    }

});