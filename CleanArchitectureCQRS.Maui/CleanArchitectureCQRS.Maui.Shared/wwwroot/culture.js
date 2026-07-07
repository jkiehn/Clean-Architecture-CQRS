window.entityWorkspaceCulture = {
    get: function () {
        return window.localStorage.getItem("entity-workspace.locale");
    },
    set: function (value) {
        window.localStorage.setItem("entity-workspace.locale", value);
    },
    getSystem: function () {
        return navigator.language || "en-US";
    }
};
