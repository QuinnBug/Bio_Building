mergeInto(LibraryManager.library, {

  GetJSON: function (string path, string objectName, string callback, string fallback) {
    var parsedPath = Pinter.stringify(path);
    var parsedObjectName = Pinter.stringify(objectName);
    var parsedCallback = Pinter.stringify(callback);
    var parsedFallback = Pinter.stringify(fallback);

    try {
        firebase.database().ref(parsedPath).once.('value').then(function(snapshot){
            unityInstance.Module.SendMessage(objectName, callback, JSON.strigify(snapshot.val()));
    });

    } catch(error) {
        unityInstance.Module.SendMessage(objectName, fallback, "There was an error: " + error.message);
    }
  }

});