(function () {
    window.zoomTracker = window.zoomTracker || {};
    window.cachedMediaQueriesList = [];

    window.zoomTracker = {
        init: function (gridCssClass) {
            // creating a small poc for media tracking. However, "onresize" event and ResizeObserver are also good alternatives for a poc
            const gridVirtualContainer = document.querySelector(`.${gridCssClass} .k-virtual-content`);

            const mqString = `(resolution: ${window.devicePixelRatio}dppx)`;

            window.cachedMediaQueriesList.push(mqString);

            const setTranslateY = (e) => {
               let currentMqString = `(resolution: ${window.devicePixelRatio}dppx)`;

               //recalc the offset only when no scrolling is applied
               if (gridVirtualContainer.scrollTop == 0) {
                   window.recalcOffset();
               }

               if (!e.matches && !window.cachedMediaQueriesList.includes(currentMqString)) {
                   window.cachedMediaQueriesList.push(currentMqString);

                   matchMedia(currentMqString).addEventListener("change", setTranslateY);
               }
            }

            setTranslateY({ matches: true });

            matchMedia(mqString).addEventListener("change", setTranslateY);
        }
    }
})();
