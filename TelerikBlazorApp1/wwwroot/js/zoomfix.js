function recalcOffset() {

    // grid setting, row height and page size
    var rowHeight = 26;
    var pageSize = 40;
    var ratio = window.devicePixelRatio;

    // THIS IS IMPORTANT: chrome calculation is off, any decimal value after ratio is rounded.
    // any ratio that not result in a whole number now will get wrong height
    // it should be always 26, but for 80%, it is 26.25 using below calculation, for 90% it is 25.55555555
    var newRowHeight = Math.round(rowHeight * ratio) / ratio;
    console.debug("new row height: " + newRowHeight + " after zoom ratio:" + ratio)
    // the true baseOffset considering the chrome's wrong height
    // it should always be 26 * 40 = 1040, but in 80% it should be 1050, for 90% it is 1022.22222222222
    // once we adjust the page height based on new row height, the offset on top row is fixed.
    var baseOffset = newRowHeight * pageSize * -1;
    console.debug("new page height = new row height * page size: " + baseOffset);
    // ------------- this is just to find the telerik scroll pane -------------------
    var virtDivs = document.querySelectorAll(".k-virtual-position");
    if (virtDivs === undefined) {
        return;
    }

    var virtDiv = undefined;
    var strOffset = undefined;

    for (i = 0; i < virtDivs.length; ++i) {
        var tempViewPort = virtDivs[i];
        var tempStrOffset = tempViewPort.getAttribute("data-translate");
        if (tempStrOffset != undefined) {
            strOffset = tempStrOffset;
            virtDiv = tempViewPort;
            break;
        }
    }

    if (virtDiv === undefined) {
        return;
    }
    // ------------------------------ this is just to apply the corrected TranslateY value -------------
    var contDiv = virtDiv.parentElement.parentElement;
    var scrollTop = contDiv.scrollTop;

    var newOffset = Math.round(baseOffset + scrollTop);
    var strNewOffset = "" + newOffset;

    var style = virtDiv.getAttribute("style");
    var newStyle = style.replace(strOffset, strNewOffset);
    virtDiv.setAttribute("style", newStyle);
    console.debug(style + " is replaced by " + newStyle);
};