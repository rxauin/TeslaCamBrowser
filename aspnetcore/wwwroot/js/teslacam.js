function minimize(fileId) {
    document.getElementById("x" + fileId).style.visibility = "hidden";
    document.getElementById("o" + fileId).style.visibility = "visible";

    document.getElementById("dl" + fileId).style.visibility = "hidden";
    document.getElementById("link" + fileId).style.visibility = "hidden";
    document.getElementById("mvswap" + fileId).style.visibility = "hidden";

    document.getElementById("movie" + fileId).style.display = "none";

    document.getElementById("txt" + fileId).style.color = "red";

    var div = $("#div" + fileId);
    div.animate({ height: 70 }, "slow");

}


function maximize(fileId) {
    document.getElementById("div" + fileId).style.opacity = 1;

    document.getElementById("movie" + fileId).display = "block";;

    document.getElementById("x" + fileId).style.visibility = "visible";
    document.getElementById("o" + fileId).style.visibility = "hidden";

    document.getElementById("dl" + fileId).style.visibility = "visible";
    document.getElementById("link" + fileId).style.visibility = "visible";
    document.getElementById("mvswap" + fileId).style.visibility = "visible";

    document.getElementById("movie" + fileId).style.display = "block";

    document.getElementById("txt" + fileId).style.color = "black";

    var div = $("#div" + fileId);
    div.animate({ height: 340 }, "slow");
}


function swapVideo(fileId) {
    var currentType = document.getElementById("movCurrent" + fileId).value;

    var newVideo;
    if (currentType == "preview") {
        newVideo = document.getElementById("timelapsed" + fileId).value;
        document.getElementById("movCurrent" + fileId).value = "timelapse"
    }
    else {
        newVideo = document.getElementById("preview" + fileId).value;
        document.getElementById("movCurrent" + fileId).value = "preview";
    }
        
    document.getElementById("moviePlayer" + fileId).src = newVideo;
}


function mapDeleted() {
    var deleteElement = document.getElementById("FilesToDelete");
    
    var deleteCounter = 0;
    var output = "";
    var inputs = document.getElementsByTagName("input");
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].checked == true) {
            output += inputs[i].id.substr(3) + ":";
            deleteCounter++;
        }
    }

    deleteElement.value = output;
    if (deleteCounter == 0) {
        return false;
    }
    else if (deleteCounter == 1) {
        deleteCounter = deleteCounter + " incident";
    }
    else {
        deleteCounter = deleteCounter + " incidents";
    }

    return confirm("This will delete " + deleteCounter + ". Are you sure?");
}

function toggleChecks() {

    var chkValue = document.getElementById("chkMain").checked;

    var inputs = document.getElementsByTagName("input");
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].id != "chkMain") {
            inputs[i].checked = chkValue;
        }
    }
}