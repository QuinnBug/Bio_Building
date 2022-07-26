mergeInto(LibraryManager.library, {

  GetJSON: function (path, objectName, callback, fallback) {
    var parsedPath = Pinter.stringify(path);
    var parsedObjectName = Pinter.stringify(objectName);
    var parsedCallback = Pinter.stringify(callback);
    var parsedFallback = Pinter.stringify(fallback);

    try {
        firebase.database().ref(parsedPath).once.('value').then(function(snapshot){
            window.unityInstance.sendMessage(parsedObjectName, parsedCallback, JSON.strigify(snapshot.val()));
    });

    } catch(error) {
        window.unityInstance.sendMessage(parsedObjectName, parsedFallback, "There was an error: " + error.message);
    }
  }

  CreateUser: function()

  AttemptLogin: function()

});