// Size-to-notation mapping
var sizeToNotationMapping = [
    { size: 0, notation: "bytes" }, 
    { size: 1000, notation: "Kb" }, 
    { size: 1000000, notation: "Mb" },
    { size: 1000000000, notation: "Gb" }
];

// Default http fail handler
function defaultFail (http) {
    console.warn("Request failed:");
    console.warn("URL: " + http.responseURL);
    console.warn("Status: " + http.status);
    console.log(http.response);
}

// HTTP request boilerplate
function beginHttpGet ({url, headers={}, success, fail=defaultFail}) {
    var http = new XMLHttpRequest();
    http.open("GET", url);
    for (var key in headers) {
        http.setRequestHeader(key, headers[key]);
    }
    http.onreadystatechange = function() {
        if (http.readyState == XMLHttpRequest.DONE) {
            if (http.status >= 200 && http.status < 300) {
                success(http);
            }
            else {
                fail(http);
            }
        }
    };
    http.send();
}

var app = new Vue({
    el: "#main",
    data: {
        dirUrlPrefix: "/fs?path=",
        parent: "",
        base: DIR_BASE,
        dirs: [],
        files: []
    },
    methods: {
        goTo: function(dirName){
            var capturedThis = this;
            var currentUrl = this.dirUrlPrefix + dirName;
            beginHttpGet({
                url: currentUrl, 
                headers: {
                    "JSON-Only": "true"
                },
                success: function(http) {
                    var response = (http.responseType == "json") ? 
                        http.response : JSON.parse(http.response);
                    console.log(http.response);
                    capturedThis.base = response["Base"];
                    capturedThis.dirs = response["Dirs"];
                    capturedThis.files = response["Files"];
                    capturedThis.parent = response["Parent"];
                    document.title = SITE_NAME + ": " + dirName;
                    history.pushState(null, document.title, currentUrl);
                }
            });
        },
        goToDir: function(dirName){
            this.goTo(this.base + dirName);
        },
        goToParent: function(){
            this.goTo(this.parent);
        },
        start: function(){
            this.goToDir("");
        },
        toConvenient: function(givenBytes) {
            var currentNotation = sizeToNotationMapping.findLast(item => item.size < givenBytes);
            return ((currentNotation.size == 0) ? givenBytes.toString() : (givenBytes / currentNotation.size).toFixed(2)) + " " + currentNotation.notation;
        }
    }
});

app.start();
