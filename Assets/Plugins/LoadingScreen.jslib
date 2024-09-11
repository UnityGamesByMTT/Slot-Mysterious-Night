
mergeInto(LibraryManager.library, {
    delayHideLoadingScreen: function() {
        var loadingScreen = document.getElementById("loading-screen");
        loadingScreen.style.opacity = "0";
        loadingScreen.style.zIndex = "-1";
        loadingScreen.style.display = "none";
        loadingScreen.remove();
    }
});


