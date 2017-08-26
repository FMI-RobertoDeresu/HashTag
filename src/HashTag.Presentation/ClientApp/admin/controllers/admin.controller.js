(function() {

    angular
        .module("admin")
        .controller("adminController", adminController);

    adminController.$inject = ["adminService"];

    function adminController(adminService) {
        let adminVM = this;

        adminVM.isBusy = false;
        adminVM.errors = [];
        adminVM.messages = [];
        adminVM.responseObject = null;

        adminVM.buildClusters = function() { adminServiceCall(adminService.buildClusters); };
        adminVM.savePredictionClasses = function() { adminServiceCall(adminService.savePredictionClasses); };
        adminVM.saveSamplesPhotosPredictions = function() { adminServiceCall(adminService.saveSamplesPhotosPredictions); };
        adminVM.kmeansResearch = function() { adminServiceCall(adminService.kmeansResearch) };
        adminVM.createTestDirectory = function() { adminServiceCall(adminService.createTestDirectory) };
        adminVM.bindPhotosToClusters = function() { adminServiceCall(adminService.bindPhotosToClusters) };

        adminService.getKMeansResearchData().then(response => drawKMeansChart(adminVM, response));

        function adminServiceCall(action) {
            adminVM.isBusy = true;
            adminVM.errors = [];
            adminVM.messages = [];
            adminVM.responseObject = null;

            return action().then(function(response) {
                adminVM.isBusy = false;
                adminVM.responseObject = response.data;
                adminVM.messages = response.messages;
                adminVM.messagesCssClass = response.success
                    ? "alert-danger"
                    : "alert-success";
                
                return response;
            });
        }
    }

    function drawKMeansChart(adminVM, response) {
        if (!response.success) {
            adminVM.responseObject = response.data;
            adminVM.messages = response.messages;
            adminVM.messagesCssClass = "alert-danger";
            return;
        }

        let labels = response.data.map(x => x.clusters);

        let maxPrecision = Math.max(...response.data.map(x => x.result));
        let precisionData = response.data.map(x => (x.result / maxPrecision) * 100);

        let maxTime = Math.max(...response.data.map(x => x.searchMilliseconds));
        let timeData = response.data.map(x => (x.searchMilliseconds / maxTime) * 100);

        let combinedData = precisionData.map((p, i) => precisionData[i] / timeData[i]);

        let ctx = document.getElementById("kMeansChart");
        let kMeansChart = new Chart(ctx, {
            type: "line",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: "precizie = numarul de cuvinte comune dintre descrierea atribuita de aplicatie si descrierea originala / numarul de cuvinte din descriere originala",
                        data: precisionData,
                        fill: false,
                        borderColor: "#36a2eb",
                        lineTension: 0.1
                    },
                    {
                        label: "timp cautare = timpul total de cautare a celei mai similare imagini pentru 300 de imagini, cautarea a fost efectuata pe un set de 7700 de imagini",
                        data: timeData,
                        fill: false,
                        borderColor: "#ff6384",
                        lineTension: 0.1
                    },
                    {
                        label: "precizie/timp cautare",
                        data: combinedData,
                        fill: false,
                        borderColor: "#4bc0c0",
                        lineTension: 0.1
                    }
                ]
            },
            options: {
                title: {
                    display: true,
                    text: "Acest grafic ilustreaza rezultatele obtinute pentru diferite valori ale numarului de clusteri. Pentru afisare valorile au fost reduse la intervalul 0-100 pastrand relatile dintre ele."
                }
            }
        });
    }

}());